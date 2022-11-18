namespace SI.UnitOfWork.Interfaces
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
