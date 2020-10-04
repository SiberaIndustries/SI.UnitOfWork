using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using SI.UnitOfWork.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public class EFContext : DbContext, IDbContext
    {
        public const string IAuditableEntityCreatedByPropertyName = "_CreatedBy";
        public const string IAuditableEntityCreatedPropertyName = "_CreatedAt";
        public const string IAuditableEntityLastModifiedByPropertyName = "_LastModifiedBy";
        public const string IAuditableEntityLastModifiedPropertyName = "_LastModifiedAt";
        public const string IMultiTenantEntityTenantIdPropertyName = "_TenantId";
        public const string ISoftDeleteEntityIsDeletedPropertyName = "_IsDeleted";
        private EntityMarkers entityMarkers;
        private IIdentityProvider? tenantProvider;
        private IIdentityProvider? userProvider;

        public EFContext(DbContextOptions<EFContext> options)
            : base(options)
        {
        }

        [Flags]
        internal enum EntityMarkers
        {
            None = 0,
            HasAuditableEntities = 1,
            HasSoftDeletionEntities = 2,
            HasMultiTenantEntities = 4,
        }

        private static LambdaExpression ConvertFilterExpression(Type entityType, Expression<Func<object, bool>> filterExpression)
        {
            var newParam = Expression.Parameter(entityType);
            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);
            return Expression.Lambda(newBody, newParam);
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

        private void ResolveEntityDefinitions(Type entityType, EntityTypeBuilder entityTypeBuilder, DatabaseFacade database)
        {
            foreach (var prop in entityType.GetProperties().Where(x => x.PropertyType.GetInterfaces().Contains(typeof(IOwnedType))))
            {   // Owned types
                entityTypeBuilder.OwnsOne(prop.PropertyType, prop.Name);
            }

            var interfaces = entityType.GetInterfaces();
            if (database.IsSqlServer() && interfaces.Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(IConcurrentEntity<>)))
            {   // Concurrent entities in MSSQL server
                entityTypeBuilder.Property(nameof(IConcurrentEntity<object>.ConcurrencyToken)).IsRowVersion();
            }
            else if (database.IsNpgsql() && interfaces.Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(IConcurrentEntity<>)))
            {   // Concurrent entities in PostgreSql server
                entityTypeBuilder.Property(nameof(IConcurrentEntity<object>.ConcurrencyToken)).HasColumnName("xmin").HasColumnType("xid").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();
            }

            entityTypeBuilder.IsMemoryOptimized(interfaces.Contains(typeof(IPerformanceCritical)));

            // Soft deletion / Multi tenant entities
            if (interfaces.Contains(typeof(ISoftDeleteEntity)) && interfaces.Contains(typeof(IMultiTenantEntity)))
            {
                entityMarkers |= EntityMarkers.HasSoftDeletionEntities;
                entityTypeBuilder.Property<Guid>(IMultiTenantEntityTenantIdPropertyName);
                entityTypeBuilder.Property<bool>(ISoftDeleteEntityIsDeletedPropertyName);
                entityTypeBuilder.HasQueryFilter(ConvertFilterExpression(entityType, x => !EF.Property<bool>(x, ISoftDeleteEntityIsDeletedPropertyName) && EF.Property<Guid>(x, IMultiTenantEntityTenantIdPropertyName) == GetTenantId()));
            }
            else if (interfaces.Contains(typeof(ISoftDeleteEntity)))
            {
                entityMarkers |= EntityMarkers.HasSoftDeletionEntities | EntityMarkers.HasMultiTenantEntities;
                entityTypeBuilder.Property<bool>(ISoftDeleteEntityIsDeletedPropertyName);
                entityTypeBuilder.HasQueryFilter(ConvertFilterExpression(entityType, x => !EF.Property<bool>(x, ISoftDeleteEntityIsDeletedPropertyName)));
            }
            else if (interfaces.Contains(typeof(IMultiTenantEntity)))
            {
                entityMarkers |= EntityMarkers.HasMultiTenantEntities;
                entityTypeBuilder.Property<Guid>(IMultiTenantEntityTenantIdPropertyName);
                entityTypeBuilder.HasQueryFilter(ConvertFilterExpression(entityType, x => EF.Property<Guid>(x, IMultiTenantEntityTenantIdPropertyName) == GetTenantId()));
            }

            if (interfaces.Contains(typeof(IAuditableEntity)))
            {
                entityMarkers |= EntityMarkers.HasAuditableEntities;
                entityTypeBuilder.Property<Guid>(IAuditableEntityCreatedByPropertyName);
                entityTypeBuilder.Property<DateTime>(IAuditableEntityCreatedPropertyName);
                entityTypeBuilder.Property<Guid>(IAuditableEntityLastModifiedByPropertyName);
                entityTypeBuilder.Property<DateTime>(IAuditableEntityLastModifiedPropertyName);
            }
        }

        private void Intercept()
        {
            if (entityMarkers == EntityMarkers.None)
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
                if (entityMarkers.HasFlag(EntityMarkers.HasSoftDeletionEntities) && entry.State == EntityState.Deleted && interfaces.Contains(typeof(ISoftDeleteEntity)))
                {
                    entry.Property(ISoftDeleteEntityIsDeletedPropertyName).CurrentValue = true;
                    entry.State = EntityState.Modified;
                }

                // Set timestamps and identity
                if (entityMarkers.HasFlag(EntityMarkers.HasAuditableEntities) && (entry.State == EntityState.Added || entry.State == EntityState.Modified) && interfaces.Contains(typeof(IAuditableEntity)))
                {
                    entry.Property(IAuditableEntityLastModifiedPropertyName).CurrentValue = timestamp;
                    entry.Property(IAuditableEntityLastModifiedByPropertyName).CurrentValue = GetUserId();
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(IAuditableEntityCreatedPropertyName).CurrentValue = timestamp;
                        entry.Property(IAuditableEntityCreatedByPropertyName).CurrentValue = GetUserId();
                    }
                }

                // Set tenant
                if (entityMarkers.HasFlag(EntityMarkers.HasMultiTenantEntities) && entry.State == EntityState.Added && interfaces.Contains(typeof(IMultiTenantEntity)))
                {
                    entry.Property(IMultiTenantEntityTenantIdPropertyName).CurrentValue = GetTenantId();
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

            // Apply Entity type configuration to all relevant assemblies containing types which implement IEntityTypeConfiguration
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) && !x.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
                .Where(x => x.GetTypes().Any(x => x.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))))
                .ToList().ForEach(assembly => modelBuilder.ApplyConfigurationsFromAssembly(assembly));

            // Resolve entity type definitions for every Entity ClrType
            modelBuilder.Model.GetEntityTypes()
                .Select(x => x.ClrType)
                .Where(type => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntity<>)))
                .ToList().ForEach(entityType => ResolveEntityDefinitions(entityType, modelBuilder.Entity(entityType), Database));
        }

        internal sealed class NullIdentityProvider : IIdentityProvider
        {
            public Guid Identity => Guid.Empty;
        }
    }
}
