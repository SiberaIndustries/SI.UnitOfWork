using SI.UnitOfWork.Interfaces;
using SI.UnitOfWork.Tests.SampleData.Entities;

namespace SI.UnitOfWork.Tests.SampleData.Repositories
{
    public class DefaultPersonRepository : DefaultRepository, IPersonRepository
    {
        public DefaultPersonRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public string CustomMethod()
        {
            return "Hello " + nameof(CustomMethod);
        }
    }

    public class EFPersonRepository : DefaultPersonRepository
    {
        public EFPersonRepository(EFContext dbContext)
            : base(dbContext)
        {
        }
    }

    public interface IPersonRepository : IRepository<Person>
    {
        public string CustomMethod();
    }
}
