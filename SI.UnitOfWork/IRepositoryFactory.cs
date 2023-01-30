using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public interface IRepositoryFactory
    {
#if NET6_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Types might be removed")]
#endif
        IRepository<TEntity>? GetRepository<TEntity>()
            where TEntity : class;

        TRepository GetRepository<TEntity, TRepository>()
            where TEntity : class
            where TRepository : class, IRepository<TEntity>;
    }

    public interface IRepositoryFactory<out TContext> : IRepositoryFactory
        where TContext : IDbContext
    {
        TContext DbContext { get; }
    }
}
