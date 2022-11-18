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

        protected T Load<T>(ref T navigationField, [CallerMemberName] string navigationName = null!)
        {
            lazyLoader?.Invoke(this, navigationName);
            return navigationField;
        }
    }

    public class LazyEntity : LazyEntity<Guid>
    {
        protected LazyEntity()
        {
        }

        protected LazyEntity(Action<object, string> lazyLoader)
            : base(lazyLoader)
        {
        }
    }
}
