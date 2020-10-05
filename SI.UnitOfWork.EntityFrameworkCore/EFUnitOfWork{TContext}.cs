using Microsoft.EntityFrameworkCore;
using SI.UnitOfWork.Interfaces;
using System;

namespace SI.UnitOfWork
{
    public class EFUnitOfWork<TContext> : UnitOfWork<TContext>
        where TContext : DbContext, IDbContext
    {
        public EFUnitOfWork(TContext dbContext, IServiceProvider serviceProvider)
            : base(dbContext, serviceProvider)
        {
        }
    }
}
