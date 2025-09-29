using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Repositories
{
    /// <summary>
    /// Repository interface for GameSession aggregate
    /// </summary>
    public interface IGameSessionRepository
    {
        // Basic CRUD operations
        Task<GameSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GameSession> AddAsync(GameSession session, CancellationToken cancellationToken = default);
        void Update(GameSession session);
        void Remove(GameSession session);

        // Query operations
        Task<IReadOnlyList<GameSession>> GetByCreatorAsync(string createdBy, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GameSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GameSession>> GetSessionsByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GameSession>> GetRecentSessionsAsync(int count = 10, CancellationToken cancellationToken = default);
        
        // Advanced queries
        Task<IReadOnlyList<GameSession>> SearchSessionsAsync(
            string? namePattern = null,
            string? createdBy = null,
            SessionStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> CountActiveSessionsAsync(CancellationToken cancellationToken = default);

        // Snapshot operations
        Task<SessionSnapshot?> GetSnapshotAsync(Guid sessionId, Guid snapshotId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SessionSnapshot>> GetSessionSnapshotsAsync(Guid sessionId, CancellationToken cancellationToken = default);
        
        // Cleanup operations
        Task<int> ArchiveOldSessionsAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);
        Task<int> DeleteArchivedSessionsAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);
    }
}