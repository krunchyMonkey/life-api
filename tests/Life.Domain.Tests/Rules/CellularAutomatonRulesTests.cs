using FluentAssertions;
using Life.Domain.Rules;
using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Tests.Rules
{
    public class CellularAutomatonRulesTests
    {
        /// <summary>
        /// Concrete test implementation to access protected methods
        /// </summary>
        private class TestRules : CellularAutomatonRulesBase
        {
            private readonly bool _shouldBeAlive;
            private readonly string _ruleSetId;
            private readonly string _description;
            private readonly int[] _birthCounts;
            private readonly int[] _survivalCounts;

            public TestRules(bool shouldBeAlive, string ruleSetId = "Test", string description = "Test rules", 
                int[]? birthCounts = null, int[]? survivalCounts = null)
            {
                _shouldBeAlive = shouldBeAlive;
                _ruleSetId = ruleSetId;
                _description = description;
                _birthCounts = birthCounts ?? new int[0];
                _survivalCounts = survivalCounts ?? new int[0];
            }

            public override string RuleSetId => _ruleSetId;

            public override bool ShouldCellBeAlive(Board board, Position position, bool isCurrentlyAlive, int liveNeighbors)
            {
                return _shouldBeAlive;
            }

            public override string GetRuleDescription()
            {
                return _description;
            }

            /// <summary>
            /// Public wrapper to test the protected CheckSurvivalCondition method
            /// </summary>
            public bool TestCheckSurvivalCondition(bool isCurrentlyAlive, int liveNeighbors, 
                int[] birthCounts, int[] survivalCounts)
            {
                return CheckSurvivalCondition(isCurrentlyAlive, liveNeighbors, birthCounts, survivalCounts);
            }
        }

        [Fact]
        public void CellularAutomatonRulesBase_Should_Have_Required_Properties()
        {
            // Arrange & Act
            var rules = new TestRules(true, "TestRuleSet", "Test description");

            // Assert
            rules.RuleSetId.Should().Be("TestRuleSet");
            rules.GetRuleDescription().Should().Be("Test description");
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Apply_Birth_Rules_For_Dead_Cells()
        {
            // Arrange
            var rules = new TestRules(true);
            var birthCounts = new[] { 3 };
            var survivalCounts = new[] { 2, 3 };

            // Act & Assert - Dead cell with 3 neighbors should be born
            rules.TestCheckSurvivalCondition(false, 3, birthCounts, survivalCounts).Should().BeTrue();
            
            // Act & Assert - Dead cell with 2 neighbors should stay dead
            rules.TestCheckSurvivalCondition(false, 2, birthCounts, survivalCounts).Should().BeFalse();
            
            // Act & Assert - Dead cell with 1 neighbor should stay dead
            rules.TestCheckSurvivalCondition(false, 1, birthCounts, survivalCounts).Should().BeFalse();
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Apply_Survival_Rules_For_Living_Cells()
        {
            // Arrange
            var rules = new TestRules(true);
            var birthCounts = new[] { 3 };
            var survivalCounts = new[] { 2, 3 };

            // Act & Assert - Living cell with 2 neighbors should survive
            rules.TestCheckSurvivalCondition(true, 2, birthCounts, survivalCounts).Should().BeTrue();
            
            // Act & Assert - Living cell with 3 neighbors should survive
            rules.TestCheckSurvivalCondition(true, 3, birthCounts, survivalCounts).Should().BeTrue();
            
            // Act & Assert - Living cell with 1 neighbor should die
            rules.TestCheckSurvivalCondition(true, 1, birthCounts, survivalCounts).Should().BeFalse();
            
            // Act & Assert - Living cell with 4 neighbors should die
            rules.TestCheckSurvivalCondition(true, 4, birthCounts, survivalCounts).Should().BeFalse();
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Handle_Empty_Birth_Rules()
        {
            // Arrange - No birth conditions (like Seeds rules)
            var rules = new TestRules(true);
            var birthCounts = Array.Empty<int>();
            var survivalCounts = new[] { 2, 3 };

            // Act & Assert - Dead cells should never be born
            for (int neighbors = 0; neighbors <= 8; neighbors++)
            {
                rules.TestCheckSurvivalCondition(false, neighbors, birthCounts, survivalCounts).Should().BeFalse();
            }
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Handle_Empty_Survival_Rules()
        {
            // Arrange - No survival conditions (like Seeds rules)
            var rules = new TestRules(true);
            var birthCounts = new[] { 2 };
            var survivalCounts = Array.Empty<int>();

            // Act & Assert - Living cells should always die
            for (int neighbors = 0; neighbors <= 8; neighbors++)
            {
                rules.TestCheckSurvivalCondition(true, neighbors, birthCounts, survivalCounts).Should().BeFalse();
            }
            
            // Act & Assert - Dead cell with 2 neighbors should be born
            rules.TestCheckSurvivalCondition(false, 2, birthCounts, survivalCounts).Should().BeTrue();
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Handle_Complex_Rules()
        {
            // Arrange - Complex rules like Day & Night
            var rules = new TestRules(true);
            var birthCounts = new[] { 3, 6, 7, 8 };
            var survivalCounts = new[] { 3, 4, 6, 7, 8 };

            // Act & Assert - Test various neighbor counts
            rules.TestCheckSurvivalCondition(false, 6, birthCounts, survivalCounts).Should().BeTrue(); // Birth
            rules.TestCheckSurvivalCondition(true, 4, birthCounts, survivalCounts).Should().BeTrue(); // Survival
            rules.TestCheckSurvivalCondition(false, 2, birthCounts, survivalCounts).Should().BeFalse(); // No birth
            rules.TestCheckSurvivalCondition(true, 1, birthCounts, survivalCounts).Should().BeFalse(); // Death
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Handle_All_Neighbor_Counts()
        {
            // Arrange - Rules that cover all possible neighbor counts
            var rules = new TestRules(true);
            var birthCounts = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            var survivalCounts = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            // Act & Assert - All cells should always be alive
            for (int neighbors = 0; neighbors <= 8; neighbors++)
            {
                rules.TestCheckSurvivalCondition(false, neighbors, birthCounts, survivalCounts).Should().BeTrue();
                rules.TestCheckSurvivalCondition(true, neighbors, birthCounts, survivalCounts).Should().BeTrue();
            }
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Handle_Single_Count_Rules()
        {
            // Arrange
            var rules = new TestRules(true);
            var birthCounts = new[] { 3 };
            var survivalCounts = new[] { 2 };

            // Act & Assert
            rules.TestCheckSurvivalCondition(false, 3, birthCounts, survivalCounts).Should().BeTrue();
            rules.TestCheckSurvivalCondition(false, 2, birthCounts, survivalCounts).Should().BeFalse();
            
            rules.TestCheckSurvivalCondition(true, 2, birthCounts, survivalCounts).Should().BeTrue();
            rules.TestCheckSurvivalCondition(true, 3, birthCounts, survivalCounts).Should().BeFalse();
        }

        [Fact]
        public void TestRules_Should_Implement_Abstract_Methods_Correctly()
        {
            // Arrange
            var aliveRules = new TestRules(true, "AliveTest", "Always alive");
            var deadRules = new TestRules(false, "DeadTest", "Always dead");
            var dimensions = new BoardDimensions(3, 3);
            var dummyBoard = new Board(dimensions, new[] { new Position(1, 1) });
            var position = new Position(1, 1);

            // Act & Assert - Test that our TestRules correctly implement the interface
            aliveRules.ShouldCellBeAlive(dummyBoard, position, true, 2).Should().BeTrue();
            aliveRules.ShouldCellBeAlive(dummyBoard, position, false, 3).Should().BeTrue();
            
            deadRules.ShouldCellBeAlive(dummyBoard, position, true, 2).Should().BeFalse();
            deadRules.ShouldCellBeAlive(dummyBoard, position, false, 3).Should().BeFalse();
        }

        [Fact]
        public void CheckSurvivalCondition_Should_Handle_Edge_Cases()
        {
            // Arrange
            var rules = new TestRules(true);
            
            // Test with null arrays (should not cause exceptions)
            var emptyBirth = Array.Empty<int>();
            var emptySurvival = Array.Empty<int>();
            
            // Act & Assert - Edge cases should be handled gracefully
            rules.TestCheckSurvivalCondition(false, 0, emptyBirth, emptySurvival).Should().BeFalse();
            rules.TestCheckSurvivalCondition(true, 8, emptyBirth, emptySurvival).Should().BeFalse();
            
            // Test boundary neighbor counts
            rules.TestCheckSurvivalCondition(false, 0, new[] { 0 }, new[] { 0 }).Should().BeTrue();
            rules.TestCheckSurvivalCondition(true, 0, new[] { 0 }, new[] { 0 }).Should().BeTrue();
            rules.TestCheckSurvivalCondition(false, 8, new[] { 8 }, new[] { 8 }).Should().BeTrue();
            rules.TestCheckSurvivalCondition(true, 8, new[] { 8 }, new[] { 8 }).Should().BeTrue();
        }
    }
}