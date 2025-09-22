using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Rules.Standard
{
    /// <summary>
    /// Conway's Game of Life rules (B3/S23)
    /// Birth on 3 neighbors, Survival on 2 or 3 neighbors
    /// </summary>
    public sealed class ConwaysGameOfLifeRules : CellularAutomatonRulesBase
    {
        public override string RuleSetId => "Conway-B3-S23";

        private readonly int[] _birthCounts = { 3 };
        private readonly int[] _survivalCounts = { 2, 3 };

        public override bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors)
        {
            return CheckSurvivalCondition(isCurrentlyAlive, liveNeighbors, _birthCounts, _survivalCounts);
        }

        public override string GetRuleDescription()
        {
            return "Conway's Game of Life (B3/S23): " +
                   "A live cell with 2-3 neighbors survives, " +
                   "a dead cell with exactly 3 neighbors becomes alive, " +
                   "all other cells die or stay dead.";
        }
    }
}