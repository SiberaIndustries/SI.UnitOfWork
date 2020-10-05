using SI.UnitOfWork.Tests.SampleData.Entities;
using System;
using Xunit;

namespace SI.UnitOfWork.Tests
{
    public class EntityEqualityTests
    {
        [Fact]
        public void TransientEntities_ShouldNotBeEqual()
        {
            var e1 = new Person();
            var e2 = new Person();

            Assert.NotEqual(e1, e2);
        }

        [Fact]
        public void TransientEntities_ShouldBeEqual()
        {
            var e1 = new Person();
            var e2 = e1;

            Assert.Equal(e1, e2);
            Assert.True(e1 == e2);
        }

        [Fact]
        public void EntitiesWithSameId_ShouldBeEqual()
        {
            var id = Guid.NewGuid();
            var e1 = new Person { Id = id };
            var e2 = new Person { Id = id };

            Assert.Equal(e1, e2);
            Assert.False(e1 == e2);
        }

        [Fact]
        public void EntitiesWithDifferentSameId_ShouldNotBeEqual()
        {
            var e1 = new Person { Id = Guid.NewGuid() };
            var e2 = new Person { Id = Guid.NewGuid() };

            Assert.NotEqual(e1, e2);
        }

        [Fact]
        public void Entity_ShouldNotEqualTransient()
        {
            var e1 = new Person { Id = Guid.NewGuid() };
            var e2 = new Person();

            Assert.NotEqual(e1, e2);
        }

        [Fact]
        public void DifferentEntityTypesWithSameId_ShouldNotEqual()
        {
            var id = Guid.NewGuid();
            var e1 = new Person { Id = id };
            var e2 = new Person { Id = id };

            Assert.False(ReferenceEquals(e1, e2));
        }

        [Fact]
        public void EqualOperator_ShouldWorkCorrectly()
        {
            var id = Guid.NewGuid();
            var e1 = new Person { Id = id };
            var e2 = e1;

            Assert.True(e1 == e2);
        }

        [Fact]
        public void NotEqualOperator_ShouldWorkCorrectly()
        {
            var e1 = new Person { Id = Guid.NewGuid() };
            var e2 = new Person { Id = Guid.NewGuid() };

            Assert.True(e1 != e2);
        }
    }
}
