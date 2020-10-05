using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public class EFUnitOfWork<TContext> : UnitOfWorkBase<TContext>
        where TContext : DbContext, IDbContext
    {
        private readonly DbContext dbContext;
        private bool disposed;

        public EFUnitOfWork(TContext dbContext)
            : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IRepository<TEntity> GetRepository<TEntity>()
        {
            var repository = dbContext.GetService<IRepository<TEntity>>();
            return repository;
        }

        public override TRepository GetRepository<TEntity, TRepository>()
        {
            var repository = dbContext.GetService<TRepository>();
            return repository;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                dbContext?.Dispose();
            }

            disposed = true;
            base.Dispose(disposing);
        }
    }
}
