using FluentAssertions;
using Life.Domain.Aggregates;
using Life.Domain.Specifications;
using Life.Domain.Services;

namespace Life.Domain.Tests
{
    public class DomainServicesTests
    {
        [Fact]
        public void CellSurvivalRules_Should_Apply_Conway_Rules_Correctly()
        {
            // Arrange - Block pattern (stable)
            var board = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act & Assert - All block cells should survive
            CellSurvivalRules.ShouldCellBeAlive(board, 1, 1).Should().BeTrue();
            CellSurvivalRules.ShouldCellBeAlive(board, 1, 2).Should().BeTrue();
            CellSurvivalRules.ShouldCellBeAlive(board, 2, 1).Should().BeTrue();
            CellSurvivalRules.ShouldCellBeAlive(board, 2, 2).Should().BeTrue();
            
            // Empty corners should stay empty
            CellSurvivalRules.ShouldCellBeAlive(board, 0, 0).Should().BeFalse();
            CellSurvivalRules.ShouldCellBeAlive(board, 3, 3).Should().BeFalse();
        }

        [Fact]
        public void BoardIdentityComparison_Should_Correctly_Compare_Boards()
        {
            // Arrange
            var board1 = new Board(3, 3, new[] { (1, 1), (1, 2) });
            var board2 = new Board(3, 3, new[] { (1, 1), (1, 2) });
            var board3 = new Board(3, 3, new[] { (1, 1), (2, 2) });

            // Act & Assert
            BoardIdentityComparison.AreIdentical(board1, board2).Should().BeTrue();
            BoardIdentityComparison.AreIdentical(board1, board3).Should().BeFalse();
        }

        [Fact]
        public void Board_Should_Generate_Correct_Next_Generation()
        {
            // Arrange - Classic blinker pattern
            var board = new Board(3, 3, new[] { (1, 0), (1, 1), (1, 2) });

            // Act
            var nextGen = board.NextGeneration();

            // Assert - Should become horizontal blinker
            var expectedAlive = new[] { (0, 1), (1, 1), (2, 1) };
            nextGen.Alive().OrderBy(c => c.x).ThenBy(c => c.y).Should().BeEquivalentTo(expectedAlive);
        }

        [Fact]
        public void Board_Should_Detect_Stable_State()
        {
            // Arrange - Block pattern (stable)
            var stableBoard = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act
            var result = stableBoard.DetectFinalState(10, TimeSpan.FromSeconds(1));

            // Assert
            result.Stable.Should().BeTrue();
            result.Cyclic.Should().BeFalse();
            result.Iterations.Should().Be(1);
        }

        [Fact]
        public void Board_Should_Handle_Edge_Cases()
        {
            // Act & Assert - Empty board should remain empty
            var emptyBoard = new Board(3, 3, Array.Empty<(int, int)>());
            var nextEmpty = emptyBoard.NextGeneration();
            nextEmpty.Alive().Should().BeEmpty();

            // Single cell should die (underpopulation)
            var singleCell = new Board(3, 3, new[] { (1, 1) });
            var nextSingle = singleCell.NextGeneration();
            nextSingle.Alive().Should().BeEmpty();
        }

        [Fact]
        public void Board_Should_Validate_Parameters_Using_Specifications()
        {
            // Act & Assert - Valid parameters should not throw
            var validAction = () => new Board(3, 3, new[] { (1, 1), (2, 2) });
            validAction.Should().NotThrow();

            // Invalid dimensions should throw
            var invalidDimensionsAction = () => new Board(-1, 3, new[] { (1, 1) });
            invalidDimensionsAction.Should().Throw<ArgumentOutOfRangeException>();

            // Out of bounds positions should throw
            var outOfBoundsAction = () => new Board(3, 3, new[] { (5, 5) });
            outOfBoundsAction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Board_Should_Advance_Multiple_Generations()
        {
            // Arrange - Blinker pattern
            var blinkerBoard = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act - Advance 2 generations (should return to original state)
            var advanced = blinkerBoard.AdvanceGenerations(2);

            // Assert - Should be back to original pattern
            var originalAlive = blinkerBoard.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            var advancedAlive = advanced.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            advancedAlive.Should().BeEquivalentTo(originalAlive);
        }

        [Fact] 
        public void ValidBoardConstructionSpecification_Should_Validate_Parameters()
        {
            // Arrange
            var validParams = new BoardConstructionParameters(3, 3, new[] { (1, 1), (2, 2) });
            var invalidDimensionsParams = new BoardConstructionParameters(-1, 3, new[] { (1, 1) });
            var outOfBoundsParams = new BoardConstructionParameters(3, 3, new[] { (5, 5) });
            
            var spec = new ValidBoardConstructionSpecification();

            // Act & Assert
            spec.IsSatisfiedBy(validParams).Should().BeTrue();
            spec.IsSatisfiedBy(invalidDimensionsParams).Should().BeFalse();
            spec.IsSatisfiedBy(outOfBoundsParams).Should().BeFalse();

            // Test detailed error reporting
            var errors = spec.GetValidationErrors(invalidDimensionsParams);
            errors.Should().NotBeEmpty();
            errors.First().Message.Should().Contain("dimensions must be positive");

            var outOfBoundsErrors = spec.GetValidationErrors(outOfBoundsParams);
            outOfBoundsErrors.Should().NotBeEmpty();
            outOfBoundsErrors.First().Message.Should().Contain("outside board bounds");
        }
    }
}