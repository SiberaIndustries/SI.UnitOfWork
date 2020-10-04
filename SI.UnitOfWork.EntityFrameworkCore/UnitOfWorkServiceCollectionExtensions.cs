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
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
            services.AddScoped<IRepositoryFactory, EFUnitOfWork>();

            services.AddTransient(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }

        public static IServiceCollection AddEFUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext, IDbContext
        {
            services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IUnitOfWork<TContext>, EFUnitOfWork<TContext>>();
            services.AddScoped<IRepositoryFactory, EFUnitOfWork>();
            return services;
        }
    }
}
