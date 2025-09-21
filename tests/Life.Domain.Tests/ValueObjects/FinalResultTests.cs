using FluentAssertions;
using Life.Domain.ValueObjects;
using Life.Domain.Aggregates;

namespace Life.Domain.Tests.ValueObjects
{
    public class FinalResultTests
    {
        [Fact]
        public void FinalResult_Should_Initialize_With_Correct_Values_For_Stable_State()
        {
            // Arrange
            var board = new Board(new BoardDimensions(5, 5), Array.Empty<Position>());
            
            // Act
            var finalResult = new FinalResult(board, true, false, 100, null);

            // Assert
            finalResult.Board.Should().Be(board);
            finalResult.Stable.Should().BeTrue();
            finalResult.Cyclic.Should().BeFalse();
            finalResult.CycleLength.Should().BeNull();
            finalResult.Iterations.Should().Be(100);
        }

        [Fact]
        public void FinalResult_Should_Initialize_With_Correct_Values_For_Cyclic_State()
        {
            // Arrange
            var board = new Board(new BoardDimensions(5, 5), Array.Empty<Position>());
            
            // Act
            var finalResult = new FinalResult(board, false, true, 50, 3);

            // Assert
            finalResult.Board.Should().Be(board);
            finalResult.Stable.Should().BeFalse();
            finalResult.Cyclic.Should().BeTrue();
            finalResult.CycleLength.Should().Be(3);
            finalResult.Iterations.Should().Be(50);
        }
    }
}
