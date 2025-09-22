using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Rules.Standard
{
    /// <summary>
    /// HighLife rules (B36/S23) 
    /// Similar to Conway's but with additional birth condition
    /// </summary>
    public sealed class HighLifeRules : CellularAutomatonRulesBase
    {
        public override string RuleSetId => "HighLife-B36-S23";

        private readonly int[] _birthCounts = { 3, 6 };
        private readonly int[] _survivalCounts = { 2, 3 };

        public override bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors)
        {
            return CheckSurvivalCondition(isCurrentlyAlive, liveNeighbors, _birthCounts, _survivalCounts);
        }

        public override string GetRuleDescription()
        {
            return "HighLife (B36/S23): " +
                   "Like Conway's Life, but dead cells with 6 neighbors also become alive. " +
                   "Known for creating replicator patterns.";
        }
    }
}