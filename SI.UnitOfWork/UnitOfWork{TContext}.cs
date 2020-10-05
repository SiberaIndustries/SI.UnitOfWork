using SI.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
        where TContext : IDbContext
    {
        private readonly IDbContext dbContext;
        private readonly IServiceProvider serviceProvider;
        private bool disposed;

        public UnitOfWork(TContext dbContext, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            var repository = serviceProvider.GetService(typeof(IRepository<TEntity>));
            return (IRepository<TEntity>)repository;
        }

        public TRepository GetRepository<TEntity, TRepository>()
            where TEntity : class
            where TRepository : IRepository<TEntity>
        {
            var repository = serviceProvider.GetService(typeof(TRepository));
            return (TRepository)repository;
        }

        public int SaveChanges() =>
            dbContext.SaveChanges();

        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            dbContext.SaveChangesAsync(ct);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                dbContext.Dispose();
            }

            disposed = true;
        }
    }
}
