using Life.Application.GameSession.Commands;
using Life.Application.GameSession.Contracts;
using Life.Domain.Repositories;
using MediatR;

namespace Life.Application.GameSession.Handlers
{
    public sealed class AdvanceGenerationHandler : IRequestHandler<AdvanceGeneration, BoardResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public AdvanceGenerationHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<BoardResponse> Handle(AdvanceGeneration request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.Request.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {request.Request.SessionId} not found");

            if (!session.CanModify)
                throw new InvalidOperationException($"Session is in {session.Status} state and cannot be modified");

            var nextBoard = session.AdvanceGeneration();
            _sessionRepository.Update(session);
            
            return nextBoard.ToResponse();
        }
    }

    public sealed class AdvanceGenerationsHandler : IRequestHandler<AdvanceGenerations, BoardResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public AdvanceGenerationsHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<BoardResponse> Handle(AdvanceGenerations request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            
            if (req.Count <= 0)
                throw new ArgumentOutOfRangeException(nameof(req.Count), "Generation count must be positive");

            var session = await _sessionRepository.GetByIdAsync(req.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {req.SessionId} not found");

            if (!session.CanModify)
                throw new InvalidOperationException($"Session is in {session.Status} state and cannot be modified");

            var finalBoard = session.AdvanceGenerations(req.Count);
            _sessionRepository.Update(session);
            
            return finalBoard.ToResponse();
        }
    }

    public sealed class RunToCompletionHandler : IRequestHandler<RunToCompletion, FinalResultResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public RunToCompletionHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<FinalResultResponse> Handle(RunToCompletion request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            
            var session = await _sessionRepository.GetByIdAsync(req.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {req.SessionId} not found");

            if (!session.CanModify)
                throw new InvalidOperationException($"Session is in {session.Status} state and cannot be modified");

            var maxTime = TimeSpan.FromMinutes(req.MaxTimeMinutes);
            var result = session.RunToCompletion(req.MaxIterations, maxTime);
            _sessionRepository.Update(session);
            
            return new FinalResultResponse
            {
                Board = result.Board.ToResponse(),
                Stable = result.Stable,
                Cyclic = result.Cyclic,
                Iterations = result.Iterations,
                CycleLength = result.CycleLength,
                FinalGeneration = session.CurrentGeneration
            };
        }
    }
}