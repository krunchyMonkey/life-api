using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;
using System.Text;

namespace Life.Domain.Services
{
    /// <summary>
    /// Extension methods for Board to provide additional utility methods
    /// </summary>
    public static class BoardExtensions
    {
        /// <summary>
        /// Checks if a cell at the specified position is alive
        /// </summary>
        public static bool IsAlive(this Board board, int x, int y)
        {
            return board.Get(x, y);
        }

        /// <summary>
        /// Checks if a cell at the specified position is alive
        /// </summary>
        public static bool IsAlive(this Board board, Position position)
        {
            return board.Get(position);
        }

        /// <summary>
        /// Gets a visual representation of the board for debugging
        /// </summary>
        public static string ToVisualString(this Board board, char aliveChar = '?', char deadChar = '·')
        {
            var sb = new StringBuilder();
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    sb.Append(board.Get(x, y) ? aliveChar : deadChar);
                }
                if (y < board.Height - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the bounding box of all alive cells
        /// Returns null if no cells are alive
        /// </summary>
        public static (Position TopLeft, Position BottomRight)? GetBoundingBox(this Board board)
        {
            var aliveCells = board.Alive().ToList();
            if (!aliveCells.Any())
                return null;

            var minX = aliveCells.Min(c => c.x);
            var maxX = aliveCells.Max(c => c.x);
            var minY = aliveCells.Min(c => c.y);
            var maxY = aliveCells.Max(c => c.y);

            return (new Position(minX, minY), new Position(maxX, maxY));
        }

        /// <summary>
        /// Gets the density of alive cells as a percentage
        /// </summary>
        public static double GetDensity(this Board board)
        {
            var totalCells = board.Width * board.Height;
            var aliveCells = board.GetAliveCellCount();
            return totalCells > 0 ? (double)aliveCells / totalCells * 100.0 : 0.0;
        }
    }
}