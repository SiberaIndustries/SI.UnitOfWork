using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddEFUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
            services.AddTransient(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }
        public static IServiceCollection AddEFUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext, IDbContext
        {
            services.AddScoped<IUnitOfWork<TContext>, EFUnitOfWork<TContext>>();
            services.AddTransient(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }
    }
}
