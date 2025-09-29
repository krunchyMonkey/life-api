using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service for complex GameSession operations
    /// </summary>
    public static class GameSessionService
    {
        /// <summary>
        /// Creates a new game session from a known pattern
        /// </summary>
        public static GameSession CreateFromPattern(
            string sessionName,
            string createdBy,
            BoardDimensions dimensions,
            PatternType pattern,
            Position? centerPosition = null,
            SessionSettings? settings = null)
        {
            var center = centerPosition ?? new Position(dimensions.Width / 2, dimensions.Height / 2);
            var patternCells = GeneratePatternCells(pattern, center);
            
            var board = new Board(dimensions, patternCells);
            return new GameSession(sessionName, board, createdBy, settings);
        }

        /// <summary>
        /// Merges two game sessions by combining their current board states
        /// </summary>
        public static GameSession MergeSessions(
            GameSession session1,
            GameSession session2,
            string newSessionName,
            string createdBy,
            Position session2Offset,
            SessionSettings? settings = null)
        {
            ValidateSessionsForMerging(session1, session2);
            
            var mergedDimensions = CalculateMergedDimensions(
                session1.Dimensions, 
                session2.Dimensions, 
                session2Offset);
            
            var mergedCells = MergeBoardStates(
                session1.GetCurrentBoard(),
                session2.GetCurrentBoard(),
                session2Offset);
            
            var mergedBoard = new Board(mergedDimensions, mergedCells);
            return new GameSession(newSessionName, mergedBoard, createdBy, settings);
        }

        /// <summary>
        /// Compares session evolution patterns for similarity
        /// </summary>
        public static SessionComparisonResult CompareSessionEvolution(
            GameSession session1,
            GameSession session2,
            int generationsToCompare = 10)
        {
            var board1 = session1.GetCurrentBoard();
            var board2 = session2.GetCurrentBoard();
            
            var similarityScores = new List<double>();
            
            for (int i = 0; i < generationsToCompare; i++)
            {
                var similarity = CalculateBoardSimilarity(board1, board2);
                similarityScores.Add(similarity);
                
                board1 = board1.NextGeneration();
                board2 = board2.NextGeneration();
            }
            
            return new SessionComparisonResult(
                Session1Id: session1.Id,
                Session2Id: session2.Id,
                GenerationsCompared: generationsToCompare,
                AverageSimilarity: similarityScores.Average(),
                SimilarityScores: similarityScores.AsReadOnly(),
                MaxSimilarity: similarityScores.Max(),
                MinSimilarity: similarityScores.Min()
            );
        }

        /// <summary>
        /// Analyzes session for interesting patterns or behaviors
        /// </summary>
        public static SessionAnalysis AnalyzeSession(GameSession session)
        {
            var currentBoard = session.GetCurrentBoard();
            var statistics = session.GetStatistics();
            
            var patterns = new List<DetectedPattern>();
            
            // Detect common patterns
            patterns.AddRange(DetectOscillators(currentBoard));
            patterns.AddRange(DetectStillLifes(currentBoard));
            patterns.AddRange(DetectGliders(currentBoard));
            
            return new SessionAnalysis(
                SessionId: session.Id,
                GenerationAnalyzed: session.CurrentGeneration,
                PopulationTrend: AnalyzePopulationTrend(session),
                DetectedPatterns: patterns.AsReadOnly(),
                StabilityIndex: CalculateStabilityIndex(session),
                ComplexityScore: CalculateComplexityScore(currentBoard)
            );
        }

        #region Private Helper Methods

        private static IEnumerable<Position> GeneratePatternCells(PatternType pattern, Position center)
        {
            return pattern switch
            {
                PatternType.Glider => GenerateGlider(center),
                PatternType.Block => GenerateBlock(center),
                PatternType.Blinker => GenerateBlinker(center),
                PatternType.Toad => GenerateToad(center),
                PatternType.Beacon => GenerateBeacon(center),
                PatternType.RandomSoup => GenerateRandomSoup(center, 10),
                _ => throw new ArgumentOutOfRangeException(nameof(pattern))
            };
        }

        private static IEnumerable<Position> GenerateGlider(Position center)
        {
            // Classic glider pattern
            yield return new Position(center.X, center.Y);
            yield return new Position(center.X + 1, center.Y + 1);
            yield return new Position(center.X - 1, center.Y + 2);
            yield return new Position(center.X, center.Y + 2);
            yield return new Position(center.X + 1, center.Y + 2);
        }

        private static IEnumerable<Position> GenerateBlock(Position center)
        {
            // 2x2 block (still life)
            yield return center;
            yield return new Position(center.X + 1, center.Y);
            yield return new Position(center.X, center.Y + 1);
            yield return new Position(center.X + 1, center.Y + 1);
        }

        private static IEnumerable<Position> GenerateBlinker(Position center)
        {
            // Vertical blinker (period-2 oscillator)
            yield return new Position(center.X, center.Y - 1);
            yield return center;
            yield return new Position(center.X, center.Y + 1);
        }

        private static IEnumerable<Position> GenerateToad(Position center)
        {
            // Toad pattern (period-2 oscillator)
            yield return new Position(center.X, center.Y - 1);
            yield return new Position(center.X + 1, center.Y - 1);
            yield return new Position(center.X + 2, center.Y - 1);
            yield return new Position(center.X - 1, center.Y);
            yield return new Position(center.X, center.Y);
            yield return new Position(center.X + 1, center.Y);
        }

        private static IEnumerable<Position> GenerateBeacon(Position center)
        {
            // Beacon pattern (period-2 oscillator)
            yield return center;
            yield return new Position(center.X + 1, center.Y);
            yield return new Position(center.X, center.Y + 1);
            yield return new Position(center.X + 3, center.Y + 2);
            yield return new Position(center.X + 2, center.Y + 3);
            yield return new Position(center.X + 3, center.Y + 3);
        }

        private static IEnumerable<Position> GenerateRandomSoup(Position center, int density)
        {
            var random = new Random();
            var positions = new List<Position>();
            
            for (int dx = -5; dx <= 5; dx++)
            {
                for (int dy = -5; dy <= 5; dy++)
                {
                    if (random.Next(100) < density)
                    {
                        positions.Add(new Position(center.X + dx, center.Y + dy));
                    }
                }
            }
            
            return positions;
        }

        private static void ValidateSessionsForMerging(GameSession session1, GameSession session2)
        {
            if (session1.Status != SessionStatus.Active || session2.Status != SessionStatus.Active)
                throw new InvalidOperationException("Both sessions must be active for merging");
        }

        private static BoardDimensions CalculateMergedDimensions(
            BoardDimensions dims1,
            BoardDimensions dims2,
            Position offset)
        {
            var maxWidth = Math.Max(dims1.Width, dims2.Width + offset.X);
            var maxHeight = Math.Max(dims1.Height, dims2.Height + offset.Y);
            
            return new BoardDimensions(maxWidth, maxHeight);
        }

        private static IEnumerable<Position> MergeBoardStates(
            Board board1,
            Board board2,
            Position offset)
        {
            var mergedPositions = new HashSet<Position>();
            
            // Add all positions from board1
            mergedPositions.UnionWith(board1.AlivePositions());
            
            // Add offset positions from board2
            var offsetPositions = board2.AlivePositions()
                .Select(p => new Position(p.X + offset.X, p.Y + offset.Y));
            mergedPositions.UnionWith(offsetPositions);
            
            return mergedPositions;
        }

        private static double CalculateBoardSimilarity(Board board1, Board board2)
        {
            if (!board1.Dimensions.Equals(board2.Dimensions))
                return 0.0;
            
            var alive1 = board1.AlivePositions().ToHashSet();
            var alive2 = board2.AlivePositions().ToHashSet();
            
            var intersection = alive1.Intersect(alive2).Count();
            var union = alive1.Union(alive2).Count();
            
            return union == 0 ? 1.0 : (double)intersection / union; // Jaccard similarity
        }

        private static List<DetectedPattern> DetectOscillators(Board board)
        {
            // Simplified pattern detection - in real implementation, 
            // this would be much more sophisticated
            var patterns = new List<DetectedPattern>();
            
            // Detect blinkers (3 consecutive alive cells in a line)
            foreach (var position in board.AlivePositions())
            {
                if (IsBlinkerPattern(board, position))
                {
                    patterns.Add(new DetectedPattern("Blinker", position, 2));
                }
            }
            
            return patterns;
        }

        private static List<DetectedPattern> DetectStillLifes(Board board)
        {
            var patterns = new List<DetectedPattern>();
            
            // Detect blocks (2x2 squares)
            foreach (var position in board.AlivePositions())
            {
                if (IsBlockPattern(board, position))
                {
                    patterns.Add(new DetectedPattern("Block", position, 1));
                }
            }
            
            return patterns;
        }

        private static List<DetectedPattern> DetectGliders(Board board)
        {
            var patterns = new List<DetectedPattern>();
            
            // Simplified glider detection
            foreach (var position in board.AlivePositions())
            {
                if (IsGliderPattern(board, position))
                {
                    patterns.Add(new DetectedPattern("Glider", position, 4));
                }
            }
            
            return patterns;
        }

        private static bool IsBlinkerPattern(Board board, Position center)
        {
            // Check for vertical blinker
            return board.Get(center.X, center.Y - 1) &&
                   board.Get(center.X, center.Y) &&
                   board.Get(center.X, center.Y + 1);
        }

        private static bool IsBlockPattern(Board board, Position topLeft)
        {
            return board.Get(topLeft.X, topLeft.Y) &&
                   board.Get(topLeft.X + 1, topLeft.Y) &&
                   board.Get(topLeft.X, topLeft.Y + 1) &&
                   board.Get(topLeft.X + 1, topLeft.Y + 1);
        }

        private static bool IsGliderPattern(Board board, Position center)
        {
            // Simplified glider detection (one of four orientations)
            return board.Get(center.X, center.Y) &&
                   board.Get(center.X + 1, center.Y + 1) &&
                   board.Get(center.X - 1, center.Y + 2) &&
                   board.Get(center.X, center.Y + 2) &&
                   board.Get(center.X + 1, center.Y + 2);
        }

        private static PopulationTrend AnalyzePopulationTrend(GameSession session)
        {
            // This would analyze snapshot data to determine population trends
            // Simplified implementation
            var currentPop = session.GetCurrentBoard().Alive().Count();
            
            if (currentPop == 0) return PopulationTrend.Extinct;
            if (session.CurrentGeneration < 10) return PopulationTrend.Growing;
            
            return PopulationTrend.Stable;
        }

        private static double CalculateStabilityIndex(GameSession session)
        {
            // Calculate how stable the population has been
            // This would use snapshot data in a real implementation
            return 0.5; // Placeholder
        }

        private static int CalculateComplexityScore(Board board)
        {
            // Calculate complexity based on pattern density and distribution
            var alive = board.Alive().ToList();
            if (alive.Count == 0) return 0;
            
            // Simple complexity metric based on population and spatial distribution
            var maxX = alive.Max(p => p.x);
            var minX = alive.Min(p => p.x);
            var maxY = alive.Max(p => p.y);
            var minY = alive.Min(p => p.y);
            
            var area = (maxX - minX + 1) * (maxY - minY + 1);
            var density = (double)alive.Count / area;
            
            return (int)(alive.Count * density * 10);
        }

        #endregion
    }

    #region Supporting Types

    public enum PatternType
    {
        Glider,
        Block,
        Blinker,
        Toad,
        Beacon,
        RandomSoup
    }

    public enum PopulationTrend
    {
        Growing,
        Declining,
        Stable,
        Oscillating,
        Extinct
    }

    public sealed record SessionComparisonResult(
        Guid Session1Id,
        Guid Session2Id,
        int GenerationsCompared,
        double AverageSimilarity,
        IReadOnlyList<double> SimilarityScores,
        double MaxSimilarity,
        double MinSimilarity);

    public sealed record SessionAnalysis(
        Guid SessionId,
        int GenerationAnalyzed,
        PopulationTrend PopulationTrend,
        IReadOnlyList<DetectedPattern> DetectedPatterns,
        double StabilityIndex,
        int ComplexityScore);

    public sealed record DetectedPattern(
        string Name,
        Position Location,
        int Period);

    #endregion
}