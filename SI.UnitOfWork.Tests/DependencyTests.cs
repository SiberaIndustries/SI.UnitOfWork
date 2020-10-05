using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SI.UnitOfWork.Interfaces;
using SI.UnitOfWork.Tests.SampleData.Contexts;
using SI.UnitOfWork.Tests.SampleData.Entities;
using SI.UnitOfWork.Tests.SampleData.Repositories;
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
        public void ResolveDependenciesSuccessfully()
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
            var unitOfWork1 = provider.GetService<IUnitOfWork>();
            var repo11 = unitOfWork1.GetRepository<Person>();
            var repo12 = unitOfWork1.GetRepository<Person, IPersonRepository>();
            var repo13 = unitOfWork1.GetRepository<Person, DefaultRepository>();

            var unitOfWork2 = provider.GetService<IUnitOfWork<CustomEFContext>>();
            var repo21 = unitOfWork2.GetRepository<Person>();
            var repo22 = unitOfWork2.GetRepository<Person, IPersonRepository>();
            var repo23 = unitOfWork2.GetRepository<Person, DefaultRepository>();
        }

        [Fact]
        public void ResolveDependenciesSuccessfully2()
        {
            // Arrange
            serviceCollection.AddScoped<IDbContext, CustomContext>();
            serviceCollection.AddUnitOfWork();
            serviceCollection.AddUnitOfWork<CustomContext>();
            var provider = serviceCollection.BuildServiceProvider();

            // Act / Assert
            var unitOfWork1 = provider.GetService<IUnitOfWork>();
            var repo11 = unitOfWork1.GetRepository<Person>();
            var repo13 = unitOfWork1.GetRepository<Person, DefaultRepository>();

            var unitOfWork2 = provider.GetService<IUnitOfWork<CustomContext>>();
            var repo21 = unitOfWork2.GetRepository<Person>();
            var repo23 = unitOfWork2.GetRepository<Person, DefaultRepository>();
        }
    }
}
