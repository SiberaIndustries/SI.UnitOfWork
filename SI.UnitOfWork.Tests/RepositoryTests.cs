using Microsoft.EntityFrameworkCore;
using SI.UnitOfWork.Tests.SampleData.Entities;
using SI.UnitOfWork.Tests.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SI.UnitOfWork.Tests
{
    public sealed class RepositoryTests : IDisposable
    {
        private readonly DbContext dbContext;
        private readonly IRepository<Person> repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<EFContext>().UseInMemoryDatabase("InMemoryDB_" + Guid.NewGuid()).Options;
            dbContext = new EFContext(options);
            dbContext.SeedDatabase();

            repository = new EFRepository<Person>(dbContext);
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        public async Task FindExists(int id, bool exists)
        {
            var result1 = await repository.FindAsync(id.ToGuid());
            var result2 = await repository.ExistsAsync(x => x.Id.Equals(id.ToGuid()));
            Assert.Equal(exists, result1 != null);
            Assert.Equal(exists, result2);
        }

        [Fact]
        public async Task GetAll()
        {
            var result = await repository.GetAllAsync();
            Assert.Equal(DbUtilities.PersonSeed, result);
        }

        [Fact]
        public async Task Count()
        {
            var result1 = await repository.CountAsync();
            var result2 = await repository.LongCountAsync();
            Assert.Equal(2, result1);
            Assert.Equal(2L, result2);
        }

        [Fact]
        public async Task MinMaxAverageSum()
        {
            var result1 = await repository.MinAsync(selector: x => x.Birthday);
            var result2 = await repository.MaxAsync(selector: x => x.Birthday);

            var result3 = await repository.AverageAsync(selector: x => x.Birthday.Ticks);
            var result4 = DbUtilities.PersonSeed.Average(x => x.Birthday.Ticks);

            var result5 = await repository.SumAsync(selector: x => x.Birthday.Ticks);
            var result6 = DbUtilities.PersonSeed.Sum(x => x.Birthday.Ticks);

            Assert.True(result1 < result2);
            Assert.Equal(new decimal(result4), result3);
            Assert.Equal(new decimal(result6), result5);
        }

        [Fact]
        public async Task Insert()
        {
            var person = new Person { Firstname = "FN1", Lastname = "LN1" };
            var persons = new[]
            {
                new Person { Firstname = "FN2", Lastname = "LN2" },
                new Person { Firstname = "FN3", Lastname = "LN3" }
            };

            await repository.InsertAsync(person);
            await repository.InsertAsync(persons);
            await dbContext.SaveChangesAsync();

            var count = await repository.CountAsync();

            var expectedCount = DbUtilities.PersonSeed.Count + persons.Length + 1;
            Assert.Equal(expectedCount, count);
            Assert.NotEqual(default, person.Id);
            Assert.NotEqual(default, persons[0].Id);
            Assert.NotEqual(default, persons[1].Id);
        }

        [Fact]
        public async Task Update()
        {
            var testname = "TEST";

            var entities1 = (await repository.GetAllAsync(disableTracking: false)).ToList();
            Assert.DoesNotContain(entities1, x => x.Firstname == testname);

            entities1[0].Firstname = testname;
            await repository.UpdateAsync(entities1[0]);
            await dbContext.SaveChangesAsync();

            var count = await repository.CountAsync(x => x.Firstname == testname);
            Assert.Equal(1, count);

            entities1.ForEach(x => x.Firstname = testname);
            await repository.UpdateAsync(entities1);
            await dbContext.SaveChangesAsync();

            var entities2 = (await repository.GetAllAsync()).ToList();
            Assert.DoesNotContain(entities2, x => x.Firstname != testname);
        }

        [Fact]
        public async Task Delete()
        {
            await repository.DeleteAsync(1.ToGuid());
            await dbContext.SaveChangesAsync();
            var count1 = await repository.CountAsync();

            await repository.DeleteAsync(DbUtilities.PersonSeed.Last());
            await dbContext.SaveChangesAsync();
            var count2 = await repository.CountAsync();

            Assert.Equal(1, count1);
            Assert.Equal(0, count2);
        }
    }
}
