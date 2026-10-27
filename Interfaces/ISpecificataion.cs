namespace AsyncBatchProcessor.Interfaces
{
    public interface ISpecificataion<T>
    {
        public bool IsSatisfiedBy(T entity);
    }
}
