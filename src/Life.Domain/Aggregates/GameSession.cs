using System.Collections;
using Life.Domain.Common;
using Life.Domain.ValueObjects;
using Life.Domain.Events;
using Life.Domain.Services;
using Life.Domain.Exceptions;

namespace Life.Domain.Aggregates
{
    /// <summary>
    /// Represents a Game of Life session - manages board evolution, snapshots, and session lifecycle
    /// This aggregate demonstrates sophisticated DDD patterns with complex business logic
    /// </summary>
    public sealed class GameSession : AggregateRoot<Guid>
    {
        #region Static Factory Methods
        
        /// <summary>
        /// Creates a new GameSession from a predefined pattern - delegates to GameSessionService
        /// </summary>
        public static GameSession CreateFromPattern(
            string sessionName,
            string createdBy,
            BoardDimensions dimensions,
            PatternType pattern,
            Position? centerPosition = null,
            SessionSettings? settings = null)
        {
            return GameSessionService.CreateFromPattern(
                sessionName, createdBy, dimensions, pattern, centerPosition, settings);
        }
        
        /// <summary>
        /// Merges two GameSessions - delegates to GameSessionService
        /// </summary>
        public static GameSession MergeSessions(
            GameSession session1,
            GameSession session2,
            string newSessionName,
            string createdBy,
            Position session2Offset,
            SessionSettings? settings = null)
        {
            return GameSessionService.MergeSessions(
                session1, session2, newSessionName, createdBy, session2Offset, settings);
        }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Factory method for reconstituting GameSession from repository data
        /// Used by GameSessionRepository to rebuild aggregates from persistence
        /// </summary>
        public static GameSession Reconstitute(
            Guid id,
            string name,
            BoardDimensions dimensions,
            int currentGeneration,
            SessionStatus status,
            DateTime createdAt,
            DateTime lastModified,
            string createdBy,
            Board? currentBoard,
            SessionSettings settings,
            List<SessionSnapshot> snapshots)
        {
            return new GameSession(
                id, name, dimensions, currentGeneration, status,
                createdAt, lastModified, createdBy, currentBoard,
                settings, snapshots);
        }
        
        #endregion
        
        public string Name { get; private set; }
        public BoardDimensions Dimensions { get; private set; }
        public int CurrentGeneration { get; private set; }
        public SessionStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastModified { get; private set; }
        public string CreatedBy { get; private set; }
        
        private readonly List<SessionSnapshot> _snapshots = new();
        private Board? _currentBoard;
        private SessionSettings _settings;

        // Constructor for new game session
        public GameSession(
            string name, 
            Board initialBoard, 
            string createdBy,
            SessionSettings? settings = null) : base(Guid.NewGuid())
        {
            ValidateSessionCreation(name, initialBoard, createdBy);
            
            Name = name;
            Dimensions = initialBoard.Dimensions;
            CurrentGeneration = 0;
            Status = SessionStatus.Active;
            CreatedAt = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
            CreatedBy = createdBy;
            _currentBoard = initialBoard;
            _settings = settings ?? SessionSettings.Default;
            
            // Create initial snapshot
            CreateSnapshot("Initial board state");
            
            AddDomainEvent(new GameSessionCreated(Id, Name, CreatedBy, Dimensions));
        }

        // Private constructor for reconstitution from repository
        private GameSession(
            Guid id,
            string name,
            BoardDimensions dimensions,
            int currentGeneration,
            SessionStatus status,
            DateTime createdAt,
            DateTime lastModified,
            string createdBy,
            Board? currentBoard,
            SessionSettings settings,
            List<SessionSnapshot> snapshots) : base(id)
        {
            Name = name;
            Dimensions = dimensions;
            CurrentGeneration = currentGeneration;
            Status = status;
            CreatedAt = createdAt;
            LastModified = lastModified;
            CreatedBy = createdBy;
            _currentBoard = currentBoard;
            _settings = settings;
            _snapshots.AddRange(snapshots);
        }

        #region Core Business Operations

        /// <summary>
        /// Advances the game to the next generation
        /// </summary>
        public Board AdvanceGeneration()
        {
            ValidateSessionActive();
            
            if (_currentBoard == null)
                throw new InvalidOperationException("No current board state available");
            
            var previousGeneration = CurrentGeneration;
            var nextBoard = _currentBoard.NextGeneration();
            
            _currentBoard = nextBoard;
            CurrentGeneration++;
            LastModified = DateTime.UtcNow;
            
            // Auto-snapshot based on settings
            if (_settings.AutoSnapshotEnabled && ShouldCreateAutoSnapshot())
            {
                CreateAutoSnapshot($"Auto-snapshot at generation {CurrentGeneration}");
            }
            
            // Check for completion conditions
            CheckCompletionConditions(nextBoard);
            
            AddDomainEvent(new GameSessionAdvanced(Id, previousGeneration, CurrentGeneration));
            
            return _currentBoard;
        }

