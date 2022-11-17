using SI.UnitOfWork.Common;
using SI.UnitOfWork.Interfaces;
using System;

namespace SI.UnitOfWork.Tests.SampleData.Entities
{
    public class Comment : LazyEntity, ISoftDeleteEntity, IAuditableEntity
    {
        private Person person;

        public Comment()
        {
        }

        protected Comment(Action<object, string> lazyLoader)
            : base(lazyLoader)
        {
        }

        public string Title { get; set; }

        public string Text { get; set; }

        public Person Person
        {
            get => Load(ref person);
            set => person = value;
        }
    }
}
