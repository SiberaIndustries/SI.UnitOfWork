namespace SI.UnitOfWork.Interfaces
{
    public interface IAuditableEntity : IEntity
    {
        const string CreatedByPropertyName = "_CreatedBy";
        const string CreatedPropertyName = "_CreatedAt";
        const string LastModifiedByPropertyName = "_LastModifiedBy";
        const string LastModifiedPropertyName = "_LastModifiedAt";
    }
}
