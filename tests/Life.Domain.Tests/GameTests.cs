using FluentAssertions;
using Life.Domain.Aggregates;

namespace Life.Domain.Tests
{
    public class GameTests
    {
        [Fact]
        public void Board_Should_Progress_Through_Generations_Correctly()
        {
            // Arrange - Classic blinker pattern (oscillates)
            var initialBoard = new Board(3, 3, new[] { (1, 0), (1, 1), (1, 2) });

            // Act - Generate next generation using Board directly
            var generation1 = initialBoard.NextGeneration();
            var generation2 = generation1.NextGeneration();

            // Assert - Should oscillate back to original pattern
            var initialAlive = initialBoard.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            var gen2Alive = generation2.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();

            gen2Alive.Should().BeEquivalentTo(initialAlive);
        }

        [Fact]
        public void Board_Should_Handle_Multiple_Generation_Advancement()
        {
            // Arrange - Blinker pattern
            var initialBoard = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act - Advance 4 generations (blinker has period 2, so should return to original after even steps)
            var advanced = initialBoard.AdvanceGenerations(4);

            // Assert - Should be back to original state after 4 generations
            var originalAlive = initialBoard.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            var advancedAlive = advanced.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();

            advancedAlive.Should().BeEquivalentTo(originalAlive);
        }

        [Fact]
        public void Board_Should_Detect_Stable_Patterns()
        {
            // Arrange - Block pattern (stable)
            var stableBoard = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act - Detect final state
            var result = stableBoard.DetectFinalState(10, TimeSpan.FromSeconds(1));

            // Assert - Should detect stable state
            result.Stable.Should().BeTrue();
            result.Cyclic.Should().BeFalse();
            result.Iterations.Should().Be(1);
        }
    }
}
