using Life.Domain.Aggregates;

namespace Life.Domain.ValueObjects
{
    /// <summary>
    /// Represents the result of a Game of Life simulation final state detection
    /// </summary>
    public sealed record FinalResult(Board Board, bool Stable, bool Cyclic, int Iterations, int? CycleLength);
}