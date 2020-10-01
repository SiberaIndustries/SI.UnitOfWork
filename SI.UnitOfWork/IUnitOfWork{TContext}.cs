using SI.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public interface IUnitOfWork<TContext> : IDisposable
        where TContext : IDbContext
    {
        TContext DbContext { get; }
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        TRepository GetRepository<TEntity, TRepository>() where TEntity : class where TRepository : IRepository<TEntity>;
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
