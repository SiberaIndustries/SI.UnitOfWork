using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public interface IUnitOfWork<TContext> : IUnitOfWork
        where TContext : IDbContext
    {
    }
}
