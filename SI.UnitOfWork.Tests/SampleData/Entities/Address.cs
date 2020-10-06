using SI.UnitOfWork.Interfaces;

namespace SI.UnitOfWork.Tests.SampleData.Entities
{
    public class Address : IOwnedType
    {
        public string Street { get; set; }

        public string Postalcode { get; set; }

        public string City { get; set; }
    }
}
