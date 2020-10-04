using SI.UnitOfWork.Common;
using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork.Tests.SampleData.Entities
{
    public class Person : BaseEntity, ISoftDeleteEntity, IMultiTenantEntity
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }
    }
}
