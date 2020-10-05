using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public interface IUnitOfWork<TContext> : IUnitOfWork, IRepositoryFactory<TContext>
        where TContext : IDbContext
    {
    }
}
