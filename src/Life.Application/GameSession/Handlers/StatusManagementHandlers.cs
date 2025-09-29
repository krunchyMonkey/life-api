using Life.Application.GameSession.Commands;
using Life.Domain.Repositories;
using MediatR;

namespace Life.Application.GameSession.Handlers
{
    public sealed class UpdateSessionStatusHandler : IRequestHandler<UpdateSessionStatus, Unit>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public UpdateSessionStatusHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Unit> Handle(UpdateSessionStatus request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            
            var session = await _sessionRepository.GetByIdAsync(req.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {req.SessionId} not found");

            switch (req.Action.ToLowerInvariant())
            {
                case "pause":
                    session.Pause();
                    break;
                case "resume":
                    session.Resume();
                    break;
                case "archive":
                    session.Archive(req.Reason ?? "User requested");
                    break;
                default:
                    throw new ArgumentException($"Unknown action: {req.Action}");
            }

            _sessionRepository.Update(session);
            return Unit.Value;
        }
    }
}