        /// <summary>
        /// Advances multiple generations at once - delegates to Board's AdvanceGenerations method
        /// </summary>
        public Board AdvanceGenerations(int count)
        {
            ValidateSessionActive();
            
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Generation count must be positive");
                
            if (_currentBoard == null)
                throw new InvalidOperationException("No current board state available");
            
            var startGeneration = CurrentGeneration;
            
            // Delegate to Board's existing AdvanceGenerations method
            _currentBoard = _currentBoard.AdvanceGenerations(count);
            CurrentGeneration += count;
            LastModified = DateTime.UtcNow;
            
            // Create manual snapshot after bulk advancement
            CreateSnapshot($"Bulk advancement: {count} generations");
            
            CheckCompletionConditions(_currentBoard);
            
            AddDomainEvent(new GameSessionBulkAdvanced(Id, startGeneration, CurrentGeneration, count));
            
            return _currentBoard;
        }

        /// <summary>
        /// Runs simulation until final state is reached
        /// </summary>
        public FinalResult RunToCompletion(int maxIterations = 1000, TimeSpan? maxTime = null)
        {
            ValidateSessionActive();
            
            if (_currentBoard == null)
                throw new InvalidOperationException("No current board state available");
                
            var timeout = maxTime ?? TimeSpan.FromMinutes(5);
            var startGeneration = CurrentGeneration;
            
            var result = _currentBoard.DetectFinalState(maxIterations, timeout);
            
            _currentBoard = result.Board;
            CurrentGeneration += result.Iterations;
            LastModified = DateTime.UtcNow;
            
            // Create final snapshot before marking as completed
            if (result.Stable || result.Cyclic)
            {
                CreateSnapshot($"Final state: {(result.Stable ? "Stable" : $"Cyclic (period {result.CycleLength})")}");
                Status = SessionStatus.Completed;
            }
            
            AddDomainEvent(new GameSessionCompleted(Id, startGeneration, CurrentGeneration, result.Stable, result.Cyclic));
            
            return result;
        }

        #endregion

        #region Snapshot Management

        /// <summary>
        /// Creates a manual snapshot of the current game state
        /// </summary>
        public SessionSnapshot CreateSnapshot(string? description = null)
        {
            ValidateSessionActive();
            
            if (_currentBoard == null)
                throw new InvalidOperationException("No current board state to snapshot");
            
            var snapshot = new SessionSnapshot(
                id: Guid.NewGuid(),
                sessionId: Id,
                generation: CurrentGeneration,
                boardData: SerializeBoardState(_currentBoard),
                description: description ?? $"Snapshot at generation {CurrentGeneration}",
                createdAt: DateTime.UtcNow,
                isAutoGenerated: false
            );
            
            _snapshots.Add(snapshot);
            LastModified = DateTime.UtcNow;
            
            // Apply retention policy
            ApplySnapshotRetentionPolicy();
            
            AddDomainEvent(new SessionSnapshotCreated(Id, snapshot.Id, CurrentGeneration, description));
            
            return snapshot;
        }

        /// <summary>
        /// Creates an auto-generated snapshot of the current game state
        /// </summary>
        private SessionSnapshot CreateAutoSnapshot(string description)
        {
            if (_currentBoard == null)
                throw new InvalidOperationException("No current board state to snapshot");
            
            var snapshot = new SessionSnapshot(
                id: Guid.NewGuid(),
                sessionId: Id,
                generation: CurrentGeneration,
                boardData: SerializeBoardState(_currentBoard),
                description: description,
                createdAt: DateTime.UtcNow,
                isAutoGenerated: true
            );
            
            _snapshots.Add(snapshot);
            LastModified = DateTime.UtcNow;
            
            // Apply retention policy
            ApplySnapshotRetentionPolicy();
            
            AddDomainEvent(new SessionSnapshotCreated(Id, snapshot.Id, CurrentGeneration, description));
            
            return snapshot;
        }

        /// <summary>
        /// Restores the session to a specific snapshot
        /// </summary>
        public Board RestoreFromSnapshot(Guid snapshotId)
        {
            ValidateSessionActive();
            
            var snapshot = _snapshots.FirstOrDefault(s => s.Id == snapshotId)
                ?? throw new SnapshotNotFoundException($"Snapshot {snapshotId} not found in session {Id}");
            
            var previousGeneration = CurrentGeneration;
            _currentBoard = DeserializeBoardState(snapshot.BoardData);
            CurrentGeneration = snapshot.Generation;
            LastModified = DateTime.UtcNow;
            
            AddDomainEvent(new GameSessionRestored(Id, snapshotId, previousGeneration, CurrentGeneration));
            
            return _currentBoard;
        }

        /// <summary>
        /// Gets all snapshots in chronological order
        /// </summary>
        public IReadOnlyList<SessionSnapshot> GetSnapshots()
        {
            return _snapshots.OrderBy(s => s.Generation).ToList().AsReadOnly();
        }

