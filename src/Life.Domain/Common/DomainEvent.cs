namespace Life.Domain.Common
{
    /// <summary>
    /// Base class for all domain events
    /// </summary>
    public abstract record DomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}