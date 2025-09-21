using FluentAssertions;
using Life.Domain.ValueObjects;

namespace Life.Domain.Tests.ValueObjects
{
    public class PositionTests
    {
        [Fact]
        public void Position_Should_Initialize_With_Correct_Values()
        {
            // Arrange & Act
            var position = new Position(3, 5);

            // Assert
            position.X.Should().Be(3);
            position.Y.Should().Be(5);
        }

        [Fact]
        public void Positions_With_Same_Coordinates_Should_Be_Equal()
        {
            // Arrange
            var position1 = new Position(2, 4);
            var position2 = new Position(2, 4);

            // Act & Assert
            position1.Should().Be(position2);
            position1.GetHashCode().Should().Be(position2.GetHashCode());
            (position1 == position2).Should().BeTrue();
            (position1 != position2).Should().BeFalse();
        }

        [Fact]
        public void Positions_With_Different_Coordinates_Should_Not_Be_Equal()
        {
            // Arrange
            var position1 = new Position(2, 4);
            var position2 = new Position(3, 4);
            var position3 = new Position(2, 5);

            // Act & Assert
            position1.Should().NotBe(position2);
            position1.Should().NotBe(position3);
            position2.Should().NotBe(position3);
            (position1 == position2).Should().BeFalse();
            (position1 != position2).Should().BeTrue();
        }

        [Fact]
        public void Position_Should_Support_Deconstruction()
        {
            // Arrange
            var position = new Position(7, 9);

            // Act
            var (x, y) = position;

            // Assert
            x.Should().Be(7);
            y.Should().Be(9);
        }

        [Fact]
        public void Position_Should_Have_Meaningful_ToString()
        {
            // Arrange
            var position = new Position(10, 15);

            // Act
            var result = position.ToString();

            // Assert
            result.Should().Be("Position { X = 10, Y = 15 }");
        }

        [Fact]
        public void Position_Should_Handle_Zero_Coordinates()
        {
            // Arrange & Act
            var position = new Position(0, 0);

            // Assert
            position.X.Should().Be(0);
            position.Y.Should().Be(0);
        }

        [Fact]
        public void Position_Should_Handle_Negative_Coordinates()
        {
            // Arrange & Act
            var position = new Position(-5, -10);

            // Assert
            position.X.Should().Be(-5);
            position.Y.Should().Be(-10);
        }

        [Fact]
        public void Position_Should_Support_Implicit_Tuple_Conversion()
        {
            // Arrange
            var position = new Position(3, 7);

            // Act - Implicit conversion to tuple
            (int x, int y) tuple = position;

            // Assert
            tuple.x.Should().Be(3);
            tuple.y.Should().Be(7);
        }

        [Fact]
        public void Position_Equality_Should_Be_Consistent_With_HashCode()
        {
            // Arrange
            var position1 = new Position(5, 10);
            var position2 = new Position(5, 10);
            var position3 = new Position(6, 10);

            // Act & Assert
            position1.Equals(position2).Should().BeTrue();
            position1.GetHashCode().Should().Be(position2.GetHashCode());
            
            position1.Equals(position3).Should().BeFalse();
            position1.GetHashCode().Should().NotBe(position3.GetHashCode());
        }
    }
}