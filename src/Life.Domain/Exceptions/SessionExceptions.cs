namespace Life.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a snapshot is not found
    /// </summary>
    public class SnapshotNotFoundException : DomainException
    {
        public SnapshotNotFoundException(string message) : base(message)
        {
        }

        public SnapshotNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Base exception for domain-specific errors
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message)
        {
        }

        protected DomainException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}