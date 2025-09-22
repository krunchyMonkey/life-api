using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Rules.Custom
{
    /// <summary>
    /// Custom configurable rules
    /// Allows defining arbitrary birth and survival conditions
    /// </summary>
    public sealed class CustomRules : CellularAutomatonRulesBase
    {
        private readonly int[] _birthCounts;
        private readonly int[] _survivalCounts;
        private readonly string _ruleSetId;
        private readonly string _description;

        public CustomRules(int[] birthCounts, int[] survivalCounts, string? customId = null, string? description = null)
        {
            _birthCounts = birthCounts ?? throw new ArgumentNullException(nameof(birthCounts));
            _survivalCounts = survivalCounts ?? throw new ArgumentNullException(nameof(survivalCounts));
            
            // Generate rule notation (e.g., "B36/S23")
            var birthNotation = string.Join("", birthCounts);
            var survivalNotation = string.Join("", survivalCounts);
            _ruleSetId = customId ?? $"Custom-B{birthNotation}-S{survivalNotation}";
            
            _description = description ?? $"Custom Rules (B{birthNotation}/S{survivalNotation}): " +
                          $"Birth on {string.Join(",", birthCounts)} neighbors, " +
                          $"Survival on {string.Join(",", survivalCounts)} neighbors.";
        }

        public override string RuleSetId => _ruleSetId;

        public override bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors)
        {
            return CheckSurvivalCondition(isCurrentlyAlive, liveNeighbors, _birthCounts, _survivalCounts);
        }

        public override string GetRuleDescription()
        {
            return _description;
        }
    }
}