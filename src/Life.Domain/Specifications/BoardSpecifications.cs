using Life.Domain.Aggregates;
using Life.Domain.Services;

namespace Life.Domain.Specifications
{
    /// <summary>
    /// Legacy specification classes - now delegate to static domain services
    /// Kept for backward compatibility with existing tests
    /// </summary>
    public static class CellSurvivalRules
    {
        /// <summary>
        /// Determines if a cell should be alive in the next generation based on Conway's rules
        /// Delegates to CellLifecycleService for consistency
        /// </summary>
        public static bool ShouldCellBeAlive(Board board, int x, int y)
        {
            return CellLifecycleService.ShouldCellSurvive(board, x, y);
        }
    }

    /// <summary>
    /// Legacy board comparison specification - delegates to static domain service
    /// Kept for backward compatibility with existing tests
    /// </summary>
    public static class BoardIdentityComparison
    {
        /// <summary>
        /// Determines if two boards are identical
        /// Delegates to BoardComparisonService for consistency
        /// </summary>
        public static bool AreIdentical(Board first, Board second)
        {
            return BoardComparisonService.IsBoardIdenticalTo(first, second);
        }
    }
}