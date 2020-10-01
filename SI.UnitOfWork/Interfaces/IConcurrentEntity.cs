namespace SI.UnitOfWork.Interfaces
{
    public interface IConcurrentEntity<T>
    {
        T ConcurrencyToken { get; set; }
    }
}
