using System;
using System.Runtime.CompilerServices;

namespace SI.UnitOfWork.Common
{
    public abstract class LazyEntity<TKey> : BaseEntity<TKey>
        where TKey : notnull
    {
        private readonly Action<object, string>? lazyLoader;

        protected LazyEntity()
        {
        }

        protected LazyEntity(Action<object, string> lazyLoader)
        {
            this.lazyLoader = lazyLoader;
        }

        /// <summary>
        /// Invokes the lazy load command of a navigation property.
        /// </summary>
        /// <typeparam name="T">Type of the navigation field.</typeparam>
        /// <param name="navigationField">Navigation field.</param>
        /// <param name="navigationName">Name of the navigation property.</param>
        /// <returns>The navigation field.</returns>
        protected T Load<T>(ref T navigationField, [CallerMemberName] string navigationName = null!)
        {
            lazyLoader?.Invoke(this, navigationName);
            return navigationField;
        }
    }
}
