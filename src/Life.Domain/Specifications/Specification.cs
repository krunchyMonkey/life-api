namespace Life.Domain.Specifications
{
    /// <summary>
    /// Base implementation of the specification pattern
    /// </summary>
    public abstract class Specification<T> : ISpecification<T>
    {
        public abstract bool IsSatisfiedBy(T entity);

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }
    }

    internal class AndSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override bool IsSatisfiedBy(T entity)
        {
            return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
        }
    }

    internal class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override bool IsSatisfiedBy(T entity)
        {
            return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
        }
    }

    internal class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification;

        public NotSpecification(ISpecification<T> specification)
        {
            _specification = specification;
        }

        public override bool IsSatisfiedBy(T entity)
        {
            return !_specification.IsSatisfiedBy(entity);
        }
    }
}