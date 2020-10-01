using System;

namespace SI.UnitOfWork.Interfaces
{
    public interface IIdentityProvider
    {
        Guid Identity { get; }
    }
}
