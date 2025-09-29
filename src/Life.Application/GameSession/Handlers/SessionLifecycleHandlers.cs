using Life.Application.GameSession.Commands;
using Life.Application.GameSession.Contracts;
using Life.Domain.Aggregates;
using Life.Domain.Repositories;
using Life.Domain.ValueObjects;
using MediatR;

namespace Life.Application.GameSession.Handlers
{
    public sealed class CreateSessionHandler : IRequestHandler<CreateSession, GameSessionResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public CreateSessionHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<GameSessionResponse> Handle(CreateSession request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            var board = new Life.Domain.Aggregates.Board(req.Width, req.Height, req.InitialCells.Select(c => (c.X, c.Y)));
            var settings = req.Settings.ToSessionSettings();

            var session = new Life.Domain.Aggregates.GameSession(req.Name, board, req.CreatedBy, settings);
            
            await _sessionRepository.AddAsync(session, cancellationToken);
            
            return session.ToResponse();
        }
    }

    public sealed class CreateSessionFromPatternHandler : IRequestHandler<CreateSessionFromPattern, GameSessionResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public CreateSessionFromPatternHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<GameSessionResponse> Handle(CreateSessionFromPattern request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            var dimensions = new BoardDimensions(req.Width, req.Height);
            var centerPosition = req.CenterPosition?.ToPosition();
            var settings = req.Settings.ToSessionSettings();

            var session = Life.Domain.Services.GameSessionService.CreateFromPattern(
                req.Name,
                req.CreatedBy,
                dimensions,
                req.Pattern,
                centerPosition,
                settings
            );
            
            await _sessionRepository.AddAsync(session, cancellationToken);
            
            return session.ToResponse();
        }
    }

    public sealed class GetSessionHandler : IRequestHandler<GetSession, GameSessionResponse?>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public GetSessionHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<GameSessionResponse?> Handle(GetSession request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            return session?.ToResponse();
        }
    }

    public sealed class GetUserSessionsHandler : IRequestHandler<GetUserSessions, IEnumerable<GameSessionResponse>>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public GetUserSessionsHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<GameSessionResponse>> Handle(GetUserSessions request, CancellationToken cancellationToken)
        {
            var sessions = await _sessionRepository.GetByCreatorAsync(request.Request.CreatedBy, cancellationToken);
            return sessions.Select(s => s.ToResponse());
        }
    }

    public sealed class GetActiveSessionsHandler : IRequestHandler<GetActiveSessions, IEnumerable<GameSessionResponse>>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public GetActiveSessionsHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<GameSessionResponse>> Handle(GetActiveSessions request, CancellationToken cancellationToken)
        {
            var sessions = await _sessionRepository.GetActiveSessionsAsync(cancellationToken);
            return sessions.Select(s => s.ToResponse());
        }
    }

    public sealed class SearchSessionsHandler : IRequestHandler<SearchSessions, IEnumerable<GameSessionResponse>>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public SearchSessionsHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<GameSessionResponse>> Handle(SearchSessions request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            
            SessionStatus? status = null;
            if (!string.IsNullOrEmpty(req.Status) && Enum.TryParse<SessionStatus>(req.Status, out var parsedStatus))
                status = parsedStatus;

            var sessions = await _sessionRepository.SearchSessionsAsync(
                req.NamePattern,
                req.CreatedBy,
                status,
                req.CreatedAfter,
                req.CreatedBefore,
                cancellationToken
            );
            
            return sessions.Select(s => s.ToResponse());
        }
    }
}