using Life.Domain.Common;

namespace Life.Domain.Events
{
    /// <summary>
    /// Raised when a board advances to its next generation
    /// </summary>
    public sealed record BoardGenerationAdvanced(Guid OriginalBoardId, Guid NewBoardId) : DomainEvent;

    /// <summary>
    /// Raised when a board advances multiple generations at once
    /// </summary>
    public sealed record BoardMultipleGenerationsAdvanced(Guid OriginalBoardId, Guid FinalBoardId, long GenerationCount) : DomainEvent;

    /// <summary>
    /// Raised when a board reaches a final state (stable or cyclic)
    /// </summary>
    public sealed record BoardFinalStateDetected(Guid OriginalBoardId, Guid FinalBoardId, bool IsCyclic, bool IsStable, int? CycleLength) : DomainEvent;
}