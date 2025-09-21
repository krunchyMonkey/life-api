using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Rules.Standard
{
    /// <summary>
    /// Day & Night rules (B3678/S34678)
    /// Symmetric rule where patterns and their inverses behave the same
    /// </summary>
    public sealed class DayAndNightRules : CellularAutomatonRulesBase
    {
        public override string RuleSetId => "DayNight-B3678-S34678";

        private readonly int[] _birthCounts = { 3, 6, 7, 8 };
        private readonly int[] _survivalCounts = { 3, 4, 6, 7, 8 };

        public override bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors)
        {
            return CheckSurvivalCondition(isCurrentlyAlive, liveNeighbors, _birthCounts, _survivalCounts);
        }

        public override string GetRuleDescription()
        {
            return "Day & Night (B3678/S34678): " +
                   "Symmetric rules where patterns and their inverses evolve identically. " +
                   "Creates interesting symmetric behaviors.";
        }
    }
}