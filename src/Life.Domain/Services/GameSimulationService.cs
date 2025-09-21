using Life.Domain.Aggregates;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service implementation for Game of Life simulation
    /// </summary>
    public sealed class GameSimulationService : IGameSimulationService
    {
        public Board ComputeNextGeneration(Board board)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));

            return board.NextGeneration();
        }

        public Board AdvanceGenerations(Board board, long generations)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));
            
            if (generations < 0)
                throw new ArgumentOutOfRangeException(nameof(generations), "Generations must be non-negative");

            var current = board;
            for (long i = 0; i < generations; i++)
            {
                current = ComputeNextGeneration(current);
            }
            
            return current;
        }
    }
}