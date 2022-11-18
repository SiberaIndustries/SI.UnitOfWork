using SI.UnitOfWork.Interfaces;
using System.Reflection;

namespace SI.UnitOfWork
{
    public class UnitOfWork<TContext> : UnitOfWorkBase<TContext>
        where TContext : IDbContext
    {
        private readonly IServiceProvider serviceProvider;
        private bool disposed;

        public UnitOfWork(TContext dbContext, IServiceProvider serviceProvider)
            : base(dbContext)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override IRepository<TEntity>? GetRepository<TEntity>()
        {
            var repository = serviceProvider.GetService(typeof(IRepository<TEntity>));
            if (repository == null)
            {
                var type = Assembly.GetCallingAssembly().GetTypes().SingleOrDefault(x => x.IsInterface && x.GetInterfaces().Contains(typeof(IRepository<TEntity>)));
                if (type != null)
                {
                    repository = serviceProvider.GetService(type);
                }
            }

            return (IRepository<TEntity>?)repository;
        }

        public override TRepository GetRepository<TEntity, TRepository>()
        {
            var repository = serviceProvider.GetService(typeof(TRepository));
            return (TRepository)repository!;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            base.Dispose(disposing);
        }
    }

    public class UnitOfWork : UnitOfWork<IDbContext>
    {
        public UnitOfWork(IDbContext dbContext, IServiceProvider serviceProvider)
            : base(dbContext, serviceProvider)
        {
        }
    }
}
