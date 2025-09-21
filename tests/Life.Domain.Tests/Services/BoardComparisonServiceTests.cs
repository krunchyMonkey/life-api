using FluentAssertions;
using Life.Domain.Aggregates;
using Life.Domain.Services;

namespace Life.Domain.Tests.Services
{
    public class BoardComparisonServiceTests
    {
        [Fact]
        public void IsBoardIdenticalTo_Should_Return_True_For_Identical_Boards()
        {
            // Arrange
            var aliveCells = new[] { (1, 1), (1, 2), (2, 1), (2, 2) };
            var board1 = new Board(4, 4, aliveCells);
            var board2 = new Board(4, 4, aliveCells);

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Return_False_For_Different_Dimensions()
        {
            // Arrange
            var aliveCells = new[] { (1, 1), (1, 2) };
            var board1 = new Board(4, 4, aliveCells);
            var board2 = new Board(5, 4, aliveCells); // Different width

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Return_False_For_Different_Cells()
        {
            // Arrange
            var board1 = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1) });
            var board2 = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 2) }); // Different third cell

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Return_True_For_Same_Cells_Different_Order()
        {
            // Arrange
            var board1 = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1) });
            var board2 = new Board(4, 4, new[] { (2, 1), (1, 1), (1, 2) }); // Same cells, different order

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Return_True_For_Empty_Boards()
        {
            // Arrange
            var board1 = new Board(3, 3, Array.Empty<(int, int)>());
            var board2 = new Board(3, 3, Array.Empty<(int, int)>());

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Return_False_For_Different_Cell_Counts()
        {
            // Arrange
            var board1 = new Board(4, 4, new[] { (1, 1), (1, 2) });
            var board2 = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1) }); // Extra cell

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Return_True_For_Self_Comparison()
        {
            // Arrange
            var board = new Board(5, 5, new[] { (2, 2), (2, 3), (3, 2) });

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board, board);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Handle_Large_Boards()
        {
            // Arrange
            var largeCellPattern = Enumerable.Range(0, 100)
                .Select(i => (i % 10, i / 10))
                .ToArray();
            
            var board1 = new Board(10, 10, largeCellPattern);
            var board2 = new Board(10, 10, largeCellPattern);

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Handle_Single_Cell_Difference_In_Large_Board()
        {
            // Arrange
            var largeCellPattern1 = Enumerable.Range(0, 99)
                .Select(i => (i % 10, i / 10))
                .ToArray();
                
            var largeCellPattern2 = largeCellPattern1
                .Append((9, 9))
                .ToArray();
            
            var board1 = new Board(10, 10, largeCellPattern1);
            var board2 = new Board(10, 10, largeCellPattern2);

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Handle_Height_Difference()
        {
            // Arrange
            var aliveCells = new[] { (1, 1), (1, 2) };
            var board1 = new Board(4, 4, aliveCells);
            var board2 = new Board(4, 5, aliveCells); // Different height

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(board1, board2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Handle_Full_vs_Empty_Board()
        {
            // Arrange
            var fullBoardCells = new List<(int, int)>();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    fullBoardCells.Add((x, y));
                }
            }

            var fullBoard = new Board(3, 3, fullBoardCells);
            var emptyBoard = new Board(3, 3, Array.Empty<(int, int)>());

            // Act
            var result = BoardComparisonService.IsBoardIdenticalTo(fullBoard, emptyBoard);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBoardIdenticalTo_Should_Be_Symmetric()
        {
            // Arrange
            var board1 = new Board(4, 4, new[] { (1, 1), (2, 2) });
            var board2 = new Board(4, 4, new[] { (1, 1), (2, 1) });

            // Act
            var result1to2 = BoardComparisonService.IsBoardIdenticalTo(board1, board2);
            var result2to1 = BoardComparisonService.IsBoardIdenticalTo(board2, board1);

            // Assert
            result1to2.Should().Be(result2to1);
            result1to2.Should().BeFalse(); // They are different
        }
    }
}