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
            var result1 = await repository.FindAsync(new object[] { id.ToGuid() });
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
        public async Task GetAllAsyncEnumerable()
        {
            var result = repository.GetAllAsyncEnumerable();

            var count = 0;
            await foreach (var x in result)
            {
                Assert.Equal(++count, x.Id.ToInt());
            }
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
            var resultMin1 = await repository.MinAsync(x => x.Birthday);
            var resultMin2 = await repository.MinAsync(x => x.Id, x => x.Birthday < DateTime.Now);

            var resultMax1 = await repository.MaxAsync(x => x.Birthday);
            var resultMax2 = await repository.MaxAsync(x => x.Id, x => x.Birthday < DateTime.Now);

            var resultAvg1 = await repository.AverageAsync(x => x.Birthday.Ticks);
            var resultAvg2 = await repository.AverageAsync(x => x.Birthday.Ticks, x => x.Birthday < DateTime.Now);
            var resultAvg3 = DbUtilities.PersonSeed.Average(x => x.Birthday.Ticks);

            var resultSum1 = await repository.SumAsync(x => x.Birthday.Ticks);
            var resultSum2 = await repository.SumAsync(x => x.Birthday.Ticks, x => x.Birthday < DateTime.Now);
            var resultSum3 = DbUtilities.PersonSeed.Sum(x => x.Birthday.Ticks);

            Assert.Equal(new DateTime(1988, 1, 1), resultMin1);
            Assert.Equal(1, resultMin2.ToInt());

            Assert.Equal(new DateTime(1990, 6, 6), resultMax1);
            Assert.Equal(2, resultMax2.ToInt());

            Assert.Equal(new decimal(resultAvg3), resultAvg1);
            Assert.Equal(new decimal(resultAvg3), resultAvg2);

            Assert.Equal(new decimal(resultSum3), resultSum1);
            Assert.Equal(new decimal(resultSum3), resultSum2);
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
