using SI.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
        where TContext : IDbContext
    {
        private readonly IServiceProvider serviceProvider;
        private bool disposed;

        public UnitOfWork(TContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public TContext DbContext { get; }

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
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                DbContext.Dispose();
            }

            disposed = true;
        }
    }
}
