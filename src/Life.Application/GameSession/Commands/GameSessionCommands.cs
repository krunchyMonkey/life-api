using Life.Application.GameSession.Contracts;
using MediatR;

namespace Life.Application.GameSession.Commands
{
    // Session lifecycle commands
    public sealed record CreateSession(CreateSessionRequest Request) : IRequest<GameSessionResponse>;
    
    public sealed record CreateSessionFromPattern(CreateFromPatternRequest Request) : IRequest<GameSessionResponse>;
    
    public sealed record GetSession(Guid SessionId) : IRequest<GameSessionResponse?>;
    
    public sealed record GetUserSessions(GetUserSessionsRequest Request) : IRequest<IEnumerable<GameSessionResponse>>;
    
    public sealed record GetActiveSessions() : IRequest<IEnumerable<GameSessionResponse>>;
    
    public sealed record SearchSessions(SearchSessionsRequest Request) : IRequest<IEnumerable<GameSessionResponse>>;

    // Session operation commands
    public sealed record AdvanceGeneration(AdvanceGenerationRequest Request) : IRequest<BoardResponse>;
    
    public sealed record AdvanceGenerations(AdvanceGenerationsRequest Request) : IRequest<BoardResponse>;
    
    public sealed record RunToCompletion(RunToCompletionRequest Request) : IRequest<FinalResultResponse>;

    // Snapshot commands
    public sealed record CreateSnapshot(CreateSnapshotRequest Request) : IRequest<SnapshotResponse>;
    
    public sealed record GetSnapshots(Guid SessionId) : IRequest<IEnumerable<SnapshotResponse>>;
    
    public sealed record RestoreFromSnapshot(RestoreSnapshotRequest Request) : IRequest<BoardResponse>;

    // Analysis commands
    public sealed record GetSessionStatistics(Guid SessionId) : IRequest<SessionStatisticsResponse>;
    
    public sealed record AnalyzeSession(Guid SessionId) : IRequest<SessionAnalysisResponse>;
    
    public sealed record CompareSessions(CompareSessionsRequest Request) : IRequest<SessionComparisonResponse>;

    // Status management commands
    public sealed record UpdateSessionStatus(UpdateSessionStatusRequest Request) : IRequest<Unit>;
}