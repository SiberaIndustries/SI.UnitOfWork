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
        [Fact]
        public void ResolveDependenciesSuccessfully()
        {
            var services = new ServiceCollection();

            services.AddTransient<PersonRepository>();
            services.AddTransient<DefaultRepository>();
            services.AddDbContext<EFContext>(x => x.UseInMemoryDatabase("InMemory1"));
            services.AddDbContext<IDbContext, EFContext>(x => x.UseInMemoryDatabase("InMemory1"));
            services.AddDbContext<CustomContext>(x => x.UseInMemoryDatabase("InMemory2"));
            services.AddEFUnitOfWork();
            services.AddEFUnitOfWork<CustomContext>();

            var provider = services.BuildServiceProvider();

            var unitOfWork1 = provider.GetService<IUnitOfWork>();
            var repo11 = unitOfWork1.GetRepository<Person>();
            var repo12 = unitOfWork1.GetRepository<Person, PersonRepository>();
            var repo13 = unitOfWork1.GetRepository<Person, DefaultRepository>();

            var unitOfWork2 = provider.GetService<IUnitOfWork<CustomContext>>();
            var repo21 = unitOfWork2.GetRepository<Person>();
            var repo22 = unitOfWork2.GetRepository<Person, PersonRepository>();
            var repo23 = unitOfWork2.GetRepository<Person, DefaultRepository>();
        }
    }
}
