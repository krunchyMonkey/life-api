using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Static domain service for board comparison and identity operations
    /// Provides pure comparison logic as extension methods for Board
    /// </summary>
    public static class BoardComparisonService
    {
        /// <summary>
        /// Checks if this board is identical to another board
        /// </summary>
        public static bool IsBoardIdenticalTo(this Board first, Board second)
        {
            if (first is null || second is null) return false;
            if (first.Dimensions != second.Dimensions) return false;
            
            var firstAlive = first.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            var secondAlive = second.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            
            return firstAlive.SequenceEqual(secondAlive);
        }

        /// <summary>
        /// Gets a unique string representation of the board state for cycle detection
        /// </summary>
        public static string GetStateKey(this Board board)
        {
            var alive = board.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            return string.Join(",", alive.Select(c => $"{c.x}:{c.y}"));
        }

        /// <summary>
        /// Checks if the board has any alive cells
        /// </summary>
        public static bool HasAliveCells(this Board board)
        {
            return board.Alive().Any();
        }

        /// <summary>
        /// Gets the total count of alive cells
        /// </summary>
        public static int GetAliveCellCount(this Board board)
        {
            return board.Alive().Count();
        }
    }
}