using Life.Domain.Aggregates;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service for Game of Life simulation operations
    /// </summary>
    public interface IGameSimulationService
    {
        /// <summary>
        /// Computes the next generation of a board
        /// </summary>
        Board ComputeNextGeneration(Board board);
        
        /// <summary>
        /// Advances a board by the specified number of generations
        /// </summary>
        Board AdvanceGenerations(Board board, long generations);
    }
}