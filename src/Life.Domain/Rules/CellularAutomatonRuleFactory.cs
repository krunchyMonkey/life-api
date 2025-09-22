using Life.Domain.Rules.Standard;
using Life.Domain.Rules.Custom;

namespace Life.Domain.Rules
{
    /// <summary>
    /// Factory for creating cellular automaton rules
    /// Follows Factory Pattern and provides a registry of available rules
    /// </summary>
    public static class CellularAutomatonRuleFactory
    {
        private static readonly Dictionary<string, Func<ICellularAutomatonRules>> _ruleCreators = new()
        {
            { "conway", () => new ConwaysGameOfLifeRules() },
            { "highlife", () => new HighLifeRules() },
            { "daynight", () => new DayAndNightRules() },
            { "seeds", () => new SeedsRules() }
        };

        /// <summary>
        /// Creates a rule set by name (case-insensitive)
        /// </summary>
        public static ICellularAutomatonRules CreateRules(string ruleName)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
                throw new ArgumentException("Rule name cannot be null or empty", nameof(ruleName));

            var key = ruleName.ToLowerInvariant();
            
            if (_ruleCreators.TryGetValue(key, out var creator))
            {
                return creator();
            }

            throw new ArgumentException($"Unknown rule set: {ruleName}. Available rules: {string.Join(", ", GetAvailableRules())}");
        }

        /// <summary>
        /// Creates Conway's Game of Life rules (default)
        /// </summary>
        public static ICellularAutomatonRules CreateConwaysRules()
        {
            return new ConwaysGameOfLifeRules();
        }

        /// <summary>
        /// Creates custom rules with specified birth and survival conditions
        /// </summary>
        public static ICellularAutomatonRules CreateCustomRules(int[] birthCounts, int[] survivalCounts, 
            string? customId = null, string? description = null)
        {
            return new CustomRules(birthCounts, survivalCounts, customId, description);
        }

        /// <summary>
        /// Creates custom rules from standard notation (e.g., "B3/S23", "B36/S23")
        /// </summary>
        public static ICellularAutomatonRules CreateFromNotation(string notation)
        {
            if (string.IsNullOrWhiteSpace(notation))
                throw new ArgumentException("Notation cannot be null or empty", nameof(notation));

            // Parse notation like "B3/S23" or "B36/S23"
            var parts = notation.ToUpperInvariant().Split('/');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid notation format. Expected format: B{numbers}/S{numbers}");

            var birthPart = parts[0];
            var survivalPart = parts[1];

            if (!birthPart.StartsWith('B') || !survivalPart.StartsWith('S'))
                throw new ArgumentException("Invalid notation format. Expected format: B{numbers}/S{numbers}");

            var birthCounts = ParseNumbers(birthPart[1..]);
            var survivalCounts = ParseNumbers(survivalPart[1..]);

            return new CustomRules(birthCounts, survivalCounts);
        }

        /// <summary>
        /// Gets all available predefined rule names
        /// </summary>
        public static IEnumerable<string> GetAvailableRules()
        {
            return _ruleCreators.Keys.OrderBy(k => k);
        }

        /// <summary>
        /// Gets information about all available rules
        /// </summary>
        public static IEnumerable<(string Name, string Description)> GetAvailableRuleInfo()
        {
            foreach (var (name, creator) in _ruleCreators)
            {
                var rules = creator();
                yield return (name, rules.GetRuleDescription());
            }
        }

        /// <summary>
        /// Registers a custom rule set
        /// </summary>
        public static void RegisterCustomRule(string name, Func<ICellularAutomatonRules> ruleCreator)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Rule name cannot be null or empty", nameof(name));
            
            if (ruleCreator == null)
                throw new ArgumentNullException(nameof(ruleCreator));

            _ruleCreators[name.ToLowerInvariant()] = ruleCreator;
        }

        private static int[] ParseNumbers(string numberString)
        {
            if (string.IsNullOrEmpty(numberString))
                return Array.Empty<int>();

            return numberString
                .Where(char.IsDigit)
                .Select(c => int.Parse(c.ToString()))
                .ToArray();
        }
    }
}