using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service for detecting final states in Game of Life simulations
    /// </summary>
    public interface IFinalStateDetectionService
    {
        /// <summary>
        /// Detects if a board reaches a final state (stable or cyclic) within the given constraints
        /// </summary>
        FinalResult DetectFinalState(Board startingBoard, int maxIterations, TimeSpan maxTime);
    }
}