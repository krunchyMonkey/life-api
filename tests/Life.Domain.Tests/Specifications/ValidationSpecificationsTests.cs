using FluentAssertions;
using Life.Domain.Services;
using Life.Domain.Specifications;
using Life.Domain.ValueObjects;

namespace Life.Domain.Tests.Specifications
{
    public class ValidationSpecificationsTests
    {
        #region ValidBoardConstructionSpecification Tests

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Accept_Valid_Parameters()
        {
            // Arrange
            var validParams = new BoardConstructionParameters(5, 5, new[] { (1, 1), (2, 2), (3, 3) });
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(validParams);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Accept_Empty_Positions()
        {
            // Arrange
            var validParams = new BoardConstructionParameters(5, 5, Array.Empty<(int, int)>());
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(validParams);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Accept_Boundary_Positions()
        {
            // Arrange
            var validParams = new BoardConstructionParameters(3, 3, new[] { (0, 0), (2, 2), (0, 2), (2, 0) });
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(validParams);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Zero_Width()
        {
            // Arrange
            var specification = new ValidBoardConstructionSpecification();
            var invalidParams = new BoardConstructionParameters(0, 5, Array.Empty<(int, int)>());
            
            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);
            
            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Zero_Height()
        {
            // Arrange
            var specification = new ValidBoardConstructionSpecification();
            var invalidParams = new BoardConstructionParameters(5, 0, Array.Empty<(int, int)>());
            
            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);
            
            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Negative_Width()
        {
            // Arrange
            var specification = new ValidBoardConstructionSpecification();
            var invalidParams = new BoardConstructionParameters(-1, 5, Array.Empty<(int, int)>());
            
            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);
            
            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Negative_Height()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(5, -1, Array.Empty<(int, int)>());
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Out_Of_Bounds_X_Position()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(5, 5, new[] { (1, 1), (5, 2), (2, 3) }); // (5, 2) is out of bounds
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Out_Of_Bounds_Y_Position()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(5, 5, new[] { (1, 1), (2, 5), (2, 3) }); // (2, 5) is out of bounds
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Negative_X_Position()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(5, 5, new[] { (1, 1), (-1, 2), (2, 3) }); // (-1, 2) is invalid
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Reject_Negative_Y_Position()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(5, 5, new[] { (1, 1), (2, -1), (2, 3) }); // (2, -1) is invalid
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(invalidParams);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidBoardConstructionSpecification_GetValidationErrors_Should_Return_Dimension_Errors()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(-1, 0, Array.Empty<(int, int)>());
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var errors = specification.GetValidationErrors(invalidParams);

            // Assert
            errors.Should().HaveCount(1);
            errors[0].ParameterName.Should().Be(nameof(invalidParams.Width));
            errors[0].Message.Should().Contain("positive");
        }

        [Fact]
        public void ValidBoardConstructionSpecification_GetValidationErrors_Should_Return_Position_Errors()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(3, 3, new[] { (1, 1), (3, 1), (1, 2) }); // (3, 1) is out of bounds
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var errors = specification.GetValidationErrors(invalidParams);

            // Assert
            errors.Should().HaveCount(1);
            errors[0].ParameterName.Should().Be(nameof(invalidParams.Alive));
            errors[0].Message.Should().Contain("(3, 1)");
            errors[0].Message.Should().Contain("outside board bounds");
        }

        [Fact]
        public void ValidBoardConstructionSpecification_GetValidationErrors_Should_Return_Multiple_Position_Errors()
        {
            // Arrange - Multiple invalid positions, but should only return one error (first invalid position)
            var invalidParams = new BoardConstructionParameters(2, 2, new[] { (1, 1), (2, 1), (1, 3) }); // Both (2, 1) and (1, 3) are out of bounds
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var errors = specification.GetValidationErrors(invalidParams);

            // Assert
            errors.Should().HaveCount(1); // Only reports first invalid position to avoid spam
            errors[0].ParameterName.Should().Be(nameof(invalidParams.Alive));
        }

        [Fact]
        public void ValidBoardConstructionSpecification_Should_Skip_Position_Validation_For_Invalid_Dimensions()
        {
            // Arrange
            var invalidParams = new BoardConstructionParameters(0, 5, new[] { (10, 10) }); // Invalid dimension, would be invalid position if dimensions were valid
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var errors = specification.GetValidationErrors(invalidParams);
            var isValid = specification.IsSatisfiedBy(invalidParams);

            // Assert
            isValid.Should().BeFalse();
            errors.Should().HaveCount(1); // Only dimension error, position validation skipped
            errors[0].ParameterName.Should().Be(nameof(invalidParams.Width));
        }

        [Fact]
        public void ValidBoardConstructionSpecification_GetValidationErrors_Should_Return_Empty_For_Valid_Parameters()
        {
            // Arrange
            var validParams = new BoardConstructionParameters(5, 5, new[] { (1, 1), (2, 2), (3, 3) });
            var specification = new ValidBoardConstructionSpecification();

            // Act
            var errors = specification.GetValidationErrors(validParams);

            // Assert
            errors.Should().BeEmpty();
        }

        #endregion

        #region ValidBoardDimensionsSpecification Tests

        [Fact]
        public void ValidBoardDimensionsSpecification_Should_Accept_Valid_Dimensions()
        {
            // Arrange
            var validDimensions = new BoardDimensions(5, 5);
            var specification = new ValidBoardDimensionsSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(validDimensions);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidBoardDimensionsSpecification_Should_Accept_Large_Dimensions()
        {
            // Arrange
            var validDimensions = new BoardDimensions(1000, 1000);
            var specification = new ValidBoardDimensionsSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(validDimensions);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidBoardDimensionsSpecification_Should_Accept_Minimum_Valid_Dimensions()
        {
            // Arrange
            var validDimensions = new BoardDimensions(1, 1);
            var specification = new ValidBoardDimensionsSpecification();

            // Act
            var isValid = specification.IsSatisfiedBy(validDimensions);

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region ValidPositionsSpecification Tests

        [Fact]
        public void ValidPositionsSpecification_Should_Accept_Empty_Positions()
        {
            // Arrange
            var dimensions = new BoardDimensions(5, 5);
            var emptyPositions = Array.Empty<Position>();
            var specification = new ValidPositionsSpecification(dimensions);

            // Act
            var isValid = specification.IsSatisfiedBy(emptyPositions);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidPositionsSpecification_Should_Accept_Valid_Positions()
        {
            // Arrange
            var dimensions = new BoardDimensions(5, 5);
            var validPositions = new[] { new Position(1, 1), new Position(2, 2), new Position(3, 3) };
            var specification = new ValidPositionsSpecification(dimensions);

            // Act
            var isValid = specification.IsSatisfiedBy(validPositions);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidPositionsSpecification_Should_Accept_Boundary_Positions()
        {
            // Arrange
            var dimensions = new BoardDimensions(3, 3);
            var boundaryPositions = new[] { new Position(0, 0), new Position(2, 2), new Position(0, 2), new Position(2, 0) };
            var specification = new ValidPositionsSpecification(dimensions);

            // Act
            var isValid = specification.IsSatisfiedBy(boundaryPositions);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidPositionsSpecification_Should_Reject_Out_Of_Bounds_Positions()
        {
            // Arrange
            var dimensions = new BoardDimensions(3, 3);
            var invalidPositions = new[] { new Position(1, 1), new Position(3, 1), new Position(1, 2) }; // (3, 1) is out of bounds
            var specification = new ValidPositionsSpecification(dimensions);

            // Act
            var isValid = specification.IsSatisfiedBy(invalidPositions);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidPositionsSpecification_Should_Reject_Negative_Positions()
        {
            // Arrange
            var dimensions = new BoardDimensions(5, 5);
            var invalidPositions = new[] { new Position(1, 1), new Position(-1, 2), new Position(2, 3) }; // (-1, 2) is invalid
            var specification = new ValidPositionsSpecification(dimensions);

            // Act
            var isValid = specification.IsSatisfiedBy(invalidPositions);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidPositionsSpecification_Should_Throw_For_Null_Dimensions()
        {
            // Arrange & Act & Assert
            var act = () => new ValidPositionsSpecification(null!);
            
            act.Should().Throw<ArgumentNullException>()
               .WithMessage("*dimensions*");
        }

        [Fact]
        public void ValidPositionsSpecification_Should_Handle_Single_Position()
        {
            // Arrange
            var dimensions = new BoardDimensions(5, 5);
            var validPosition = new[] { new Position(2, 2) };
            var invalidPosition = new[] { new Position(5, 5) };
            var specification = new ValidPositionsSpecification(dimensions);

            // Act & Assert
            specification.IsSatisfiedBy(validPosition).Should().BeTrue();
            specification.IsSatisfiedBy(invalidPosition).Should().BeFalse();
        }

        #endregion

        #region ValidPositionSpecification Tests

        [Fact]
        public void ValidPositionSpecification_Should_Accept_Valid_Position()
        {
            // Arrange
            var specification = new ValidPositionSpecification(5, 5);
            var validPosition = new Position(2, 3);

            // Act
            var isValid = specification.IsSatisfiedBy(validPosition);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidPositionSpecification_Should_Accept_Corner_Positions()
        {
            // Arrange
            var specification = new ValidPositionSpecification(5, 5);

            // Act & Assert
            specification.IsSatisfiedBy(new Position(0, 0)).Should().BeTrue();
            specification.IsSatisfiedBy(new Position(4, 4)).Should().BeTrue();
            specification.IsSatisfiedBy(new Position(0, 4)).Should().BeTrue();
            specification.IsSatisfiedBy(new Position(4, 0)).Should().BeTrue();
        }

        [Fact]
        public void ValidPositionSpecification_Should_Accept_Edge_Positions()
        {
            // Arrange
            var specification = new ValidPositionSpecification(5, 5);

            // Act & Assert
            specification.IsSatisfiedBy(new Position(0, 2)).Should().BeTrue(); // Left edge
            specification.IsSatisfiedBy(new Position(4, 2)).Should().BeTrue(); // Right edge
            specification.IsSatisfiedBy(new Position(2, 0)).Should().BeTrue(); // Top edge
            specification.IsSatisfiedBy(new Position(2, 4)).Should().BeTrue(); // Bottom edge
        }

        [Fact]
        public void ValidPositionSpecification_Should_Reject_Out_Of_Bounds_Positions()
        {
            // Arrange
            var specification = new ValidPositionSpecification(3, 3);

            // Act & Assert
            specification.IsSatisfiedBy(new Position(-1, 0)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(0, -1)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(3, 0)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(0, 3)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(3, 3)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(-1, -1)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(5, 5)).Should().BeFalse();
        }

        [Fact]
        public void ValidPositionSpecification_Should_Handle_Minimum_Board_Size()
        {
            // Arrange
            var specification = new ValidPositionSpecification(1, 1);

            // Act & Assert
            specification.IsSatisfiedBy(new Position(0, 0)).Should().BeTrue();
            specification.IsSatisfiedBy(new Position(1, 0)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(0, 1)).Should().BeFalse();
            specification.IsSatisfiedBy(new Position(1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ValidPositionSpecification_Should_Handle_Rectangular_Boards()
        {
            // Arrange
            var specification = new ValidPositionSpecification(3, 5); // 3 wide, 5 tall

            // Act & Assert
            specification.IsSatisfiedBy(new Position(2, 4)).Should().BeTrue(); // Max valid position
            specification.IsSatisfiedBy(new Position(3, 4)).Should().BeFalse(); // Too wide
            specification.IsSatisfiedBy(new Position(2, 5)).Should().BeFalse(); // Too tall
            specification.IsSatisfiedBy(new Position(0, 0)).Should().BeTrue();  // Min valid position
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void All_Specifications_Should_Work_Together_For_Valid_Board()
        {
            // Arrange
            var dimensions = new BoardDimensions(5, 5);
            var positions = new[] { new Position(1, 1), new Position(2, 2), new Position(3, 3) };
            var constructionParams = new BoardConstructionParameters(5, 5, new[] { (1, 1), (2, 2), (3, 3) });

            var dimensionSpec = new ValidBoardDimensionsSpecification();
            var positionSpec = new ValidPositionsSpecification(dimensions);
            var constructionSpec = new ValidBoardConstructionSpecification();

            // Act & Assert
            dimensionSpec.IsSatisfiedBy(dimensions).Should().BeTrue();
            positionSpec.IsSatisfiedBy(positions).Should().BeTrue();
            constructionSpec.IsSatisfiedBy(constructionParams).Should().BeTrue();
        }

        [Fact]
        public void All_Specifications_Should_Work_Together_For_Invalid_Board()
        {
            // Arrange
            var positions = new[] { new Position(1, 1), new Position(5, 5) }; // (5, 5) is out of bounds for 3x3 board
            var constructionParams = new BoardConstructionParameters(3, 3, new[] { (1, 1), (5, 5) });

            var dimensions = new BoardDimensions(3, 3);
            var positionSpec = new ValidPositionsSpecification(dimensions);
            var constructionSpec = new ValidBoardConstructionSpecification();

            // Act & Assert
            positionSpec.IsSatisfiedBy(positions).Should().BeFalse();
            constructionSpec.IsSatisfiedBy(constructionParams).Should().BeFalse();
        }

        #endregion
    }
}