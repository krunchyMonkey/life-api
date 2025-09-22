using FluentAssertions;
using Life.Domain.Aggregates;
using Life.Domain.Services;
using Life.Domain.ValueObjects;

namespace Life.Domain.Tests.Services
{
    public class CellLifecycleServiceTests
    {
        [Fact]
        public void ShouldCellSurvive_Should_Apply_Conway_Birth_Rule()
        {
            // Arrange - L-tetromino pattern where center has exactly 3 neighbors
            var board = new Board(5, 5, new[] { (1, 1), (2, 1), (2, 2), (2, 3) });

            // Act - Test birth at position with exactly 3 neighbors
            var shouldSurvive = CellLifecycleService.ShouldCellSurvive(board, 1, 2);

            // Assert
            shouldSurvive.Should().BeTrue(); // Dead cell with 3 neighbors should be born
        }

        [Fact]
        public void ShouldCellSurvive_Should_Apply_Conway_Survival_Rules()
        {
            // Arrange - 2x2 block pattern
            var board = new Board(4, 4, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

            // Act & Assert - All cells in block have exactly 3 neighbors and should survive
            CellLifecycleService.ShouldCellSurvive(board, 1, 1).Should().BeTrue();
            CellLifecycleService.ShouldCellSurvive(board, 1, 2).Should().BeTrue();
            CellLifecycleService.ShouldCellSurvive(board, 2, 1).Should().BeTrue();
            CellLifecycleService.ShouldCellSurvive(board, 2, 2).Should().BeTrue();
        }

        [Fact]
        public void ShouldCellSurvive_Should_Apply_Conway_Death_Rules()
        {
            // Arrange - Full 3x3 board (all cells alive)
            var board = new Board(3, 3, new[] 
            {
                (0, 0), (0, 1), (0, 2),
                (1, 0), (1, 1), (1, 2),
                (2, 0), (2, 1), (2, 2)
            });

            // Act & Assert - All cells should die from overcrowding (>3 neighbors)
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    CellLifecycleService.ShouldCellSurvive(board, x, y).Should().BeFalse();
                }
            }
        }

        [Fact]
        public void ShouldCellSurvive_Should_Handle_Blinker_Pattern()
        {
            // Arrange - Horizontal blinker
            var board = new Board(5, 5, new[] { (2, 1), (2, 2), (2, 3) });

            // Act & Assert - Center cell has 2 neighbors, should survive
            CellLifecycleService.ShouldCellSurvive(board, 2, 2).Should().BeTrue();

            // Act & Assert - End cells have 1 neighbor each, should die
            CellLifecycleService.ShouldCellSurvive(board, 2, 1).Should().BeFalse();
            CellLifecycleService.ShouldCellSurvive(board, 2, 3).Should().BeFalse();

            // Act & Assert - Cells above and below center have 3 neighbors, should be born
            CellLifecycleService.ShouldCellSurvive(board, 1, 2).Should().BeTrue();
            CellLifecycleService.ShouldCellSurvive(board, 3, 2).Should().BeTrue();
        }

        [Fact]
        public void ShouldCellSurvive_Should_Handle_Empty_Board()
        {
            // Arrange
            var emptyBoard = new Board(5, 5, Array.Empty<(int, int)>());

            // Act & Assert - All cells should remain dead (0 neighbors)
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    CellLifecycleService.ShouldCellSurvive(emptyBoard, x, y).Should().BeFalse();
                }
            }
        }

        [Fact]
        public void ShouldCellSurvive_Should_Handle_Single_Cell()
        {
            // Arrange
            var board = new Board(3, 3, new[] { (1, 1) });

            // Act & Assert - Single cell should die (0 neighbors)
            CellLifecycleService.ShouldCellSurvive(board, 1, 1).Should().BeFalse();

            // Act & Assert - All surrounding cells should remain dead (1 neighbor each)
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1) continue; // Skip the center cell
                    CellLifecycleService.ShouldCellSurvive(board, x, y).Should().BeFalse();
                }
            }
        }

        [Fact]
        public void ShouldCellSurvive_Should_Handle_Edge_Cases()
        {
            // Arrange - Cells at board edges
            var board = new Board(3, 3, new[] { (0, 0), (0, 1), (1, 0) });

            // Act & Assert - Corner cell (0,0) has 2 neighbors, should survive
            CellLifecycleService.ShouldCellSurvive(board, 0, 0).Should().BeTrue();

            // Act & Assert - Edge cell (0,1) has 2 neighbors, should survive  
            CellLifecycleService.ShouldCellSurvive(board, 0, 1).Should().BeTrue();

            // Act & Assert - Edge cell (1,0) has 2 neighbors, should survive
            CellLifecycleService.ShouldCellSurvive(board, 1, 0).Should().BeTrue();

            // Act & Assert - Center cell (1,1) has 3 neighbors, should be born
            CellLifecycleService.ShouldCellSurvive(board, 1, 1).Should().BeTrue();
        }

        [Fact]
        public void CountLiveNeighbors_Should_Count_All_Adjacent_Cells()
        {
            // Arrange - 3x3 pattern with center cell
            var board = new Board(5, 5, new[] 
            {
                (1, 1), (1, 2), (1, 3),
                (2, 1),         (2, 3),
                (3, 1), (3, 2), (3, 3)
            });

            // Act
            var neighbors = board.CountLiveNeighbors(new Position(2, 2));

            // Assert - Center position (2,2) should have 8 neighbors
            neighbors.Should().Be(8);
        }

        [Fact]
        public void CountLiveNeighbors_Should_Handle_Corner_Position()
        {
            // Arrange
            var board = new Board(3, 3, new[] { (0, 1), (1, 0), (1, 1) });

            // Act
            var neighbors = board.CountLiveNeighbors(new Position(0, 0));

            // Assert - Corner position (0,0) should have 3 neighbors
            neighbors.Should().Be(3);
        }

        [Fact]
        public void CountLiveNeighbors_Should_Handle_Edge_Position()
        {
            // Arrange
            var board = new Board(5, 5, new[] { (0, 2), (1, 1), (1, 2), (1, 3), (2, 2) });

            // Act
            var neighbors = board.CountLiveNeighbors(new Position(1, 2));

            // Assert - Edge position (1,2) should have 4 neighbors
            neighbors.Should().Be(4);
        }

        [Fact]
        public void CountLiveNeighbors_Should_Return_Zero_For_Isolated_Position()
        {
            // Arrange
            var board = new Board(5, 5, new[] { (4, 4) }); // Cell in opposite corner

            // Act
            var neighbors = board.CountLiveNeighbors(new Position(0, 0));

            // Assert
            neighbors.Should().Be(0);
        }
    }
}