using FluentAssertions;
using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Tests.Services
{
    public class BoardGameplayTests
    {
        [Fact]
        public void NextGeneration_Should_Apply_Conway_Rules_For_Blinker()
        {
            // Arrange - Horizontal blinker pattern
            var board = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act
            var nextBoard = board.NextGeneration();

            // Assert - Should become vertical blinker
            var alive = nextBoard.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            alive.Should().BeEquivalentTo(new[] { (1, 2), (2, 2), (3, 2) });
        }

        [Fact]
        public void NextGeneration_Should_Preserve_Block_Pattern()
        {
            // Arrange - Block pattern (still life)
            var board = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act
            var nextBoard = board.NextGeneration();

            // Assert - Block should remain unchanged
            var alive = nextBoard.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            alive.Should().BeEquivalentTo(new[] { (1, 1), (1, 2), (2, 1), (2, 2) });
        }

        [Fact]
        public void NextGeneration_Should_Handle_Empty_Board()
        {
            // Arrange
            var board = new Board(3, 3, Array.Empty<(int, int)>());

            // Act
            var nextBoard = board.NextGeneration();

            // Assert - Should remain empty
            nextBoard.Alive().Should().BeEmpty();
        }

        [Fact]
        public void AdvanceGenerations_Should_Simulate_Multiple_Generations()
        {
            // Arrange
            var board = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) }); // Block

            // Act
            var advancedBoard = board.AdvanceGenerations(10);

            // Assert - Block should remain unchanged after 10 generations
            var alive = advancedBoard.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            alive.Should().BeEquivalentTo(new[] { (1, 1), (1, 2), (2, 1), (2, 2) });
        }

        [Fact]
        public void AdvanceGenerations_Should_Handle_Zero_Generations()
        {
            // Arrange
            var board = new Board(3, 3, new[] { (1, 1), (1, 2) });

            // Act
            var result = board.AdvanceGenerations(0);

            // Assert - Should return the same board state
            var alive = result.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            alive.Should().BeEquivalentTo(new[] { (1, 1), (1, 2) });
        }

        [Fact]
        public void DetectFinalState_Should_Detect_Stable_State()
        {
            // Arrange
            var board = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) }); // Block

            // Act
            var result = board.DetectFinalState(100, TimeSpan.FromSeconds(1));

            // Assert
            result.Stable.Should().BeTrue();
            result.Cyclic.Should().BeFalse();
            result.CycleLength.Should().BeNull();
            result.Iterations.Should().Be(1);
        }

        [Fact]
        public void DetectFinalState_Should_Detect_Cyclic_State()
        {
            // Arrange
            var board = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) }); // Blinker

            // Act
            var result = board.DetectFinalState(100, TimeSpan.FromSeconds(1));

            // Assert
            result.Cyclic.Should().BeTrue();
            result.Stable.Should().BeFalse();
            result.CycleLength.Should().Be(2);
        }

        [Fact]
        public void NextGeneration_Should_Apply_Birth_Rule()
        {
            // Arrange - Three cells in a row to create birth in middle
            var board = new Board(5, 5, new[] { (1, 1), (2, 1), (3, 1) });

            // Act
            var nextBoard = board.NextGeneration();

            // Assert - Cell (2,0) and (2,2) should be born
            nextBoard.Get(new Position(2, 0)).Should().BeTrue();
            nextBoard.Get(new Position(2, 2)).Should().BeTrue();
        }

        [Fact]
        public void NextGeneration_Should_Apply_Death_By_Isolation()
        {
            // Arrange - Single cell (should die)
            var board = new Board(3, 3, new[] { (1, 1) });

            // Act
            var nextBoard = board.NextGeneration();

            // Assert - Single cell should die
            nextBoard.Alive().Should().BeEmpty();
        }

        [Fact]
        public void NextGeneration_Should_Apply_Death_By_Overpopulation()
        {
            // Arrange - Cell with 4 neighbors (overpopulation)
            var board = new Board(5, 5, new[] { (2, 2), (1, 2), (3, 2), (2, 1), (2, 3), (1, 1) });

            // Act
            var nextBoard = board.NextGeneration();

            // Assert - Center cell should die due to overpopulation
            nextBoard.Get(new Position(2, 2)).Should().BeFalse();
        }

        [Fact]
        public void Board_Should_Handle_Blinker_Oscillation()
        {
            // Arrange
            var board = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act
            var generation1 = board.NextGeneration();
            var generation2 = generation1.NextGeneration();

            // Assert - Should return to original after 2 generations
            var originalAlive = board.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            var finalAlive = generation2.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            
            finalAlive.Should().BeEquivalentTo(originalAlive);
        }
    }
}
