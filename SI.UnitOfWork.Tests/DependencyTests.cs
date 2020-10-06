using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SI.UnitOfWork.Interfaces;
using SI.UnitOfWork.Tests.SampleData.Contexts;
using SI.UnitOfWork.Tests.SampleData.Entities;
using SI.UnitOfWork.Tests.SampleData.Repositories;
using System.Linq;
using Xunit;

namespace SI.UnitOfWork.Tests
{
    public class DependencyTests
    {
        private readonly IServiceCollection serviceCollection;

        public DependencyTests()
        {
            serviceCollection = new ServiceCollection();
        }

        [Fact]
        public void ResolveEFDependenciesSuccessfully()
        {
            // Arrange
            serviceCollection.AddScoped<IPersonRepository, PersonRepository>();
            serviceCollection.AddScoped<DefaultRepository>();
            serviceCollection.AddDbContext<EFContext>(x => x.UseInMemoryDatabase("InMemory1"));
            serviceCollection.AddDbContext<CustomEFContext>(x => x.UseInMemoryDatabase("InMemory2"));
            serviceCollection.AddEFUnitOfWork();
            serviceCollection.AddEFUnitOfWork<CustomEFContext>();
            var provider = serviceCollection.BuildServiceProvider();

            // Act / Assert
            var unitOfWorks = new IUnitOfWork[]
            {
                provider.GetService<IUnitOfWork>(),
                provider.GetService<IUnitOfWork<CustomEFContext>>(),
                provider.GetService<IUnitOfWorkFactory>().GetUnitOfWork<CustomEFContext>(),
            };

            Assert.DoesNotContain(unitOfWorks, x => x == null);
            foreach (var unitOfWork in unitOfWorks)
            {
                var repo1 = unitOfWork.GetRepository<Person>();
                var repo3 = unitOfWork.GetRepository<Person, DefaultRepository>();
                var repo2 = unitOfWork.GetRepository<Person, IPersonRepository>();
                Assert.NotNull(repo1);
                Assert.NotNull(repo2);
                Assert.NotNull(repo3);
            }

            unitOfWorks.ToList().ForEach(x => x.Dispose());
        }

        [Fact]
        public void ResolveDependenciesSuccessfully()
        {
            // Arrange
            serviceCollection.AddScoped<IRepository<Person>, DefaultRepository>();
            serviceCollection.AddScoped<DefaultRepository>();
            serviceCollection.AddScoped<IDbContext, CustomContext>();
            serviceCollection.AddUnitOfWork();
            serviceCollection.AddUnitOfWork<CustomContext>();
            var provider = serviceCollection.BuildServiceProvider();

            // Act / Assert
            var unitOfWorks = new IUnitOfWork[]
            {
                provider.GetService<IUnitOfWork>(),
                provider.GetService<IUnitOfWork<CustomContext>>(),
                provider.GetService<IUnitOfWorkFactory>().GetUnitOfWork<CustomContext>(),
            };

            Assert.DoesNotContain(unitOfWorks, x => x == null);
            foreach (var unitOfWork in unitOfWorks)
            {
                var repo1 = unitOfWork.GetRepository<Person>();
                var repo2 = unitOfWork.GetRepository<Person, DefaultRepository>();
                Assert.NotNull(repo1);
                Assert.NotNull(repo2);
            }

            unitOfWorks.ToList().ForEach(x => x.Dispose());
        }

        [Fact]
        public void AddEFUnitOfWorkWithoutUnnessecaryDuplications()
        {
            // Act / Assert
            serviceCollection.AddEFUnitOfWork();
            Assert.Equal(6, serviceCollection.Count);

            serviceCollection.AddEFUnitOfWork<EFContext>();
            Assert.Equal(8, serviceCollection.Count);

            serviceCollection.AddEFUnitOfWork<EFContext>();
            Assert.Equal(8, serviceCollection.Count);

            serviceCollection.AddEFUnitOfWork<EFContext, EFContext>();
            Assert.Equal(8, serviceCollection.Count);

            serviceCollection.AddEFUnitOfWork<EFContext, EFContext, EFContext>();
            Assert.Equal(8, serviceCollection.Count);

            serviceCollection.AddEFUnitOfWork<EFContext, CustomEFContext>();
            Assert.Equal(10, serviceCollection.Count);
        }

        [Fact]
        public void AddUnitOfWorkWithoutUnnessecaryDuplications()
        {
            // Act / Assert
            serviceCollection.AddUnitOfWork();
            Assert.Equal(3, serviceCollection.Count);

            serviceCollection.AddUnitOfWork<CustomContext>();
            Assert.Equal(6, serviceCollection.Count);

            serviceCollection.AddUnitOfWork<CustomContext>();
            Assert.Equal(6, serviceCollection.Count);

            serviceCollection.AddUnitOfWork<CustomContext, CustomContext>();
            Assert.Equal(6, serviceCollection.Count);

            serviceCollection.AddUnitOfWork<CustomContext, CustomContext, CustomContext>();
            Assert.Equal(6, serviceCollection.Count);
        }
    }
}
