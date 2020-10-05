using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddEFUnitOfWork(this IServiceCollection services)
        {
            services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.TryAddScoped<IUnitOfWork, EFUnitOfWork>();
            services.TryAddScoped<IRepositoryFactory, EFUnitOfWork>();

            services.TryAddScoped<DbContext, EFContext>();
            services.TryAddScoped<IDbContext, EFContext>();
            services.TryAddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }

        public static IServiceCollection AddEFUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext, IDbContext
        {
            services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.TryAddScoped<IUnitOfWork<TContext>, EFUnitOfWork<TContext>>();
            services.TryAddScoped<IRepositoryFactory<TContext>, EFUnitOfWork<TContext>>();

            // services.TryAddScoped<DbContext, TContext>()
            // services.TryAddScoped<IDbContext, TContext>()
            services.TryAddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }

        public static IServiceCollection AddEFUnitOfWork<TContext1, TContext2>(this IServiceCollection services)
            where TContext1 : DbContext, IDbContext
            where TContext2 : DbContext, IDbContext
        {
            services.AddEFUnitOfWork<TContext1>();
            services.AddEFUnitOfWork<TContext2>();
            return services;
        }

        public static IServiceCollection AddEFUnitOfWork<TContext1, TContext2, TContext3>(this IServiceCollection services)
            where TContext1 : DbContext, IDbContext
            where TContext2 : DbContext, IDbContext
            where TContext3 : DbContext, IDbContext
        {
            services.AddEFUnitOfWork<TContext1>();
            services.AddEFUnitOfWork<TContext2>();
            services.AddEFUnitOfWork<TContext3>();
            return services;
        }
    }
}
