using System;

namespace SI.UnitOfWork.Common
{
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
