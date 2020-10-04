namespace SI.UnitOfWork
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;

        TRepository GetRepository<TEntity, TRepository>()
            where TEntity : class
            where TRepository : IRepository<TEntity>;
    }
}
