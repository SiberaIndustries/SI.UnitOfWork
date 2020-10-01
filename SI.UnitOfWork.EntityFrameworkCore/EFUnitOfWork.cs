using Microsoft.EntityFrameworkCore.Infrastructure;
using SI.UnitOfWork.Common;
using SI.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly EFContext dbContext;
        private readonly IDictionary<Type, object> repositories = new Dictionary<Type, object>();
        private bool disposed = false;

        public EFUnitOfWork(EFContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IDbContext DbContext => dbContext;

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
                repositories[type] = dbContext.GetService<TRepository>();
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
