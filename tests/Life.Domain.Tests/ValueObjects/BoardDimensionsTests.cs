using FluentAssertions;
using Life.Domain.ValueObjects;

namespace Life.Domain.Tests.ValueObjects
{
    public class BoardDimensionsTests
    {
        [Fact]
        public void BoardDimensions_Should_Initialize_With_Correct_Values()
        {
            // Arrange & Act
            var dimensions = new BoardDimensions(10, 15);

            // Assert
            dimensions.Width.Should().Be(10);
            dimensions.Height.Should().Be(15);
        }

        [Fact]
        public void BoardDimensions_With_Same_Values_Should_Be_Equal()
        {
            // Arrange
            var dimensions1 = new BoardDimensions(8, 12);
            var dimensions2 = new BoardDimensions(8, 12);

            // Act & Assert
            dimensions1.Should().Be(dimensions2);
            dimensions1.GetHashCode().Should().Be(dimensions2.GetHashCode());
            (dimensions1 == dimensions2).Should().BeTrue();
            (dimensions1 != dimensions2).Should().BeFalse();
        }

        [Fact]
        public void BoardDimensions_With_Different_Values_Should_Not_Be_Equal()
        {
            // Arrange
            var dimensions1 = new BoardDimensions(8, 12);
            var dimensions2 = new BoardDimensions(9, 12);
            var dimensions3 = new BoardDimensions(8, 13);

            // Act & Assert
            dimensions1.Should().NotBe(dimensions2);
            dimensions1.Should().NotBe(dimensions3);
            dimensions2.Should().NotBe(dimensions3);
            (dimensions1 == dimensions2).Should().BeFalse();
            (dimensions1 != dimensions2).Should().BeTrue();
        }

        [Fact]
        public void BoardDimensions_Should_Support_Deconstruction()
        {
            // Arrange
            var dimensions = new BoardDimensions(20, 25);

            // Act
            var (width, height) = dimensions;

            // Assert
            width.Should().Be(20);
            height.Should().Be(25);
        }

        [Fact]
        public void BoardDimensions_Should_Have_Meaningful_ToString()
        {
            // Arrange
            var dimensions = new BoardDimensions(30, 40);

            // Act
            var result = dimensions.ToString();

            // Assert
            result.Should().Be("BoardDimensions { Width = 30, Height = 40 }");
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(5, 0)]
        [InlineData(-1, 5)]
        [InlineData(5, -1)]
        public void BoardDimensions_Should_Throw_For_Invalid_Dimensions(int width, int height)
        {
            // Arrange & Act & Assert
            var act = () => new BoardDimensions(width, height);
            
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Width and height must be positive*");
        }

        [Fact]
        public void BoardDimensions_Should_Accept_Minimum_Valid_Dimensions()
        {
            // Arrange & Act
            var dimensions = new BoardDimensions(1, 1);

            // Assert
            dimensions.Width.Should().Be(1);
            dimensions.Height.Should().Be(1);
        }

        [Fact]
        public void BoardDimensions_Should_Calculate_Total_Cells()
        {
            // Arrange
            var dimensions = new BoardDimensions(5, 4);

            // Act
            var totalCells = dimensions.TotalCells;

            // Assert
            totalCells.Should().Be(20);
        }

        [Fact]
        public void BoardDimensions_Should_Check_If_Position_Is_Within_Bounds()
        {
            // Arrange
            var dimensions = new BoardDimensions(5, 4);

            // Act & Assert
            dimensions.Contains(new Position(0, 0)).Should().BeTrue();
            dimensions.Contains(new Position(4, 3)).Should().BeTrue();
            dimensions.Contains(new Position(2, 2)).Should().BeTrue();
            
            dimensions.Contains(new Position(-1, 0)).Should().BeFalse();
            dimensions.Contains(new Position(0, -1)).Should().BeFalse();
            dimensions.Contains(new Position(5, 0)).Should().BeFalse();
            dimensions.Contains(new Position(0, 4)).Should().BeFalse();
            dimensions.Contains(new Position(5, 4)).Should().BeFalse();
        }

        [Fact]
        public void BoardDimensions_Equality_Should_Be_Consistent_With_HashCode()
        {
            // Arrange
            var dimensions1 = new BoardDimensions(6, 8);
            var dimensions2 = new BoardDimensions(6, 8);
            var dimensions3 = new BoardDimensions(7, 8);

            // Act & Assert
            dimensions1.Equals(dimensions2).Should().BeTrue();
            dimensions1.GetHashCode().Should().Be(dimensions2.GetHashCode());
            
            dimensions1.Equals(dimensions3).Should().BeFalse();
            dimensions1.GetHashCode().Should().NotBe(dimensions3.GetHashCode());
        }
    }
}