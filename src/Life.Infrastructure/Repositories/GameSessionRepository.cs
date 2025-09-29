using Life.Domain.Aggregates;
using Life.Domain.Repositories;
using Life.Domain.Services;
using Life.Domain.ValueObjects;
using Life.Infrastructure.Data;
using Life.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Life.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for GameSession aggregate
    /// Uses hybrid approach - leverages existing BoardEntity infrastructure for current board state
    /// and new GameSession-specific entities for session data and snapshots
    /// </summary>
    public sealed class GameSessionRepository : IGameSessionRepository
    {
        private readonly LifeDbContext _context;
        private readonly ILogger<GameSessionRepository>? _logger;

        public GameSessionRepository(LifeDbContext context, ILogger<GameSessionRepository>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        #region Basic CRUD Operations

        public async Task<GameSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Retrieving GameSession with ID: {SessionId}", id);

            var entity = await _context.GameSessions
                .Include(s => s.Snapshots.OrderBy(snap => snap.Generation))
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (entity == null)
            {
                _logger?.LogWarning("GameSession not found: {SessionId}", id);
                return null;
            }

            // Get current board state from BoardEntity/SnapshotEntity infrastructure
            Board? currentBoard = null;
            if (!string.IsNullOrEmpty(entity.CurrentBoardId))
            {
                var currentSnapshot = await _context.Snapshots
                    .Where(s => s.BoardId == entity.CurrentBoardId)
                    .OrderByDescending(s => s.Generation)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentSnapshot != null)
                {
                    currentBoard = BoardSerializationService.Deserialize(currentSnapshot.Data);
                }
            }

            _logger?.LogInformation("Successfully retrieved GameSession: {SessionId}", id);
            return MapToAggregate(entity, currentBoard);
        }

        public async Task<GameSession> AddAsync(GameSession session, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Adding new GameSession: {SessionId}", session.Id);

            var entity = MapToEntity(session);
            
            // Create board entity for current state (reuse existing infrastructure)
            var currentBoard = session.GetCurrentBoard();
            var boardId = Guid.NewGuid().ToString("n");
            
            var boardEntity = new BoardEntity
            {
                Id = boardId,
                Width = currentBoard.Width,
                Height = currentBoard.Height,
                CreatedUtc = DateTimeOffset.UtcNow
            };

            var snapshotEntity = new SnapshotEntity
            {
                BoardId = boardId,
                Generation = session.CurrentGeneration,
                Data = BoardSerializationService.Serialize(currentBoard)
            };

            entity.CurrentBoardId = boardId;

            // Add session snapshots
            var sessionSnapshots = session.GetSnapshots().Select(MapSnapshotToEntity).ToList();
            
            _context.GameSessions.Add(entity);
            _context.Boards.Add(boardEntity);
            _context.Snapshots.Add(snapshotEntity);
            _context.SessionSnapshots.AddRange(sessionSnapshots);

            await _context.SaveChangesAsync(cancellationToken);

            _logger?.LogInformation("Successfully added GameSession: {SessionId}", session.Id);
            return session;
        }

        public void Update(GameSession session)
        {
            _logger?.LogInformation("Updating GameSession: {SessionId}", session.Id);

            var entity = _context.GameSessions
                .Include(s => s.Snapshots)
                .FirstOrDefault(s => s.Id == session.Id);

            if (entity == null)
            {
                throw new InvalidOperationException($"GameSession {session.Id} not found for update");
            }

            // Update entity properties
            UpdateEntityFromAggregate(entity, session);

            // Update current board state
            var currentBoard = session.GetCurrentBoard();
            if (!string.IsNullOrEmpty(entity.CurrentBoardId))
            {
                // Add new snapshot for current generation
                var newSnapshot = new SnapshotEntity
                {
                    BoardId = entity.CurrentBoardId,
                    Generation = session.CurrentGeneration,
                    Data = BoardSerializationService.Serialize(currentBoard)
                };
                _context.Snapshots.Add(newSnapshot);
            }

            // Handle session snapshots - simple approach: remove all and re-add
            // TODO: Optimize this with proper change tracking
            _context.SessionSnapshots.RemoveRange(entity.Snapshots);
            var updatedSnapshots = session.GetSnapshots().Select(MapSnapshotToEntity).ToList();
            _context.SessionSnapshots.AddRange(updatedSnapshots);

            _logger?.LogInformation("Updated GameSession: {SessionId}", session.Id);
        }

        public void Remove(GameSession session)
        {
            _logger?.LogInformation("Removing GameSession: {SessionId}", session.Id);

            var entity = _context.GameSessions.Find(session.Id);
            if (entity != null)
            {
                _context.GameSessions.Remove(entity);
                // SessionSnapshots will be cascade deleted
                // Current board and its snapshots remain for potential reuse
            }

            _logger?.LogInformation("Removed GameSession: {SessionId}", session.Id);
        }

        #endregion

        #region Query Operations

        public async Task<IReadOnlyList<GameSession>> GetByCreatorAsync(string createdBy, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Retrieving GameSessions for creator: {CreatedBy}", createdBy);

            var entities = await _context.GameSessions
                .Where(s => s.CreatedBy == createdBy)
                .OrderByDescending(s => s.LastModified)
                .ToListAsync(cancellationToken);

            var sessions = new List<GameSession>();
            foreach (var entity in entities)
            {
                var session = await GetByIdAsync(entity.Id, cancellationToken);
                if (session != null)
                {
                    sessions.Add(session);
                }
            }

            _logger?.LogInformation("Retrieved {Count} GameSessions for creator: {CreatedBy}", sessions.Count, createdBy);
            return sessions.AsReadOnly();
        }

        public async Task<IReadOnlyList<GameSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
        {
            return await GetSessionsByStatusAsync(SessionStatus.Active, cancellationToken);
        }

        public async Task<IReadOnlyList<GameSession>> GetSessionsByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Retrieving GameSessions with status: {Status}", status);

            var statusString = status.ToString();
            var entities = await _context.GameSessions
                .Where(s => s.Status == statusString)
                .OrderByDescending(s => s.LastModified)
                .ToListAsync(cancellationToken);

            var sessions = new List<GameSession>();
            foreach (var entity in entities)
            {
                var session = await GetByIdAsync(entity.Id, cancellationToken);
                if (session != null)
                {
                    sessions.Add(session);
                }
            }

            _logger?.LogInformation("Retrieved {Count} GameSessions with status: {Status}", sessions.Count, status);
            return sessions.AsReadOnly();
        }

        public async Task<IReadOnlyList<GameSession>> GetRecentSessionsAsync(int count = 10, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Retrieving {Count} most recent GameSessions", count);

            var entities = await _context.GameSessions
                .OrderByDescending(s => s.LastModified)
                .Take(count)
                .ToListAsync(cancellationToken);

            var sessions = new List<GameSession>();
            foreach (var entity in entities)
            {
                var session = await GetByIdAsync(entity.Id, cancellationToken);
                if (session != null)
                {
                    sessions.Add(session);
                }
            }

            _logger?.LogInformation("Retrieved {Count} recent GameSessions", sessions.Count);
            return sessions.AsReadOnly();
        }

        public async Task<IReadOnlyList<GameSession>> SearchSessionsAsync(
            string? namePattern = null,
            string? createdBy = null,
            SessionStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Searching GameSessions with filters");

            var query = _context.GameSessions.AsQueryable();

            if (!string.IsNullOrEmpty(namePattern))
                query = query.Where(s => s.Name.Contains(namePattern));

            if (!string.IsNullOrEmpty(createdBy))
                query = query.Where(s => s.CreatedBy == createdBy);

            if (status.HasValue)
                query = query.Where(s => s.Status == status.Value.ToString());

            if (createdAfter.HasValue)
                query = query.Where(s => s.CreatedAt >= createdAfter.Value);

            if (createdBefore.HasValue)
                query = query.Where(s => s.CreatedAt <= createdBefore.Value);

            var entities = await query
                .OrderByDescending(s => s.LastModified)
                .ToListAsync(cancellationToken);

            var sessions = new List<GameSession>();
            foreach (var entity in entities)
            {
                var session = await GetByIdAsync(entity.Id, cancellationToken);
                if (session != null)
                {
                    sessions.Add(session);
                }
            }

            _logger?.LogInformation("Found {Count} GameSessions matching search criteria", sessions.Count);
            return sessions.AsReadOnly();
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.GameSessions.AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<int> CountActiveSessionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.GameSessions
                .CountAsync(s => s.Status == SessionStatus.Active.ToString(), cancellationToken);
        }

        #endregion

        #region Snapshot Operations

        public async Task<SessionSnapshot?> GetSnapshotAsync(Guid sessionId, Guid snapshotId, CancellationToken cancellationToken = default)
        {
            var entity = await _context.SessionSnapshots
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.Id == snapshotId, cancellationToken);

            return entity != null ? MapSnapshotToValueObject(entity) : null;
        }

        public async Task<IReadOnlyList<SessionSnapshot>> GetSessionSnapshotsAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            var entities = await _context.SessionSnapshots
                .Where(s => s.SessionId == sessionId)
                .OrderBy(s => s.Generation)
                .ToListAsync(cancellationToken);

            return entities.Select(MapSnapshotToValueObject).ToList().AsReadOnly();
        }

        #endregion

        #region Cleanup Operations

        public async Task<int> ArchiveOldSessionsAsync(TimeSpan olderThan, CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow - olderThan;
            
            var sessionsToArchive = await _context.GameSessions
                .Where(s => s.LastModified < cutoffDate && s.Status != SessionStatus.Archived.ToString())
                .ToListAsync(cancellationToken);

            foreach (var session in sessionsToArchive)
            {
                session.Status = SessionStatus.Archived.ToString();
                session.LastModified = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            
            _logger?.LogInformation("Archived {Count} old GameSessions", sessionsToArchive.Count);
            return sessionsToArchive.Count;
        }

        public async Task<int> DeleteArchivedSessionsAsync(TimeSpan olderThan, CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow - olderThan;
            
            var sessionsToDelete = await _context.GameSessions
                .Where(s => s.LastModified < cutoffDate && s.Status == SessionStatus.Archived.ToString())
                .ToListAsync(cancellationToken);

            _context.GameSessions.RemoveRange(sessionsToDelete);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger?.LogInformation("Deleted {Count} archived GameSessions", sessionsToDelete.Count);
            return sessionsToDelete.Count;
        }

        #endregion

        #region Private Mapping Methods

        private static GameSessionEntity MapToEntity(GameSession session)
        {
            var settings = session.GetSettings();
            
            return new GameSessionEntity
            {
                Id = session.Id,
                Name = session.Name,
                Width = session.Dimensions.Width,
                Height = session.Dimensions.Height,
                CurrentGeneration = session.CurrentGeneration,
                Status = session.Status.ToString(),
                CreatedAt = session.CreatedAt,
                LastModified = session.LastModified,
                CreatedBy = session.CreatedBy,
                AutoSnapshotEnabled = settings.AutoSnapshotEnabled,
                AutoSnapshotInterval = settings.AutoSnapshotInterval,
                MaxSnapshots = settings.MaxSnapshots,
                SessionTimeoutMs = (long)settings.SessionTimeout.TotalMilliseconds,
                AllowConcurrentAccess = settings.AllowConcurrentAccess
            };
        }

        private static void UpdateEntityFromAggregate(GameSessionEntity entity, GameSession session)
        {
            entity.Name = session.Name;
            entity.CurrentGeneration = session.CurrentGeneration;
            entity.Status = session.Status.ToString();
            entity.LastModified = session.LastModified;
            // Settings updates would go here if we had access to them
        }

        private static GameSession MapToAggregate(GameSessionEntity entity, Board? currentBoard)
        {
            var settings = new SessionSettings
            {
                AutoSnapshotEnabled = entity.AutoSnapshotEnabled,
                AutoSnapshotInterval = entity.AutoSnapshotInterval,
                MaxSnapshots = entity.MaxSnapshots,
                SessionTimeout = TimeSpan.FromMilliseconds(entity.SessionTimeoutMs),
                AllowConcurrentAccess = entity.AllowConcurrentAccess
            };

            var dimensions = new BoardDimensions(entity.Width, entity.Height);
            var snapshots = entity.Snapshots
                .Select(s => new SessionSnapshot(
                    s.Id, 
                    s.SessionId, 
                    s.Generation, 
                    s.BoardData, 
                    s.Description ?? string.Empty, 
                    s.CreatedAt, 
                    s.IsAutoGenerated))
                .ToList();

            return GameSession.Reconstitute(
                entity.Id,
                entity.Name,
                dimensions,
                entity.CurrentGeneration,
                Enum.Parse<SessionStatus>(entity.Status),
                entity.CreatedAt,
                entity.LastModified,
                entity.CreatedBy,
                currentBoard,
                settings,
                snapshots
            );
        }

        private static SessionSnapshotEntity MapSnapshotToEntity(SessionSnapshot snapshot)
        {
            return new SessionSnapshotEntity
            {
                Id = snapshot.Id,
                SessionId = snapshot.SessionId,
                Generation = snapshot.Generation,
                BoardData = snapshot.BoardData,
                Description = snapshot.Description,
                CreatedAt = snapshot.CreatedAt,
                IsAutoGenerated = snapshot.IsAutoGenerated
            };
        }

        private static SessionSnapshot MapSnapshotToValueObject(SessionSnapshotEntity entity)
        {
            return new SessionSnapshot(
                entity.Id,
                entity.SessionId,
                entity.Generation,
                entity.BoardData,
                entity.Description ?? string.Empty,
                entity.CreatedAt,
                entity.IsAutoGenerated
            );
        }

        #endregion
    }
}