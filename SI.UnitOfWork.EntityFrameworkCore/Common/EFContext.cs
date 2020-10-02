using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SI.UnitOfWork.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork.Common
{
    public class EFContext : DbContext, IDbContext
    {
        public const string IAuditableEntity_CreatedByPropertyName = "_CreatedBy";
        public const string IAuditableEntity_CreatedPropertyName = "_CreatedAt";
        public const string IAuditableEntity_LastModifiedByPropertyName = "_LastModifiedBy";
        public const string IAuditableEntity_LastModifiedPropertyName = "_LastModifiedAt";
        public const string IMultiTenantEntity_TenantIdPropertyName = "_TenantId";
        public const string ISoftDeleteEntity_IsDeletedPropertyName = "_IsDeleted";
        private EntityMarker entityMarker;
        private IIdentityProvider? tenantProvider;
        private IIdentityProvider? userProvider;

        public EFContext(DbContextOptions<EFContext> options)
            : base(options)
        {
        }

        private Guid GetTenantId()
        {
            if (tenantProvider == null)
            {
                try
                {
                    tenantProvider = this.GetService<ITentantProvider>();
                }
                catch (InvalidOperationException)
                {
                    tenantProvider = new NullIdentityProvider();
                }
            }

            return tenantProvider.Identity;
        }

        private Guid GetUserId()
        {
            if (userProvider == null)
            {
                try
                {
                    userProvider = this.GetService<IUserProvider>();
                }
                catch (InvalidOperationException)
                {
                    userProvider = new NullIdentityProvider();
                }
            }

            return userProvider.Identity;
        }

        private void ResolveEntityDefinitions<TEntity>(EntityTypeBuilder<TEntity> entityTypeBuilder, DatabaseFacade database)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            foreach (var prop in entityType.GetProperties().Where(x => x.PropertyType.GetInterfaces().Contains(typeof(IOwnedType))))
            {   // Owned types
                entityTypeBuilder.OwnsOne(prop.PropertyType, prop.Name);
            }

            var interfaces = entityType.GetInterfaces();
            if (database.IsSqlServer() && interfaces.Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(IConcurrentEntity<>)))
            {   // Concurrent entities in MSSQL server
                entityTypeBuilder.Property(nameof(IConcurrentEntity<TEntity>.ConcurrencyToken)).IsRowVersion();
            }
            else if (database.IsNpgsql() && interfaces.Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(IConcurrentEntity<>)))
            {   // Concurrent entities in PostgreSql server
                entityTypeBuilder.Property(nameof(IConcurrentEntity<TEntity>.ConcurrencyToken)).HasColumnName("xmin").HasColumnType("xid").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();
            }

            entityTypeBuilder.IsMemoryOptimized(interfaces.Contains(typeof(IPerformanceCritical)));

            // Soft deletion / Multi tenant entities
            if (interfaces.Contains(typeof(ISoftDeleteEntity)) && interfaces.Contains(typeof(IMultiTenantEntity)))
            {
                entityMarker |= EntityMarker.HasSoftDeletionEntities;
                entityTypeBuilder.Property(IMultiTenantEntity_TenantIdPropertyName);
                entityTypeBuilder.Property<bool>(ISoftDeleteEntity_IsDeletedPropertyName);
                entityTypeBuilder.HasQueryFilter(x => !EF.Property<bool>(x, ISoftDeleteEntity_IsDeletedPropertyName) && EF.Property<Guid>(x, IMultiTenantEntity_TenantIdPropertyName) == GetTenantId());
            }
            else if (interfaces.Contains(typeof(ISoftDeleteEntity)))
            {
                entityMarker |= EntityMarker.HasSoftDeletionEntities | EntityMarker.HasMultiTenantEntities;
                entityTypeBuilder.Property<bool>(ISoftDeleteEntity_IsDeletedPropertyName);
                entityTypeBuilder.HasQueryFilter(x => !EF.Property<bool>(x, ISoftDeleteEntity_IsDeletedPropertyName));
            }
            else if (interfaces.Contains(typeof(IMultiTenantEntity)))
            {
                entityMarker |= EntityMarker.HasMultiTenantEntities;
                entityTypeBuilder.Property<Guid>(IMultiTenantEntity_TenantIdPropertyName);
                entityTypeBuilder.HasQueryFilter(x => EF.Property<Guid>(x, IMultiTenantEntity_TenantIdPropertyName) == GetTenantId());
            }

            if (interfaces.Contains(typeof(IAuditableEntity)))
            {
                entityMarker |= EntityMarker.HasAuditableEntities;
                entityTypeBuilder.Property<Guid>(IAuditableEntity_CreatedByPropertyName);
                entityTypeBuilder.Property<DateTime>(IAuditableEntity_CreatedPropertyName);
                entityTypeBuilder.Property<Guid>(IAuditableEntity_LastModifiedByPropertyName);
                entityTypeBuilder.Property<DateTime>(IAuditableEntity_LastModifiedPropertyName);
            }
        }

        private void Intercept()
        {
            if (entityMarker == EntityMarker.None)
            {
                return;
            }

            var timestamp = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Deleted && entry.State != EntityState.Added && entry.State != EntityState.Modified)
                {
                    continue;
                }

                var interfaces = entry.Entity.GetType().GetInterfaces();

                // Set _IsDeleted flag to 1 instead of removing the entity
                if (entityMarker.HasFlag(EntityMarker.HasSoftDeletionEntities) && entry.State == EntityState.Deleted && interfaces.Contains(typeof(ISoftDeleteEntity)))
                {
                    entry.Property(ISoftDeleteEntity_IsDeletedPropertyName).CurrentValue = true;
                    entry.State = EntityState.Modified;
                }

                // Set timestamps and identity
                if (entityMarker.HasFlag(EntityMarker.HasAuditableEntities) && (entry.State == EntityState.Added || entry.State == EntityState.Modified) && interfaces.Contains(typeof(IAuditableEntity)))
                {
                    entry.Property(IAuditableEntity_LastModifiedPropertyName).CurrentValue = timestamp;
                    entry.Property(IAuditableEntity_LastModifiedByPropertyName).CurrentValue = GetUserId();
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(IAuditableEntity_CreatedPropertyName).CurrentValue = timestamp;
                        entry.Property(IAuditableEntity_CreatedByPropertyName).CurrentValue = GetUserId();
                    }
                }

                // Set tenant
                if (entityMarker.HasFlag(EntityMarker.HasMultiTenantEntities) && entry.State == EntityState.Added && interfaces.Contains(typeof(IMultiTenantEntity)))
                {
                    entry.Property(IMultiTenantEntity_TenantIdPropertyName).CurrentValue = GetTenantId();
                }
            }
        }

        public int SaveChangesWithoutInterception(bool? acceptAllChangesOnSuccess = null) =>
            acceptAllChangesOnSuccess.HasValue
            ? base.SaveChanges(acceptAllChangesOnSuccess.Value)
            : base.SaveChanges();

        public Task<int> SaveChangesWithoutInterceptionAsync(bool? acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
            acceptAllChangesOnSuccess.HasValue
            ? base.SaveChangesAsync(acceptAllChangesOnSuccess.Value, cancellationToken)
            : base.SaveChangesAsync(cancellationToken);

        public override int SaveChanges()
        {
            Intercept();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            Intercept();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Intercept();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            Intercept();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            var entityTypes = modelBuilder.Model.GetEntityTypes().Select(x => x.ClrType).Where(type => type.GetInterfaces().Contains(typeof(IEntity))).ToArray();
            foreach (var entityType in entityTypes)
            {   // Get and execute modelBuilder.Entity<TEntity>()
                var mbEntityOfT = typeof(ModelBuilder).GetMethods().First(x => x.Name == nameof(modelBuilder.Entity)).MakeGenericMethod(entityType);
                var builder = mbEntityOfT.Invoke(modelBuilder, null);

                // Get and execute ResolveEntityDefinitions<TEntity>(..)
                var resolveEntityDefinitionsOfT = GetType().GetMethod(nameof(ResolveEntityDefinitions), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType);
                resolveEntityDefinitionsOfT.Invoke(this, new object[] { builder, Database });
            }
        }

        internal sealed class NullIdentityProvider : IIdentityProvider
        {
            public Guid Identity => Guid.Empty;
        }

        [Flags]
        internal enum EntityMarker
        {
            None = 0,
            HasAuditableEntities = 1,
            HasSoftDeletionEntities = 2,
            HasMultiTenantEntities = 4,
        }
    }
}
