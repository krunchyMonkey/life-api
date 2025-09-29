using System.ComponentModel.DataAnnotations;

namespace Life.Infrastructure.Entities
{
    /// <summary>
    /// Entity for persisting GameSession aggregates to the database
    /// Flattens SessionSettings as individual properties for better queryability
    /// </summary>
    public sealed class GameSessionEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        public int Width { get; set; }

        [Required] 
        public int Height { get; set; }

        [Required]
        public int CurrentGeneration { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // SessionStatus enum as string

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime LastModified { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = "";

        // Flattened SessionSettings properties for better queryability
        public bool AutoSnapshotEnabled { get; set; } = true;

        public int AutoSnapshotInterval { get; set; } = 10;

        public int MaxSnapshots { get; set; } = 50;

        /// <summary>
        /// Session timeout stored as total milliseconds for database compatibility
        /// </summary>
        public long SessionTimeoutMs { get; set; } = TimeSpan.FromHours(24).Ticks / TimeSpan.TicksPerMillisecond;

        public bool AllowConcurrentAccess { get; set; } = false;

        // Current board reference - reuses existing BoardEntity infrastructure
        [MaxLength(32)]
        public string? CurrentBoardId { get; set; }

        // Navigation properties
        public BoardEntity? CurrentBoard { get; set; }
        
        public ICollection<SessionSnapshotEntity> Snapshots { get; set; } = new List<SessionSnapshotEntity>();

        /// <summary>
        /// Helper property to get/set SessionTimeout as TimeSpan
        /// </summary>
        public TimeSpan SessionTimeout
        {
            get => TimeSpan.FromMilliseconds(SessionTimeoutMs);
            set => SessionTimeoutMs = (long)value.TotalMilliseconds;
        }
    }
}