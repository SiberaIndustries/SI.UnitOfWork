using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public interface IUnitOfWork : IUnitOfWork<IDbContext>
    {
    }
}
