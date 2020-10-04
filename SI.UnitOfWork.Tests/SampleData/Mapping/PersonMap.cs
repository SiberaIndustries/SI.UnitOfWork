using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SI.UnitOfWork.Tests.SampleData.Entities;

namespace SI.UnitOfWork.Tests.SampleData.Mapping
{
    public class PersonMap : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
        }
    }
}
