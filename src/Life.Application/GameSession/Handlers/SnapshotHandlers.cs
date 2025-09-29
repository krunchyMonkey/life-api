using Life.Application.GameSession.Commands;
using Life.Application.GameSession.Contracts;
using Life.Domain.Exceptions;
using Life.Domain.Repositories;
using MediatR;

namespace Life.Application.GameSession.Handlers
{
    public sealed class CreateSnapshotHandler : IRequestHandler<CreateSnapshot, SnapshotResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public CreateSnapshotHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<SnapshotResponse> Handle(CreateSnapshot request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.Request.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {request.Request.SessionId} not found");

            if (!session.CanModify)
                throw new InvalidOperationException($"Session is in {session.Status} state and cannot be modified");

            var snapshot = session.CreateSnapshot(request.Request.Description);
            _sessionRepository.Update(session);
            
            return snapshot.ToResponse();
        }
    }

    public sealed class GetSnapshotsHandler : IRequestHandler<GetSnapshots, IEnumerable<SnapshotResponse>>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public GetSnapshotsHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<SnapshotResponse>> Handle(GetSnapshots request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {request.SessionId} not found");

            var snapshots = session.GetSnapshots();
            return snapshots.Select(s => s.ToResponse());
        }
    }

    public sealed class RestoreFromSnapshotHandler : IRequestHandler<RestoreFromSnapshot, BoardResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public RestoreFromSnapshotHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<BoardResponse> Handle(RestoreFromSnapshot request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            
            var session = await _sessionRepository.GetByIdAsync(req.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {req.SessionId} not found");

            if (!session.CanModify)
                throw new InvalidOperationException($"Session is in {session.Status} state and cannot be modified");

            try
            {
                var restoredBoard = session.RestoreFromSnapshot(req.SnapshotId);
                _sessionRepository.Update(session);
                
                return restoredBoard.ToResponse();
            }
            catch (SnapshotNotFoundException)
            {
                throw new InvalidOperationException($"Snapshot {req.SnapshotId} not found");
            }
        }
    }
}