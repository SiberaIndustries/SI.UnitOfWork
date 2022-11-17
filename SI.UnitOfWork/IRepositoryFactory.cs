using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity>? GetRepository<TEntity>()
            where TEntity : class;

        TRepository GetRepository<TEntity, TRepository>()
            where TEntity : class
            where TRepository : class, IRepository<TEntity>;
    }

    public interface IRepositoryFactory<TContext> : IRepositoryFactory
        where TContext : IDbContext
    {
    }
}
