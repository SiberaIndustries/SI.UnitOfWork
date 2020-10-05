using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();
            services.TryAddScoped<IRepositoryFactory, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
          where TContext : class, IDbContext
        {
            services.TryAddScoped<TContext>();
            services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.TryAddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            services.TryAddScoped<IRepositoryFactory<TContext>, UnitOfWork<TContext>>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext1, TContext2>(this IServiceCollection services)
          where TContext1 : class, IDbContext
          where TContext2 : class, IDbContext
        {
            services.AddUnitOfWork<TContext1>();
            services.AddUnitOfWork<TContext2>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext1, TContext2, TContext3>(this IServiceCollection services)
          where TContext1 : class, IDbContext
          where TContext2 : class, IDbContext
          where TContext3 : class, IDbContext
        {
            services.AddUnitOfWork<TContext1>();
            services.AddUnitOfWork<TContext2>();
            services.AddUnitOfWork<TContext3>();
            return services;
        }
    }
}
