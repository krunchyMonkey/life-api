using Life.Domain.Common;

namespace Life.Domain.Common
{
    /// <summary>
    /// Base class for aggregate roots that can raise domain events
    /// </summary>
    public abstract class AggregateRoot<T> : Entity<T>
    {
        private readonly List<DomainEvent> _domainEvents = new();

        protected AggregateRoot(T id) : base(id) { }
        protected AggregateRoot() : base() { }

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}