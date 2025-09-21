using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Static domain service for complex board analysis and simulation operations
    /// Provides advanced board operations as extension methods
    /// </summary>
    public static class BoardSimulationService
    {
        /// <summary>
        /// Advances the board by multiple generations efficiently
        /// </summary>
        public static Board AdvanceGenerations(this Board board, long count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Generation count must be non-negative");
            
            if (count == 0)
                return board;

            var current = board;
            for (long i = 0; i < count; i++)
            {
                current = current.GenerateNextGeneration();
            }
            
            return current;
        }

        /// <summary>
        /// Detects final states (stable or oscillating patterns)
        /// </summary>
        public static FinalResult DetectBoardFinalState(this Board board, int maxIterations, TimeSpan maxTime)
        {
            if (maxIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxIterations), "Max iterations must be positive");

            var seenStates = new Dictionary<string, int>();
            var current = board;
            var startTime = DateTime.UtcNow;

            for (int iteration = 1; iteration <= maxIterations; iteration++)
            {
                var stateKey = current.GetStateKey();
                
                // Check for cycles
                if (seenStates.TryGetValue(stateKey, out var previousIteration))
                {
                    var cycleLength = iteration - previousIteration;
                    return new FinalResult(current, false, true, iteration, cycleLength);
                }
                
                seenStates[stateKey] = iteration;
                var next = current.GenerateNextGeneration();
                
                // Check for stability
                if (current.IsIdenticalTo(next))
                {
                    return new FinalResult(next, true, false, iteration, null);
                }
                
                // Check timeout
                if ((DateTime.UtcNow - startTime) > maxTime)
                {
                    throw new TimeoutException("Final state detection exceeded maximum time limit");
                }
                
                current = next;
            }

            throw new TimeoutException($"Could not determine final state within {maxIterations} iterations");
        }

        /// <summary>
        /// Predicts the stability of a board by running a quick simulation
        /// </summary>
        public static bool IsLikelyStable(this Board board, int checkGenerations = 10)
        {
            var current = board;
            for (int i = 0; i < checkGenerations; i++)
            {
                var next = current.GenerateNextGeneration();
                if (current.IsIdenticalTo(next))
                    return true;
                current = next;
            }
            return false;
        }
    }
}