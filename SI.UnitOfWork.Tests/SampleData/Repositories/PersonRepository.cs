using SI.UnitOfWork.Tests.SampleData.Entities;

namespace SI.UnitOfWork.Tests.SampleData.Repositories
{
    public class PersonRepository : EFRepository<Person>, IPersonRepository
    {
        public PersonRepository(EFContext dbContext)
            : base(dbContext)
        {
        }

        public string CustomMethod()
        {
            return "Hello " + nameof(CustomMethod);
        }
    }

    public interface IPersonRepository : IRepository<Person>
    {
        public string CustomMethod();
    }
}
