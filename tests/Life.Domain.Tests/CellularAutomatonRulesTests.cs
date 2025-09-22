using FluentAssertions;
using Life.Domain.Aggregates;
using Life.Domain.Rules;
using Life.Domain.Rules.Standard;
using Life.Domain.Rules.Custom;
using Life.Domain.Services;

namespace Life.Domain.Tests
{
    public class CellularAutomatonRulesTests
    {
        [Fact]
        public void ConwaysRules_Should_Apply_Standard_GameOfLife_Rules()
        {
            // Arrange - Block pattern (stable under Conway's rules)
            var board = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });
            var rules = new ConwaysGameOfLifeRules();

            // Act
            var nextBoard = board.GenerateNextGeneration(rules);

            // Assert - Block should remain stable
            board.IsIdenticalTo(nextBoard).Should().BeTrue();
        }

        [Fact]
        public void HighLifeRules_Should_Create_Different_Patterns_Than_Conway()
        {
            // Arrange - Pattern that behaves differently under HighLife vs Conway
            var board = new Board(7, 7, new[] { (2, 2), (2, 3), (2, 4), (3, 2), (4, 2) });
            
            var conwayRules = new ConwaysGameOfLifeRules();
            var highLifeRules = new HighLifeRules();

            // Act
            var conwayResult = board.GenerateNextGeneration(conwayRules);
            var highLifeResult = board.GenerateNextGeneration(highLifeRules);

            // Assert - Results should be different due to B36 vs B3
            conwayResult.IsIdenticalTo(highLifeResult).Should().BeFalse();
        }

        [Fact]
        public void SeedsRules_Should_Kill_All_Cells_Each_Generation()
        {
            // Arrange - Any living pattern
            var board = new Board(5, 5, new[] { (2, 2), (2, 3), (3, 2), (3, 3) });
            var seedsRules = new SeedsRules();

            // Act - Two generations (first creates new cells, second kills them all)
            var gen1 = board.GenerateNextGeneration(seedsRules);
            var gen2 = gen1.GenerateNextGeneration(seedsRules);

            // Assert - All cells should die every generation
            gen2.HasAliveCells().Should().BeFalse();
        }

        [Fact]
        public void CustomRules_Should_Work_With_Arbitrary_Birth_Survival_Conditions()
        {
            // Arrange - Create custom rules: B1/S12 (very aggressive growth)
            var customRules = new CustomRules(new[] { 1 }, new[] { 1, 2 });
            var board = new Board(5, 5, new[] { (2, 2) });

            // Act
            var nextBoard = board.GenerateNextGeneration(customRules);

            // Assert - Should create more cells due to B1 rule
            nextBoard.GetAliveCellCount().Should().BeGreaterThan(board.GetAliveCellCount());
        }

        [Fact]
        public void RuleFactory_Should_Create_Rules_By_Name()
        {
            // Arrange & Act
            var conwayRules = CellularAutomatonRuleFactory.CreateRules("conway");
            var highLifeRules = CellularAutomatonRuleFactory.CreateRules("HighLife");
            var dayNightRules = CellularAutomatonRuleFactory.CreateRules("DAYNIGHT");

            // Assert
            conwayRules.RuleSetId.Should().Contain("Conway");
            highLifeRules.RuleSetId.Should().Contain("HighLife");
            dayNightRules.RuleSetId.Should().Contain("DayNight");
        }

        [Fact]
        public void RuleFactory_Should_Create_Rules_From_Notation()
        {
            // Arrange & Act
            var b3s23 = CellularAutomatonRuleFactory.CreateFromNotation("B3/S23");
            var b36s23 = CellularAutomatonRuleFactory.CreateFromNotation("B36/S23");

            // Assert
            b3s23.RuleSetId.Should().Contain("B3");
            b3s23.RuleSetId.Should().Contain("S23");
            
            b36s23.RuleSetId.Should().Contain("B36");
            b36s23.RuleSetId.Should().Contain("S23");
        }

        [Fact]
        public void FluentAPI_Should_Allow_Rule_Based_Board_Operations()
        {
            // Arrange
            var board = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) }); // Blinker

            // Act - Use fluent API with different rules
            var conwayResult = board.WithRules("conway").NextGeneration();
            var highLifeResult = board.WithRules("highlife").NextGeneration();
            var customResult = board.WithRuleNotation("B36/S23").NextGeneration();

            // Assert
            conwayResult.Should().NotBeNull();
            highLifeResult.Should().NotBeNull();
            customResult.Should().NotBeNull();
            
            // HighLife and custom B36/S23 should be identical
            highLifeResult.IsIdenticalTo(customResult).Should().BeTrue();
        }

        [Fact]
        public void BoardEvolutionAnalysis_Should_Track_Population_Changes()
        {
            // Arrange
            var board = new Board(10, 10, new[] { (4, 4), (4, 5), (5, 4), (5, 5), (6, 6) }); // Block + single cell
            var rules = new ConwaysGameOfLifeRules();

            // Act
            var analysis = board.AnalyzeEvolution(5, rules);

            // Assert
            analysis.GenerationsSimulated.Should().Be(5);
            analysis.PopulationHistory.Should().HaveCount(6); // Initial + 5 generations
            analysis.StartingPopulation.Should().Be(5);
            analysis.RuleSetUsed.Should().Contain("Conway");
        }

        [Fact]
        public void RuleComparison_Should_Show_Differences_Between_Rule_Sets()
        {
            // Arrange
            var board = new Board(7, 7, new[] { (2, 2), (2, 3), (2, 4), (3, 2), (4, 2) });
            var conwayRules = new ConwaysGameOfLifeRules();
            var highLifeRules = new HighLifeRules();
            var seedsRules = new SeedsRules();

            // Act
            var comparison = board.CompareRules(10, conwayRules, highLifeRules, seedsRules);

            // Assert
            comparison.Results.Should().HaveCount(3);
            comparison.Results.Should().ContainKey(conwayRules.RuleSetId);
            comparison.Results.Should().ContainKey(highLifeRules.RuleSetId);
            comparison.Results.Should().ContainKey(seedsRules.RuleSetId);
            
            // Seeds rules should result in extinction
            comparison.Results[seedsRules.RuleSetId].PopulationExtinct.Should().BeTrue();
        }

        [Fact]
        public void Board_Should_Maintain_Backward_Compatibility_With_Default_Rules()
        {
            // Arrange - This should work exactly like before
            var board = new Board(3, 3, new[] { (1, 0), (1, 1), (1, 2) });

            // Act - Using original methods without specifying rules
            var nextGen = board.NextGeneration();
            var advanced = board.AdvanceGenerations(2);
            var finalState = board.DetectFinalState(10, TimeSpan.FromSeconds(1));

            // Assert - Should work with Conway's rules by default
            nextGen.Should().NotBeNull();
            advanced.Should().NotBeNull();
            finalState.Should().NotBeNull();
            finalState.Cyclic.Should().BeTrue(); // Blinker is cyclic
        }

        [Fact]
        public void CustomRules_Should_Generate_Proper_Notation()
        {
            // Arrange & Act
            var rules1 = new CustomRules(new[] { 3 }, new[] { 2, 3 });
            var rules2 = new CustomRules(new[] { 3, 6 }, new[] { 2, 3 }, "MyCustomRules", "Custom description");

            // Assert
            rules1.RuleSetId.Should().Contain("B3");
            rules1.RuleSetId.Should().Contain("S23");
            
            rules2.RuleSetId.Should().Be("MyCustomRules");
            rules2.GetRuleDescription().Should().Be("Custom description");
        }
    }
}