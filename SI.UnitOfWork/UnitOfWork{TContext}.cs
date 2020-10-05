using SI.UnitOfWork.Interfaces;
using System;

namespace SI.UnitOfWork
{
    public class UnitOfWork<TContext> : UnitOfWorkBase<TContext>
        where TContext : IDbContext
    {
        private readonly IServiceProvider serviceProvider;
        private bool disposed;

        public UnitOfWork(TContext dbContext, IServiceProvider serviceProvider)
            : base(dbContext)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override IRepository<TEntity> GetRepository<TEntity>()
        {
            var repository = serviceProvider.GetService(typeof(IRepository<TEntity>));
            return (IRepository<TEntity>)repository;
        }

        public override TRepository GetRepository<TEntity, TRepository>()
        {
            var repository = serviceProvider.GetService(typeof(TRepository));
            return (TRepository)repository;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            base.Dispose(disposing);
        }
    }
}
