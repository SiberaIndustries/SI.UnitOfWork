using Microsoft.EntityFrameworkCore;
using SI.UnitOfWork.Tests.SampleData.Entities;
using System;
using System.Collections.Generic;

namespace SI.UnitOfWork.Tests.Utilities
{
    public static class DbUtilities
    {
        public static readonly IList<Person> PersonSeed = new Person[]
        {
            new Person
            {
                Id = 1.ToGuid(),
                Firstname = "Bob",
                Lastname = "Smith",
                Birthday = new DateTime(1988, 1, 1),
                Address = new Address
                {
                    Street = "Samplestreet 12",
                    Postalcode = "1234",
                    City = "SampleCity"
                }
            },
            new Person
            {
                Id = 2.ToGuid(),
                Firstname = "Alice",
                Lastname = "Smith",
                Birthday = new DateTime(1990, 6, 6)
            }
        };

        public static readonly IList<Comment> CommentSeed = new Comment[]
        {
            new Comment
            {
                 Id = 1.ToGuid(),
                 Title = "Hello",
                 Text = "World",
                 Person = PersonSeed[0]
            },
            new Comment
            {
                 Id = 2.ToGuid(),
                 Title = "Awesome",
                 Text = "This is awesome",
                 Person = PersonSeed[1]
            },
            new Comment
            {
                 Id = 3.ToGuid(),
                 Title = "Disagreed",
                 Text = "I don't agree!",
                 Person = PersonSeed[1]
            }
        };

        public static void SeedDatabase(this DbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            dbContext.AddRange(PersonSeed);
            dbContext.AddRange(CommentSeed);
            dbContext.SaveChanges();
        }
    }
}
