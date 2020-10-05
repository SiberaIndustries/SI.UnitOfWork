using Microsoft.EntityFrameworkCore;

namespace SI.UnitOfWork.Tests.SampleData.Contexts
{
    public class CustomEFContext : EFContext
    {
        public CustomEFContext(DbContextOptions<EFContext> options)
            : base(options)
        {
        }
    }
}
