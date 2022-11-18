using SI.UnitOfWork.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork.Tests.SampleData.Contexts
{
    public sealed class CustomContext : IDbContext
    {
        public int SaveChanges()
        {
            return 42;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return Task.FromResult(42);
        }

        public void Dispose()
        {
        }
    }
}
