using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Static domain service for Conway's Game of Life rule evaluation
    /// Provides pure business logic as extension methods for Board
    /// </summary>
    public static class CellLifecycleService
    {
        /// <summary>
        /// Determines if a cell should be alive in the next generation based on Conway's rules
        /// </summary>
        public static bool ShouldCellSurvive(this Board board, int x, int y)
        {
            var position = new Position(x, y);
            var isCurrentlyAlive = board.Get(position);
            var liveNeighbors = board.CountLiveNeighbors(position);

            // Conway's Game of Life rules:
            // 1. Any live cell with fewer than two live neighbors dies (underpopulation)
            // 2. Any live cell with two or three live neighbors lives on to the next generation
            // 3. Any live cell with more than three live neighbors dies (overpopulation)
            // 4. Any dead cell with exactly three live neighbors becomes a live cell (reproduction)
            
            return isCurrentlyAlive ? (liveNeighbors == 2 || liveNeighbors == 3) : (liveNeighbors == 3);
        }

        /// <summary>
        /// Counts the number of live neighbors around a position
        /// </summary>
        public static int CountLiveNeighbors(this Board board, Position position)
        {
            return position.GetNeighbors()
                .Where(neighbor => board.Dimensions.Contains(neighbor))
                .Count(neighbor => board.Get(neighbor));
        }

        /// <summary>
        /// Generates the next generation board state
        /// </summary>
        public static Board GenerateNextGeneration(this Board board)
        {
            var aliveInNextGeneration = new List<Position>();
            
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (board.ShouldCellSurvive(x, y))
                    {
                        aliveInNextGeneration.Add(new Position(x, y));
                    }
                }
            }
            
            return new Board(board.Dimensions, aliveInNextGeneration);
        }
    }
}