# SI.UnitOfWork

[![NuGet](https://img.shields.io/nuget/v/SI.UnitOfWork.svg)](https://www.nuget.org/packages/SI.UnitOfWork)
[![NuGet](https://img.shields.io/nuget/v/SI.UnitOfWork.EntityFrameworkCore.svg)](https://www.nuget.org/packages/SI.UnitOfWork.EntityFrameworkCore)
[![.NET Core](https://github.com/SiberaIndustries/SI.UnitOfWork/workflows/.NET%20Core/badge.svg)](https://github.com/SiberaIndustries/SI.UnitOfWork/actions?query=workflow%3A%22.NET+Core%22)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_SI.UnitOfWork&metric=alert_status)](https://sonarcloud.io/dashboard?id=SiberaIndustries_SI.UnitOfWork)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_SI.UnitOfWork&metric=coverage)](https://sonarcloud.io/dashboard?id=SiberaIndustries_SI.UnitOfWork)

## Introduction

Most of the repository pattern libraries for .NET or .NET Core depend on the Entity Framework.
Unlike all these frameworks, the `SI.UnitOfWork` has **no such dependencies** and follows the strict paradigms of the repository pattern.
But if you want, `SI.UnitOfWork.EntityFrameworkCore` provides a complete and ready-to-use library for **SqLite**, **PostgreSQL**, **SQL Server** and the **InMemory** provider of Entity Framework.

---

## Getting Started with `SI.UnitOfWork`

### 1. Install and reference the NuGet package

```cs
Install-Package SI.UnitOfWork
```

### 2. Edit your `ConfigureServices()` method in `Startup.cs`

```cs
public void ConfigureServices(IServiceCollection services)
{
    // ..
    services.AddDbContext<IDbContext, SampleContext>(o => o.UseInMemoryDatabase("mem-db"));
    services.AddUnitOfWork<SampleContext>();
    services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
    services.AddScoped(typeof(ICustomRepository), typeof(CustomRepository));
    // ..
}
```

### 2. Inject and use

```cs
public class SampleClass
{   
    // Opt. 1. Inject a UoW for regular DB read and write operations
    // Opt. 2. Inject a UoW-Factory in case you work with multiple databases
    // Opt. 3. Inject a RepositoryFactory to get read-only repositories
    public SampleClass(
        IUnitOfWork unitOfWork, 
        IUnitOfWorkFactory<SampleContext> unitofWorkFactory,
        IRepositoryFactory<SampleContext> repositoryFactory)    
    {
        IRepository<Person> personRepository;   // Register generic repositories manually!
        ICustomRepository customRepository;     // Register custom repositories manually!


        // Opt. 1
        personRepository = unitOfWork.GetRepository<Person>();
        customRepository = unitOfWork.GetRepository<Person, ICustomRepository>();
        // .. do work
        unitOfWork.SaveChanges();


        // Opt. 2
        using var uow = unitofWorkFactory.GetUnitOfWork();
        personRepository = uow.GetRepository<Person>();
        customRepository = uow.GetRepository<Person, ICustomRepository>();
        // .. do work
        uow.SaveChanges();


        // Opt. 3
        personRepository = repositoryFactory.GetRepository<Person>();
        customRepository = repositoryFactory.GetRepository<Person, ICustomRepository>();
        // No SaveChanges available!
    }
}
```

---

## Getting Started with `SI.UnitOfWork.EntityFrameworkCore`

### 1. Install and reference the NuGet package

```cs
Install-Package SI.UnitOfWork.EntityFrameworkCore
```

### 2. Edit your `ConfigureServices()` method in `Startup.cs`

```cs
public void ConfigureServices(IServiceCollection services)
{
    // ..
    services.AddDbContext<EFContext>(o => o.UseInMemoryDatabase("mem-db");
    services.AddEFUnitOfWork();
    services.AddScoped(typeof(ICustomRepository), typeof(CustomRepository));
    // ..
}
```

### 2. Inject and use

```cs
public class SampleClass
{
    public SampleClass(IUnitOfWork unitOfWork)
    {
        IRepository<Person> personRepository;   // Generic repositories are automatically registered!
        ICustomRepository customRepository;     // Register custom repositories manually!


        personRepository = unitOfWork.GetRepository<Person>();
        customRepository = unitOfWork.GetRepository<Person, ICustomRepository>();
        // .. do work
        unitOfWork.SaveChanges();
    }
}
```

## Open Source License Acknowledgements and Third-Party Copyrights

- Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
