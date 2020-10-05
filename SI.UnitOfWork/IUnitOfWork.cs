using SI.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public interface IUnitOfWork : IRepositoryFactory, IDisposable
    {
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork, IRepositoryFactory<TContext>
        where TContext : IDbContext
    {
    }
}
