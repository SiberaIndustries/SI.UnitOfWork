using System;

namespace SI.UnitOfWork.Interfaces
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<Guid>
    {
    }
}
