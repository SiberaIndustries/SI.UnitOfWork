using SI.UnitOfWork.Tests.SampleData.Entities;

namespace SI.UnitOfWork.Tests.SampleData.Repositories
{
    public class PersonRepository : EFRepository<Person>
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
}
