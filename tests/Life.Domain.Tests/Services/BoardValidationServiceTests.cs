using FluentAssertions;
using Life.Domain.Services;

namespace Life.Domain.Tests.Services
{
    public class BoardValidationServiceTests
    {
        [Fact]
        public void ValidateConstruction_Should_Accept_Valid_Parameters()
        {
            // Arrange
            var width = 5;
            var height = 5;
            var alive = new[] { (1, 1), (2, 2), (3, 3) };

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().NotThrow();
        }

        [Fact]
        public void ValidateConstruction_Should_Reject_Zero_Width()
        {
            // Arrange
            var width = 0;
            var height = 5;
            var alive = new[] { (1, 1), (2, 2) };

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .Which.ParamName.Should().Be("Width");
        }

        [Fact]
        public void ValidateConstruction_Should_Reject_Zero_Height()
        {
            // Arrange
            var width = 5;
            var height = 0;
            var alive = new[] { (1, 1), (2, 2) };

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .Which.ParamName.Should().Be("Height");
        }

        [Fact]
        public void ValidateConstruction_Should_Reject_Negative_Width()
        {
            // Arrange
            var width = -1;
            var height = 5;
            var alive = new[] { (1, 1), (2, 2) };

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .Which.ParamName.Should().Be("Width");
        }

        [Fact]
        public void ValidateConstruction_Should_Reject_Negative_Height()
        {
            // Arrange
            var width = 5;
            var height = -1;
            var alive = new[] { (1, 1), (2, 2) };

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .Which.ParamName.Should().Be("Height");
        }

        [Fact]
        public void ValidateConstruction_Should_Accept_Empty_Alive_Cells()
        {
            // Arrange
            var width = 3;
            var height = 3;
            var alive = Array.Empty<(int, int)>();

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().NotThrow();
        }

        [Fact]
        public void ValidateConstruction_Should_Reject_Cells_Outside_Bounds()
        {
            // Arrange
            var width = 3;
            var height = 3;
            var alive = new[] { (1, 1), (5, 2) }; // (5, 2) is outside bounds

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .Which.ParamName.Should().Be("Alive");
        }

        [Fact]
        public void ValidateConstruction_Should_Accept_Large_Boards()
        {
            // Arrange
            var width = 1000;
            var height = 1000;
            var alive = new[] { (0, 0), (999, 999), (500, 500) };

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().NotThrow();
        }

        [Fact]
        public void ValidateConstruction_Should_Accept_Minimum_Valid_Size()
        {
            // Arrange
            var width = 1;
            var height = 1;
            var alive = new[] { (0, 0) };

            // Act & Assert
            var act = () => BoardValidationService.ValidateConstruction(width, height, alive);
            act.Should().NotThrow();
        }

        [Fact]
        public void BoardConstructionParameters_Should_Be_Record()
        {
            // Arrange & Act
            var params1 = new BoardConstructionParameters(3, 3, new[] { (1, 1) });
            var params2 = new BoardConstructionParameters(3, 3, new[] { (1, 1) });
            var params3 = new BoardConstructionParameters(4, 4, new[] { (1, 1) });

            // Assert
            params1.Should().Be(params2);
            params1.Should().NotBe(params3);
            params1.GetHashCode().Should().Be(params2.GetHashCode());
        }
    }
}
