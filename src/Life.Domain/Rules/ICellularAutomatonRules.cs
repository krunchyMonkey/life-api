using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Rules
{
    /// <summary>
    /// Interface for cellular automaton rules
    /// Follows Strategy Pattern for configurable game rules
    /// </summary>
    public interface ICellularAutomatonRules
    {
        /// <summary>
        /// Determines if a cell should be alive in the next generation
        /// </summary>
        /// <param name="board">Current board state</param>
        /// <param name="position">Position to evaluate</param>
        /// <param name="isCurrentlyAlive">Whether the cell is currently alive</param>
        /// <param name="liveNeighbors">Number of live neighbors</param>
        /// <returns>True if cell should be alive in next generation</returns>
        bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors);

        /// <summary>
        /// Gets a human-readable description of the rules
        /// </summary>
        string GetRuleDescription();

        /// <summary>
        /// Gets a unique identifier for the rule set
        /// </summary>
        string RuleSetId { get; }
    }

    /// <summary>
    /// Abstract base class for cellular automaton rules
    /// Provides common functionality for rule implementations
    /// </summary>
    public abstract class CellularAutomatonRulesBase : ICellularAutomatonRules
    {
        public abstract string RuleSetId { get; }
        
        public abstract bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors);
        
        public abstract string GetRuleDescription();

        /// <summary>
        /// Helper method to check survival conditions based on neighbor counts
        /// </summary>
        protected static bool CheckSurvivalCondition(bool isCurrentlyAlive, int liveNeighbors, 
            int[] birthCounts, int[] survivalCounts)
        {
            if (isCurrentlyAlive)
            {
                return survivalCounts.Contains(liveNeighbors);
            }
            else
            {
                return birthCounts.Contains(liveNeighbors);
            }
        }
    }
}