using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;
using Life.Domain.Rules;

namespace Life.Domain.Services
{
    /// <summary>
    /// Static domain service for complex board analysis and simulation operations
    /// Now supports configurable rules for different cellular automaton variants
    /// </summary>
    public static class BoardSimulationService
    {
        /// <summary>
        /// Default rules (Conway's Game of Life)
        /// </summary>
        private static readonly ICellularAutomatonRules DefaultRules = CellularAutomatonRuleFactory.CreateConwaysRules();

        /// <summary>
        /// Advances the board by multiple generations using Conway's rules (backward compatibility)
        /// </summary>
        public static Board AdvanceGenerations(this Board board, long count)
        {
            return AdvanceGenerations(board, count, DefaultRules);
        }

        /// <summary>
        /// Advances the board by multiple generations using specified rules
        /// </summary>
        public static Board AdvanceGenerations(this Board board, long count, ICellularAutomatonRules rules)
        {
            return CellLifecycleService.GenerateMultipleGenerations(board, count, rules);
        }

        /// <summary>
        /// Detects final states using Conway's rules (backward compatibility)
        /// </summary>
        public static FinalResult DetectFinalState(this Board board, int maxIterations, TimeSpan maxTime)
        {
            return DetectFinalState(board, maxIterations, maxTime, DefaultRules);
        }

        /// <summary>
        /// Detects final states (stable or oscillating patterns) using specified rules
        /// </summary>
        public static FinalResult DetectFinalState(this Board board, int maxIterations, TimeSpan maxTime, ICellularAutomatonRules rules)
        {
            if (maxIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxIterations), "Max iterations must be positive");

            var seenStates = new Dictionary<string, int>();
            var current = board;
            var startTime = DateTime.UtcNow;

            for (int iteration = 1; iteration <= maxIterations; iteration++)
            {
                var stateKey = current.GetStateKey();
                
                // Check for cycles
                if (seenStates.TryGetValue(stateKey, out var previousIteration))
                {
                    var cycleLength = iteration - previousIteration;
                    return new FinalResult(current, false, true, iteration, cycleLength);
                }
                
                seenStates[stateKey] = iteration;
                var next = current.GenerateNextGeneration(rules);
                
                // Check for stability
                if (current.IsIdenticalTo(next))
                {
                    return new FinalResult(next, true, false, iteration, null);
                }
                
                // Check timeout
                if ((DateTime.UtcNow - startTime) > maxTime)
                {
                    throw new TimeoutException("Final state detection exceeded maximum time limit");
                }
                
                current = next;
            }

            throw new TimeoutException($"Could not determine final state within {maxIterations} iterations");
        }

        /// <summary>
        /// Predicts the stability of a board using Conway's rules (backward compatibility)
        /// </summary>
        public static bool IsLikelyStable(this Board board, int checkGenerations = 10)
        {
            return IsLikelyStable(board, checkGenerations, DefaultRules);
        }

        /// <summary>
        /// Predicts the stability of a board by running a quick simulation with specified rules
        /// </summary>
        public static bool IsLikelyStable(this Board board, int checkGenerations, ICellularAutomatonRules rules)
        {
            var current = board;
            for (int i = 0; i < checkGenerations; i++)
            {
                var next = current.GenerateNextGeneration(rules);
                if (current.IsIdenticalTo(next))
                    return true;
                current = next;
            }
            return false;
        }

        /// <summary>
        /// Analyzes board evolution patterns using specified rules
        /// </summary>
        public static BoardEvolutionAnalysis AnalyzeEvolution(this Board board, int generations, ICellularAutomatonRules rules)
        {
            var populationHistory = new List<int>();
            var current = board;
            
            populationHistory.Add(current.GetAliveCellCount());

            for (int i = 0; i < generations; i++)
            {
                current = current.GenerateNextGeneration(rules);
                populationHistory.Add(current.GetAliveCellCount());
            }

            return new BoardEvolutionAnalysis(
                StartingPopulation: board.GetAliveCellCount(),
                FinalPopulation: current.GetAliveCellCount(),
                PopulationHistory: populationHistory.AsReadOnly(),
                FinalBoard: current,
                RuleSetUsed: rules.RuleSetId,
                GenerationsSimulated: generations
            );
        }

        /// <summary>
        /// Compares evolution under different rule sets
        /// </summary>
        public static RuleComparisonResult CompareRules(this Board board, int generations, 
            params ICellularAutomatonRules[] ruleSets)
        {
            var results = new Dictionary<string, BoardEvolutionAnalysis>();

            foreach (var rules in ruleSets)
            {
                var analysis = board.AnalyzeEvolution(generations, rules);
                results[rules.RuleSetId] = analysis;
            }

            return new RuleComparisonResult(board, results);
        }
    }

    /// <summary>
    /// Represents the results of analyzing board evolution over time
    /// </summary>
    public sealed record BoardEvolutionAnalysis(
        int StartingPopulation,
        int FinalPopulation,
        IReadOnlyList<int> PopulationHistory,
        Board FinalBoard,
        string RuleSetUsed,
        int GenerationsSimulated)
    {
        /// <summary>
        /// Gets the maximum population reached during evolution
        /// </summary>
        public int MaxPopulation => PopulationHistory.Max();

        /// <summary>
        /// Gets the minimum population reached during evolution
        /// </summary>
        public int MinPopulation => PopulationHistory.Min();

        /// <summary>
        /// Gets the average population during evolution
        /// </summary>
        public double AveragePopulation => PopulationHistory.Average();

        /// <summary>
        /// Indicates if the population grew overall
        /// </summary>
        public bool PopulationGrew => FinalPopulation > StartingPopulation;

        /// <summary>
        /// Indicates if the population became extinct
        /// </summary>
        public bool PopulationExtinct => FinalPopulation == 0;
    }

    /// <summary>
    /// Represents the results of comparing different rule sets
    /// </summary>
    public sealed record RuleComparisonResult(Board OriginalBoard, IReadOnlyDictionary<string, BoardEvolutionAnalysis> Results)
    {
        /// <summary>
        /// Gets the rule set that resulted in the highest final population
        /// </summary>
        public string MostProductiveRule => Results
            .OrderByDescending(kvp => kvp.Value.FinalPopulation)
            .First().Key;

        /// <summary>
        /// Gets the rule set that resulted in the most stable population
        /// </summary>
        public string MostStableRule => Results
            .OrderBy(kvp => CalculatePopulationVariance(kvp.Value.PopulationHistory))
            .First().Key;

        private static double CalculatePopulationVariance(IReadOnlyList<int> populations)
        {
            var mean = populations.Average();
            return populations.Average(p => Math.Pow(p - mean, 2));
        }
    }
}