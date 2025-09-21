using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service implementation for detecting final states
    /// </summary>
    public sealed class FinalStateDetectionService : IFinalStateDetectionService
    {
        private readonly IGameSimulationService _simulationService;
        private readonly BoardHasher _boardHasher;

        public FinalStateDetectionService(IGameSimulationService simulationService, BoardHasher boardHasher)
        {
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _boardHasher = boardHasher ?? throw new ArgumentNullException(nameof(boardHasher));
        }

        public FinalResult DetectFinalState(Board startingBoard, int maxIterations, TimeSpan maxTime)
        {
            if (startingBoard == null)
                throw new ArgumentNullException(nameof(startingBoard));
            
            if (maxIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxIterations), "Max iterations must be positive");

            var seenStates = new Dictionary<ulong, int>();
            var currentBoard = startingBoard;
            var startTime = DateTime.UtcNow;

            for (int iteration = 1; iteration <= maxIterations; iteration++)
            {
                var boardHash = _boardHasher.Hash(currentBoard);
                
                // Check if we've seen this state before (cycle detection)
                if (seenStates.TryGetValue(boardHash, out var previousIteration))
                {
                    return new FinalResult(currentBoard, false, true, iteration, iteration - previousIteration);
                }
                
                seenStates[boardHash] = iteration;
                
                // Compute next generation
                var nextBoard = _simulationService.ComputeNextGeneration(currentBoard);
                
                // Check for stability (board doesn't change)
                if (currentBoard.IsIdenticalTo(nextBoard))
                {
                    return new FinalResult(nextBoard, true, false, iteration, null);
                }
                
                // Check for timeout
                if ((DateTime.UtcNow - startTime) > maxTime)
                {
                    throw new TimeoutException("Final state detection exceeded maximum time limit");
                }
                
                currentBoard = nextBoard;
            }

            // Reached maximum iterations without finding a final state
            throw new TimeoutException($"Could not determine final state within {maxIterations} iterations");
        }
    }
}