using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service interface for detecting final states in Game of Life simulations
    /// </summary>
    public interface IFinalDetector
    {
        FinalResult Detect(Board start, Func<Board, Board> next, int maxIterations, TimeSpan maxTime);
    }
}