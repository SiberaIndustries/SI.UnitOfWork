using SI.UnitOfWork.Interfaces;
using System;

namespace SI.UnitOfWork
{
    public class UnitOfWork : UnitOfWork<IDbContext>, IUnitOfWork
    {
        public UnitOfWork(IDbContext dbContext, IServiceProvider serviceProvider)
            : base(dbContext, serviceProvider)
        {
        }
    }
}
