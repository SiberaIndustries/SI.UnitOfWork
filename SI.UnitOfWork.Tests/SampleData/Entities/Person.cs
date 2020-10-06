using SI.UnitOfWork.Common;
using SI.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;

namespace SI.UnitOfWork.Tests.SampleData.Entities
{
    public class Person : LazyEntity, ISoftDeleteEntity, IMultiTenantEntity
    {
        private Address address;
        private ICollection<Comment> comments = new List<Comment>();

        public Person()
        {
        }

        protected Person(Action<object, string> lazyLoader)
            : base(lazyLoader)
        {
        }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public DateTime Birthday { get; set; }

        public Address Address
        {
            get => Load(ref address);
            set => address = value;
        }

        public ICollection<Comment> Comments
        {
            get => Load(ref comments);
            private set => comments = value;
        }
    }
}
