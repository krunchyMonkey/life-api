using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service interface for Game of Life operations (legacy compatibility)
    /// </summary>
    public interface IGame
    {
        Board Next(Board b);
        Board Advance(Board b, long n);
        FinalResult Final(Board start, int maxIterations, TimeSpan maxTime);
    }
}