namespace SI.UnitOfWork
{
    public class EFUnitOfWork : EFUnitOfWork<EFContext>
    {
        public EFUnitOfWork(EFContext dbContext)
            : base(dbContext)
        {
        }
    }
}
