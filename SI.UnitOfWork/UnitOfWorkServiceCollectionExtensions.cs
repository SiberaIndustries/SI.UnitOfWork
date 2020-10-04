﻿using Microsoft.Extensions.DependencyInjection;
using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepositoryFactory, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
          where TContext : IDbContext
        {
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            services.AddScoped<IRepositoryFactory, UnitOfWork<TContext>>();
            return services;
        }
    }
}