using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Rules.Standard
{
    /// <summary>
    /// Seeds rules (B2/S) 
    /// Every cell dies every generation, creating short-lived explosive patterns
    /// </summary>
    public sealed class SeedsRules : CellularAutomatonRulesBase
    {
        public override string RuleSetId => "Seeds-B2-S";

        private readonly int[] _birthCounts = { 2 };
        private readonly int[] _survivalCounts = Array.Empty<int>(); // No cells survive

        public override bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors)
        {
            return CheckSurvivalCondition(isCurrentlyAlive, liveNeighbors, _birthCounts, _survivalCounts);
        }

        public override string GetRuleDescription()
        {
            return "Seeds (B2/S): " +
                   "All cells die each generation, new cells born with exactly 2 neighbors. " +
                   "Creates explosive, short-lived patterns.";
        }
    }
}