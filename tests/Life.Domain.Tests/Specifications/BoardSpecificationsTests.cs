using FluentAssertions;
using Life.Domain.Aggregates;
using Life.Domain.Specifications;
using Life.Domain.Rules.Standard;

namespace Life.Domain.Tests.Specifications
{
    public class BoardSpecificationsTests
    {
        [Fact]
        public void CellSurvivalRules_ShouldCellBeAlive_Should_Apply_Conway_Rules()
        {
            // Arrange - Create a board with a blinker pattern (3 horizontal cells)
            var board = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act & Assert - Test center cell of blinker (should survive with 2 neighbors)
            CellSurvivalRules.ShouldCellBeAlive(board, 2, 2).Should().BeTrue();

            // Act & Assert - Test cell above center (should be born with 3 neighbors)
            CellSurvivalRules.ShouldCellBeAlive(board, 1, 2).Should().BeTrue();

            // Act & Assert - Test corner cell (should remain dead with 0 neighbors)
            CellSurvivalRules.ShouldCellBeAlive(board, 0, 0).Should().BeFalse();

            // Act & Assert - Test edge cell (should remain dead with 1 neighbor)
            CellSurvivalRules.ShouldCellBeAlive(board, 1, 1).Should().BeFalse();
        }

        [Fact]
        public void CellSurvivalRules_Should_Handle_Edge_Cases()
        {
            // Arrange - Empty board
            var emptyBoard = new Board(3, 3, Array.Empty<(int, int)>());

            // Act & Assert - All cells should remain dead
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    CellSurvivalRules.ShouldCellBeAlive(emptyBoard, x, y).Should().BeFalse();
                }
            }
        }

        [Fact]
        public void CellSurvivalRules_Should_Handle_Full_Board()
        {
            // Arrange - Full 3x3 board (all cells alive)
            var fullBoard = new Board(3, 3, new[] 
            {
                (0, 0), (0, 1), (0, 2),
                (1, 0), (1, 1), (1, 2),
                (2, 0), (2, 1), (2, 2)
            });

            // Act & Assert - Corner cells should die (overcrowding with 3 neighbors)
            CellSurvivalRules.ShouldCellBeAlive(fullBoard, 0, 0).Should().BeFalse();
            CellSurvivalRules.ShouldCellBeAlive(fullBoard, 2, 2).Should().BeFalse();

            // Act & Assert - Edge cells should die (overcrowding with 5 neighbors)
            CellSurvivalRules.ShouldCellBeAlive(fullBoard, 0, 1).Should().BeFalse();
            CellSurvivalRules.ShouldCellBeAlive(fullBoard, 1, 0).Should().BeFalse();

            // Act & Assert - Center cell should die (overcrowding with 8 neighbors)
            CellSurvivalRules.ShouldCellBeAlive(fullBoard, 1, 1).Should().BeFalse();
        }

        [Fact]
        public void BoardIdentityComparison_Should_Identify_Identical_Boards()
        {
            // Arrange
            var aliveCells = new[] { (1, 1), (1, 2), (2, 1), (2, 2) };
            var board1 = new Board(4, 4, aliveCells);
            var board2 = new Board(4, 4, aliveCells);

            // Act
            var areIdentical = BoardIdentityComparison.AreIdentical(board1, board2);

            // Assert
            areIdentical.Should().BeTrue();
        }

        [Fact]
        public void BoardIdentityComparison_Should_Identify_Different_Boards_By_Dimensions()
        {
            // Arrange
            var aliveCells = new[] { (1, 1), (1, 2) };
            var board1 = new Board(4, 4, aliveCells);
            var board2 = new Board(5, 4, aliveCells);

            // Act
            var areIdentical = BoardIdentityComparison.AreIdentical(board1, board2);

            // Assert
            areIdentical.Should().BeFalse();
        }

        [Fact]
        public void BoardIdentityComparison_Should_Identify_Different_Boards_By_Cells()
        {
            // Arrange
            var board1 = new Board(4, 4, new[] { (1, 1), (1, 2) });
            var board2 = new Board(4, 4, new[] { (1, 1), (2, 2) });

            // Act
            var areIdentical = BoardIdentityComparison.AreIdentical(board1, board2);

            // Assert
            areIdentical.Should().BeFalse();
        }

        [Fact]
        public void BoardIdentityComparison_Should_Handle_Empty_Boards()
        {
            // Arrange
            var board1 = new Board(3, 3, Array.Empty<(int, int)>());
            var board2 = new Board(3, 3, Array.Empty<(int, int)>());

            // Act
            var areIdentical = BoardIdentityComparison.AreIdentical(board1, board2);

            // Assert
            areIdentical.Should().BeTrue();
        }

        [Fact]
        public void BoardIdentityComparison_Should_Handle_Same_Cells_Different_Order()
        {
            // Arrange
            var board1 = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1) });
            var board2 = new Board(4, 4, new[] { (2, 1), (1, 1), (1, 2) });

            // Act
            var areIdentical = BoardIdentityComparison.AreIdentical(board1, board2);

            // Assert
            areIdentical.Should().BeTrue();
        }

        [Fact]
        public void CellSurvivalRules_Should_Work_With_Block_Pattern()
        {
            // Arrange - 2x2 block (stable pattern)
            var board = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act & Assert - All cells in block should survive (each has exactly 3 neighbors)
            CellSurvivalRules.ShouldCellBeAlive(board, 1, 1).Should().BeTrue();
            CellSurvivalRules.ShouldCellBeAlive(board, 1, 2).Should().BeTrue();
            CellSurvivalRules.ShouldCellBeAlive(board, 2, 1).Should().BeTrue();
            CellSurvivalRules.ShouldCellBeAlive(board, 2, 2).Should().BeTrue();

            // Act & Assert - Adjacent cells should remain dead
            CellSurvivalRules.ShouldCellBeAlive(board, 0, 0).Should().BeFalse();
            CellSurvivalRules.ShouldCellBeAlive(board, 0, 1).Should().BeFalse();
            CellSurvivalRules.ShouldCellBeAlive(board, 1, 0).Should().BeFalse();
        }
    }
}