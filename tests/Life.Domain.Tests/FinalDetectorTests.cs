using FluentAssertions;
using Life.Domain.Aggregates;

namespace Life.Domain.Tests
{
    public class FinalDetectorTests
    {
        [Fact]
        public void DetectFinalState_Should_Detect_Stable_Pattern()
        {
            // Arrange - Block pattern (stable)
            var stableBoard = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act - Use Board's DetectFinalState method directly
            var result = stableBoard.DetectFinalState(10, TimeSpan.FromSeconds(1));

            // Assert
            result.Should().NotBeNull();
            result.Stable.Should().BeTrue();
            result.Cyclic.Should().BeFalse();
            result.Iterations.Should().Be(1);
        }

        [Fact]
        public void DetectFinalState_Should_Detect_Oscillating_Pattern()
        {
            // Arrange - Blinker pattern (oscillates with period 2)
            var blinkerBoard = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act - Use Board's DetectFinalState method directly
            var result = blinkerBoard.DetectFinalState(10, TimeSpan.FromSeconds(1));

            // Assert
            result.Should().NotBeNull();
            result.Stable.Should().BeFalse();
            result.Cyclic.Should().BeTrue();
            result.CycleLength.Should().Be(2);
        }

        [Fact]
        public void DetectFinalState_Should_Timeout_With_Max_Iterations()
        {
            // Arrange - Create a complex pattern that might not stabilize quickly
            var complexBoard = new Board(10, 10, new[] { (1, 1), (2, 2), (3, 3), (4, 4), (5, 5) });

            // Act & Assert - Should throw timeout exception with low iteration limit
            var action = () => complexBoard.DetectFinalState(1, TimeSpan.FromSeconds(10));
            action.Should().Throw<TimeoutException>();
        }
    }
}
