using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddEFUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
            services.AddScoped<IRepositoryFactory, EFUnitOfWork>();

            services.AddScoped<DbContext, EFContext>();
            services.AddScoped<IDbContext, EFContext>();
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }

        public static IServiceCollection AddEFUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext, IDbContext
        {
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IUnitOfWork<TContext>, EFUnitOfWork<TContext>>();
            services.AddScoped<IRepositoryFactory, EFUnitOfWork>();

            services.AddScoped<DbContext, TContext>();
            services.AddScoped<IDbContext, TContext>();
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }
    }
}
