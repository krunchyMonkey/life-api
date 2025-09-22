namespace Life.Domain.Common
{
    /// <summary>
    /// Base class for all domain entities
    /// </summary>
    public abstract class Entity<T> : IEquatable<Entity<T>>
    {
        public T Id { get; protected set; }

        protected Entity(T id)
        {
            Id = id;
        }

        protected Entity()
        {
            Id = default!;
        }

        public bool Equals(Entity<T>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Id, other.Id) && GetType() == other.GetType();
        }

        public override bool Equals(object? obj) => Equals(obj as Entity<T>);

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);

        public static bool operator ==(Entity<T>? left, Entity<T>? right) => Equals(left, right);

        public static bool operator !=(Entity<T>? left, Entity<T>? right) => !Equals(left, right);
    }
}