using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public interface IUnitOfWork : IRepositoryFactory, IDisposable
    {
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork, IRepositoryFactory<TContext>
        where TContext : IDbContext
    {
    }
}
