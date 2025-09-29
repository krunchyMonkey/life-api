using Life.Domain.Common;
using Life.Domain.ValueObjects;

namespace Life.Domain.Events
{
    // Session Lifecycle Events
    public sealed record GameSessionCreated(
        Guid SessionId,
        string Name,
        string CreatedBy,
        BoardDimensions Dimensions) : DomainEvent;

    public sealed record GameSessionAdvanced(
        Guid SessionId,
        int PreviousGeneration,
        int CurrentGeneration) : DomainEvent;

    public sealed record GameSessionBulkAdvanced(
        Guid SessionId,
        int StartGeneration,
        int EndGeneration,
        int GenerationCount) : DomainEvent;

    public sealed record GameSessionCompleted(
        Guid SessionId,
        int StartGeneration,
        int FinalGeneration,
        bool IsStable,
        bool IsCyclic) : DomainEvent;

    public sealed record GameSessionExtinct(
        Guid SessionId,
        int FinalGeneration) : DomainEvent;

    public sealed record GameSessionPaused(
        Guid SessionId,
        int CurrentGeneration) : DomainEvent;

    public sealed record GameSessionResumed(
        Guid SessionId,
        int CurrentGeneration) : DomainEvent;

    public sealed record GameSessionArchived(
        Guid SessionId,
        string Reason,
        int FinalGeneration) : DomainEvent;

    public sealed record GameSessionRestored(
        Guid SessionId,
        Guid SnapshotId,
        int PreviousGeneration,
        int RestoredGeneration) : DomainEvent;

    // Snapshot Management Events
    public sealed record SessionSnapshotCreated(
        Guid SessionId,
        Guid SnapshotId,
        int Generation,
        string? Description) : DomainEvent;

    public sealed record SessionSnapshotRemoved(
        Guid SessionId,
        Guid SnapshotId,
        string Reason) : DomainEvent;

    // Configuration Events
    public sealed record GameSessionSettingsUpdated(
        Guid SessionId,
        SessionSettings OldSettings,
        SessionSettings NewSettings) : DomainEvent;
}