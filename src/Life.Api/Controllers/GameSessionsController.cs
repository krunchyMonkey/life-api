using Life.Application.GameSession.Commands;
using Life.Application.GameSession.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Life.Api.Controllers
{
    [ApiController]
    [Route("api/v1/game-sessions")]
    public sealed class GameSessionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public GameSessionsController(IMediator mediator) => _mediator = mediator;

        #region Session Lifecycle
        
        [HttpPost]
        public Task<GameSessionResponse> CreateSession([FromBody] CreateSessionRequest request) 
            => _mediator.Send(new CreateSession(request));

        [HttpPost("from-pattern")]
        public Task<GameSessionResponse> CreateFromPattern([FromBody] CreateFromPatternRequest request) 
            => _mediator.Send(new CreateSessionFromPattern(request));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GameSessionResponse>> GetSession(Guid id)
        {
            var result = await _mediator.Send(new GetSession(id));
            return result != null ? Ok(result) : NotFound($"Session {id} not found");
        }

        [HttpGet("user/{createdBy}")]
        public Task<IEnumerable<GameSessionResponse>> GetUserSessions(string createdBy) 
            => _mediator.Send(new GetUserSessions(new GetUserSessionsRequest { CreatedBy = createdBy }));

        [HttpGet("active")]
        public Task<IEnumerable<GameSessionResponse>> GetActiveSessions() 
            => _mediator.Send(new GetActiveSessions());

        [HttpPost("search")]
        public Task<IEnumerable<GameSessionResponse>> SearchSessions([FromBody] SearchSessionsRequest request) 
            => _mediator.Send(new SearchSessions(request));

        #endregion

        #region Session Operations

        [HttpPost("{id:guid}/advance")]
        public Task<BoardResponse> AdvanceGeneration(Guid id) 
            => _mediator.Send(new AdvanceGeneration(new AdvanceGenerationRequest { SessionId = id }));

        [HttpPost("{id:guid}/advance/{count:int}")]
        public Task<BoardResponse> AdvanceGenerations(Guid id, int count) 
            => _mediator.Send(new AdvanceGenerations(new AdvanceGenerationsRequest { SessionId = id, Count = count }));

        [HttpPost("{id:guid}/run-to-completion")]
        public Task<FinalResultResponse> RunToCompletion(
            Guid id, 
            [FromQuery] int maxIterations = 1000,
            [FromQuery] int maxTimeMinutes = 5) 
            => _mediator.Send(new RunToCompletion(new RunToCompletionRequest 
            { 
                SessionId = id, 
                MaxIterations = maxIterations, 
                MaxTimeMinutes = maxTimeMinutes 
            }));

        #endregion

        #region Snapshots

        [HttpPost("{id:guid}/snapshots")]
        public Task<SnapshotResponse> CreateSnapshot(Guid id, [FromBody] CreateSnapshotRequest request) 
            => _mediator.Send(new CreateSnapshot(request with { SessionId = id }));

        [HttpGet("{id:guid}/snapshots")]
        public Task<IEnumerable<SnapshotResponse>> GetSnapshots(Guid id) 
            => _mediator.Send(new GetSnapshots(id));

        [HttpPost("{id:guid}/snapshots/{snapshotId:guid}/restore")]
        public Task<BoardResponse> RestoreFromSnapshot(Guid id, Guid snapshotId) 
            => _mediator.Send(new RestoreFromSnapshot(new RestoreSnapshotRequest { SessionId = id, SnapshotId = snapshotId }));

        #endregion

        #region Analysis

        [HttpGet("{id:guid}/statistics")]
        public Task<SessionStatisticsResponse> GetStatistics(Guid id) 
            => _mediator.Send(new GetSessionStatistics(id));

        [HttpGet("{id:guid}/analysis")]
        public Task<SessionAnalysisResponse> AnalyzeSession(Guid id) 
            => _mediator.Send(new AnalyzeSession(id));

        [HttpPost("compare")]
        public Task<SessionComparisonResponse> CompareSessions([FromBody] CompareSessionsRequest request) 
            => _mediator.Send(new CompareSessions(request));

        #endregion

        #region Status Management

        [HttpPost("{id:guid}/pause")]
        public Task PauseSession(Guid id) 
            => _mediator.Send(new UpdateSessionStatus(new UpdateSessionStatusRequest { SessionId = id, Action = "pause" }));

        [HttpPost("{id:guid}/resume")]
        public Task ResumeSession(Guid id) 
            => _mediator.Send(new UpdateSessionStatus(new UpdateSessionStatusRequest { SessionId = id, Action = "resume" }));

        [HttpPost("{id:guid}/archive")]
        public Task ArchiveSession(Guid id, [FromBody] ArchiveSessionRequest? request = null) 
            => _mediator.Send(new UpdateSessionStatus(new UpdateSessionStatusRequest 
            { 
                SessionId = id, 
                Action = "archive", 
                Reason = request?.Reason 
            }));

        #endregion
    }
}