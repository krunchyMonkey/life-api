using Life.Application.GameSession.Commands;
using Life.Application.GameSession.Contracts;
using Life.Domain.Repositories;
using Life.Domain.Services;
using MediatR;

namespace Life.Application.GameSession.Handlers
{
    public sealed class GetSessionStatisticsHandler : IRequestHandler<GetSessionStatistics, SessionStatisticsResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public GetSessionStatisticsHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<SessionStatisticsResponse> Handle(GetSessionStatistics request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {request.SessionId} not found");

            var stats = session.GetStatistics();
            return stats.ToResponse();
        }
    }

    public sealed class AnalyzeSessionHandler : IRequestHandler<AnalyzeSession, SessionAnalysisResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public AnalyzeSessionHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<SessionAnalysisResponse> Handle(AnalyzeSession request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session == null)
                throw new InvalidOperationException($"Session {request.SessionId} not found");

            var analysis = GameSessionService.AnalyzeSession(session);
            return new SessionAnalysisResponse
            {
                SessionId = analysis.SessionId,
                GenerationAnalyzed = analysis.GenerationAnalyzed,
                PopulationTrend = analysis.PopulationTrend.ToString(),
                DetectedPatterns = analysis.DetectedPatterns.Select(p => new PatternResponse
                {
                    Name = p.Name,
                    Location = new PositionResponse { X = p.Location.X, Y = p.Location.Y },
                    Period = p.Period
                }).ToList(),
                StabilityIndex = analysis.StabilityIndex,
                ComplexityScore = analysis.ComplexityScore
            };
        }
    }

    public sealed class CompareSessionsHandler : IRequestHandler<CompareSessions, SessionComparisonResponse>
    {
        private readonly IGameSessionRepository _sessionRepository;

        public CompareSessionsHandler(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<SessionComparisonResponse> Handle(CompareSessions request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            
            var session1 = await _sessionRepository.GetByIdAsync(req.Session1Id, cancellationToken);
            var session2 = await _sessionRepository.GetByIdAsync(req.Session2Id, cancellationToken);
            
            if (session1 == null)
                throw new InvalidOperationException($"Session {req.Session1Id} not found");
            if (session2 == null)
                throw new InvalidOperationException($"Session {req.Session2Id} not found");

            var comparison = GameSessionService.CompareSessionEvolution(
                session1, 
                session2, 
                req.GenerationsToCompare);
            
            return new SessionComparisonResponse
            {
                Session1Id = comparison.Session1Id,
                Session2Id = comparison.Session2Id,
                GenerationsCompared = comparison.GenerationsCompared,
                AverageSimilarity = comparison.AverageSimilarity,
                MaxSimilarity = comparison.MaxSimilarity,
                MinSimilarity = comparison.MinSimilarity,
                SimilarityScores = comparison.SimilarityScores.ToList()
            };
        }
    }
}