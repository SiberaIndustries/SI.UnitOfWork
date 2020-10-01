using SI.UnitOfWork.Interfaces;
using System;

namespace SI.UnitOfWork.Common
{
    public abstract class BaseEntity<TKey> : IEquatable<LazyEntity<TKey>>, IEntity<TKey>
        where TKey : notnull
    {
        /// <inheritdoc cref="IEntity" />
        public TKey Id { get; set; } = default!;

        /// <inheritdoc cref="IEquatable{T}" />
        public virtual bool Equals(LazyEntity<TKey>? other)
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

        /// <inheritdoc cref="object" />
        public override bool Equals(object? obj) =>
            Equals(obj as LazyEntity<TKey>);

        /// <inheritdoc cref="object" />
        public override int GetHashCode() =>
            Id.GetHashCode();
    }
}
