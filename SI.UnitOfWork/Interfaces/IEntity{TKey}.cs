namespace SI.UnitOfWork.Interfaces
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
