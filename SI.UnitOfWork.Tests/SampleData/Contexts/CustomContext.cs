using Microsoft.EntityFrameworkCore;

namespace SI.UnitOfWork.Tests.SampleData.Contexts
{
    public class CustomContext : EFContext
    {
        public CustomContext(DbContextOptions<EFContext> options)
            : base(options)
        {
        }
    }
}
