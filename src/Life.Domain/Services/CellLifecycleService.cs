using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;
using Life.Domain.Rules;

namespace Life.Domain.Services
{
    /// <summary>
    /// Static domain service for cellular automaton lifecycle management
    /// Now supports configurable rules following Strategy Pattern
    /// </summary>
    public static class CellLifecycleService
    {
        /// <summary>
        /// Default rules to use when none specified (Conway's Game of Life)
        /// </summary>
        private static readonly ICellularAutomatonRules DefaultRules = CellularAutomatonRuleFactory.CreateConwaysRules();

        /// <summary>
        /// Determines if a cell should be alive using Conway's rules (backward compatibility)
        /// </summary>
        public static bool ShouldCellSurvive(Board board, int x, int y)
        {
            return ShouldCellSurvive(board, x, y, DefaultRules);
        }

        /// <summary>
        /// Determines if a cell should be alive using specified rules
        /// </summary>
        public static bool ShouldCellSurvive(Board board, int x, int y, ICellularAutomatonRules rules)
        {
            var position = new Position(x, y);
            var isCurrentlyAlive = board.Get(position);
            var liveNeighbors = board.CountLiveNeighbors(position);

            return rules.ShouldCellBeAlive(board, position, isCurrentlyAlive, liveNeighbors);
        }

        /// <summary>
        /// Counts the number of live neighbors around a position
        /// </summary>
        public static int CountLiveNeighbors(this Board board, Position position)
        {
            return position.GetNeighbors()
                .Where(neighbor => board.Dimensions.Contains(neighbor))
                .Count(neighbor => board.Get(neighbor));
        }

        /// <summary>
        /// Generates the next generation board using Conway's rules (backward compatibility)
        /// </summary>
        public static Board GenerateNextGeneration(this Board board)
        {
            return GenerateNextGeneration(board, DefaultRules);
        }

        /// <summary>
        /// Generates the next generation board using specified rules
        /// </summary>
        public static Board GenerateNextGeneration(this Board board, ICellularAutomatonRules rules)
        {
            var aliveInNextGeneration = new List<Position>();
            
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (ShouldCellSurvive(board, x, y, rules))
                    {
                        aliveInNextGeneration.Add(new Position(x, y));
                    }
                }
            }
            
            return new Board(board.Dimensions, aliveInNextGeneration);
        }

        /// <summary>
        /// Generates multiple generations using Conway's rules
        /// </summary>
        public static Board GenerateMultipleGenerations(this Board board, long count)
        {
            return GenerateMultipleGenerations(board, count, DefaultRules);
        }

        /// <summary>
        /// Generates multiple generations using specified rules
        /// </summary>
        public static Board GenerateMultipleGenerations(this Board board, long count, ICellularAutomatonRules rules)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Generation count must be non-negative");
            
            if (count == 0)
                return board;

            var current = board;
            for (long i = 0; i < count; i++)
            {
                current = current.GenerateNextGeneration(rules);
            }
            
            return current;
        }

        /// <summary>
        /// Creates a rule-aware board generator for fluent API
        /// </summary>
        public static BoardGenerator WithRules(this Board board, ICellularAutomatonRules rules)
        {
            return new BoardGenerator(board, rules);
        }

        /// <summary>
        /// Creates a rule-aware board generator using rule name
        /// </summary>
        public static BoardGenerator WithRules(this Board board, string ruleName)
        {
            var rules = CellularAutomatonRuleFactory.CreateRules(ruleName);
            return new BoardGenerator(board, rules);
        }

        /// <summary>
        /// Creates a rule-aware board generator using standard notation
        /// </summary>
        public static BoardGenerator WithRuleNotation(this Board board, string notation)
        {
            var rules = CellularAutomatonRuleFactory.CreateFromNotation(notation);
            return new BoardGenerator(board, rules);
        }
    }

    /// <summary>
    /// Fluent API for board generation with specific rules
    /// Provides a clean interface for rule-based board operations
    /// </summary>
    public sealed class BoardGenerator
    {
        private readonly Board _board;
        private readonly ICellularAutomatonRules _rules;

        internal BoardGenerator(Board board, ICellularAutomatonRules rules)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <summary>
        /// Generates the next generation using the configured rules
        /// </summary>
        public Board NextGeneration()
        {
            return CellLifecycleService.GenerateNextGeneration(_board, _rules);
        }

        /// <summary>
        /// Generates multiple generations using the configured rules
        /// </summary>
        public Board AdvanceGenerations(long count)
        {
            return CellLifecycleService.GenerateMultipleGenerations(_board, count, _rules);
        }

        /// <summary>
        /// Gets information about the configured rules
        /// </summary>
        public string GetRuleDescription()
        {
            return _rules.GetRuleDescription();
        }

        /// <summary>
        /// Gets the rule set identifier
        /// </summary>
        public string GetRuleSetId()
        {
            return _rules.RuleSetId;
        }
    }
}