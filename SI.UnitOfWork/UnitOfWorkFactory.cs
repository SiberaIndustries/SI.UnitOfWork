using Microsoft.Extensions.DependencyInjection;
using SI.UnitOfWork.Interfaces;
using System;

namespace SI.UnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IUnitOfWork<TContext> GetUnitOfWork<TContext>()
            where TContext : IDbContext
        {
            return (IUnitOfWork<TContext>)serviceProvider.GetRequiredService(typeof(IUnitOfWork<TContext>));
        }
    }
}
