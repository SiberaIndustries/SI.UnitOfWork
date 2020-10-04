using SI.UnitOfWork.Common;

namespace SI.UnitOfWork.Tests.SampleData.Entities
{
    public class Person : BaseEntity
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }
    }
}
