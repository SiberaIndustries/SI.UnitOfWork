using SI.UnitOfWork.Interfaces;
using System;

namespace SI.UnitOfWork.Common
{
    public abstract class BaseEntity<TKey> : IEquatable<BaseEntity<TKey>>, IEntity<TKey>
        where TKey : notnull
    {
        public TKey Id { get; set; } = default!;

        public virtual bool Equals(BaseEntity<TKey>? other)
        {
            if (other == null)
            {   // If parameter is null, return false
                return false;
            }

            if (ReferenceEquals(this, other))
            {   // Optimization for a common success case
                return true;
            }

            if (Id.Equals(default) || other.Id.Equals(default) || !Equals(Id, other.Id))
            {   // Check if IDs has default values or are not equal
                return false;
            }

            var thisType = GetType();
            var objType = other.GetType();
            return thisType.IsAssignableFrom(objType) || objType.IsAssignableFrom(thisType);
        }

        public override bool Equals(object? obj) =>
            Equals(obj as BaseEntity<TKey>);

        public override int GetHashCode() =>
            Id.GetHashCode();
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {
    }
}
