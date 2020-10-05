using System;

namespace SI.UnitOfWork
{
    public class EFUnitOfWork : EFUnitOfWork<EFContext>
    {
        public EFUnitOfWork(EFContext dbContext, IServiceProvider serviceProvider)
            : base(dbContext, serviceProvider)
        {
        }
    }
}
