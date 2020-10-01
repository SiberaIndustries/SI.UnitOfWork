using Microsoft.EntityFrameworkCore;
using SI.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public class EFUnitOfWork<TContext> : IUnitOfWork<TContext>
        where TContext : DbContext, IDbContext
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDictionary<Type, object> repositories = new Dictionary<Type, object>();
        private bool disposed = false;

        public EFUnitOfWork(TContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public TContext DbContext { get; }

        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new EFRepository<TEntity>(DbContext);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        public TRepository GetRepository<TEntity, TRepository>()
            where TEntity : class
            where TRepository : IRepository<TEntity>
        {
            var type = typeof(TRepository);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = serviceProvider.GetService(type);
            }

            return (TRepository)repositories[type];
        }

        public int SaveChanges() =>
            DbContext.SaveChanges();

        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            DbContext.SaveChangesAsync(ct);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (repositories != null)
                    {
                        repositories.Clear();
                    }

                    DbContext.Dispose();
                }
            }

            disposed = true;
        }
    }
}
