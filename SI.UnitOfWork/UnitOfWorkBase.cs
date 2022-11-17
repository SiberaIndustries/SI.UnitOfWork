﻿using SI.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public abstract class UnitOfWorkBase<TContext> : IUnitOfWork<TContext>
        where TContext : IDbContext
    {
        private readonly TContext dbContext;
        private bool disposed;

        protected UnitOfWorkBase(TContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual TContext DbContext => dbContext;

        public abstract IRepository<TEntity>? GetRepository<TEntity>()
            where TEntity : class;

        public abstract TRepository GetRepository<TEntity, TRepository>()
            where TEntity : class
            where TRepository : class, IRepository<TEntity>;

        public virtual int SaveChanges() =>
            DbContext.SaveChanges();

        public virtual Task<int> SaveChangesAsync(CancellationToken ct = default) =>
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
                DbContext?.Dispose();
            }

            disposed = true;
        }
    }
}
