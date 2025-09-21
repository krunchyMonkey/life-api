using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Legacy Game service that maintains backward compatibility while delegating to DDD services
    /// </summary>
    public sealed class Game : IGame
    {
        private readonly IGameSimulationService _simulationService;
        private readonly IFinalStateDetectionService _finalStateService;

        public Game()
        {
            // Create dependencies internally for backward compatibility
            _simulationService = new GameSimulationService();
            var boardHasher = new BoardHasher();
            _finalStateService = new FinalStateDetectionService(_simulationService, boardHasher);
        }

        public Game(IGameSimulationService simulationService, IFinalStateDetectionService finalStateService)
        {
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _finalStateService = finalStateService ?? throw new ArgumentNullException(nameof(finalStateService));
        }

        public Board Next(Board board)
        {
            return _simulationService.ComputeNextGeneration(board);
        }

        public Board Advance(Board board, long generations)
        {
            return _simulationService.AdvanceGenerations(board, generations);
        }

        public FinalResult Final(Board start, int maxIterations, TimeSpan maxTime)
        {
            return _finalStateService.DetectFinalState(start, maxIterations, maxTime);
        }
    }
}
