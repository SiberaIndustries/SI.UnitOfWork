using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TContext> GetUnitOfWork<TContext>() where TContext : IDbContext;
    }
}