        /// <summary>
        /// Removes old snapshots based on retention policy
        /// </summary>
        private void ApplySnapshotRetentionPolicy()
        {
            if (_settings.MaxSnapshots <= 0) return;
            
            // Keep manual snapshots and most recent auto snapshots
            var manualSnapshots = _snapshots.Where(s => !s.IsAutoGenerated).ToList();
            var autoSnapshots = _snapshots.Where(s => s.IsAutoGenerated)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
            
            var availableSlots = Math.Max(0, _settings.MaxSnapshots - manualSnapshots.Count);
            var autoSnapshotsToKeep = autoSnapshots.Take(availableSlots).ToList();
            var snapshotsToRemove = autoSnapshots.Skip(availableSlots).ToList();
            
            foreach (var snapshot in snapshotsToRemove)
            {
                _snapshots.Remove(snapshot);
                AddDomainEvent(new SessionSnapshotRemoved(Id, snapshot.Id, "Retention policy"));
            }
        }

        #endregion

        #region Session Lifecycle Management

        /// <summary>
        /// Pauses the game session
        /// </summary>
        public void Pause()
        {
            if (Status == SessionStatus.Paused)
                return;
                
            ValidateSessionActive();
            
            Status = SessionStatus.Paused;
            LastModified = DateTime.UtcNow;
            
            AddDomainEvent(new GameSessionPaused(Id, CurrentGeneration));
        }

        /// <summary>
        /// Resumes a paused game session
        /// </summary>
        public void Resume()
        {
            if (Status != SessionStatus.Paused)
                throw new InvalidOperationException($"Cannot resume session in {Status} state");
            
            Status = SessionStatus.Active;
            LastModified = DateTime.UtcNow;
            
            AddDomainEvent(new GameSessionResumed(Id, CurrentGeneration));
        }

        /// <summary>
        /// Archives the game session (soft delete)
        /// </summary>
        public void Archive(string reason = "User requested")
        {
            if (Status == SessionStatus.Archived)
                return;
            
            Status = SessionStatus.Archived;
            LastModified = DateTime.UtcNow;
            
            AddDomainEvent(new GameSessionArchived(Id, reason, CurrentGeneration));
        }

        /// <summary>
        /// Updates session settings
        /// </summary>
        public void UpdateSettings(SessionSettings newSettings)
        {
            ValidateSessionActive();
            
            var oldSettings = _settings;
            _settings = newSettings;
            LastModified = DateTime.UtcNow;
            
            AddDomainEvent(new GameSessionSettingsUpdated(Id, oldSettings, newSettings));
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Gets the current board state
        /// </summary>
        public Board GetCurrentBoard()
        {
            return _currentBoard ?? throw new InvalidOperationException("No current board state available");
        }

        /// <summary>
        /// Gets session statistics
        /// </summary>
        public SessionStatistics GetStatistics()
        {
            return new SessionStatistics(
                SessionId: Id,
                Name: Name,
                TotalGenerations: CurrentGeneration,
                SnapshotCount: _snapshots.Count,
                SessionDuration: DateTime.UtcNow - CreatedAt,
                CurrentPopulation: _currentBoard?.Alive().Count() ?? 0,
                Status: Status,
                LastActivity: LastModified
            );
        }

        /// <summary>
        /// Checks if the session is in a state that allows modifications
        /// </summary>
        public bool CanModify => Status == SessionStatus.Active;

        /// <summary>
        /// Gets the current session settings (for repository mapping)
        /// </summary>
        public SessionSettings GetSettings() => _settings;

        #endregion

        #region Private Helper Methods

        private void ValidateSessionCreation(string name, Board initialBoard, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Session name cannot be empty", nameof(name));
                
            if (name.Length > 100)
                throw new ArgumentException("Session name cannot exceed 100 characters", nameof(name));
                
            if (initialBoard == null)
                throw new ArgumentNullException(nameof(initialBoard));
                
            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentException("Creator cannot be empty", nameof(createdBy));
        }

        private void ValidateSessionActive()
        {
            if (Status != SessionStatus.Active)
                throw new InvalidOperationException($"Operation not allowed in {Status} state");
        }

        private bool ShouldCreateAutoSnapshot()
        {
            return CurrentGeneration > 0 && 
                   CurrentGeneration % _settings.AutoSnapshotInterval == 0;
        }

        private void CheckCompletionConditions(Board board)
        {
            // Check for extinction using Board's existing method
            if (!board.Alive().Any())
            {
                Status = SessionStatus.Completed;
                AddDomainEvent(new GameSessionExtinct(Id, CurrentGeneration));
                return;
            }
            
            // Use BoardSimulationService to detect potentially stable patterns
            // This provides early detection of stability without full final state analysis
            if (CurrentGeneration > 10 && BoardSimulationService.IsLikelyStable(board, 5))
            {
                CreateSnapshot($"Potentially stable pattern detected at generation {CurrentGeneration}");
                // Note: We don't mark as completed here, just create a snapshot for analysis
                // Full stability detection happens in RunToCompletion
            }
        }

        private byte[] SerializeBoardState(Board board)
        {
            // Delegate to BoardSerializationService for consistent serialization
            return BoardSerializationService.Serialize(board);
        }

        private Board DeserializeBoardState(byte[] data)
        {
            // Delegate to BoardSerializationService for consistent deserialization
            return BoardSerializationService.Deserialize(data);
        }

        #endregion
    }
}