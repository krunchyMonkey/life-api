namespace Life.Domain.Specifications
{
    /// <summary>
    /// Base specification interface for domain rules
    /// </summary>
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);
        ISpecification<T> And(ISpecification<T> specification);
        ISpecification<T> Or(ISpecification<T> specification);
        ISpecification<T> Not();
    }
}