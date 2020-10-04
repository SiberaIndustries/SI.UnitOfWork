using SI.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public interface IUnitOfWork<TContext> : IRepositoryFactory, IDisposable
        where TContext : IDbContext
    {
        TContext DbContext { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
