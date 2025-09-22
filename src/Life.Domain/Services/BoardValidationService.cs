using Life.Domain.Specifications;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Static domain service for board construction validation
    /// Provides validation logic as specifications and extension methods
    /// </summary>
    public static class BoardValidationService
    {
        /// <summary>
        /// Validates board construction parameters using specification pattern
        /// </summary>
        public static void ValidateConstruction(int width, int height, IEnumerable<(int x, int y)> alive)
        {
            var spec = new ValidBoardConstructionSpecification();
            var parameters = new BoardConstructionParameters(width, height, alive);
            
            if (!spec.IsSatisfiedBy(parameters))
            {
                // Get specific validation errors
                var errors = spec.GetValidationErrors(parameters);
                var firstError = errors.First();
                throw new ArgumentOutOfRangeException(firstError.ParameterName, firstError.Message);
            }
        }

        /// <summary>
        /// Validates board dimensions using specification pattern
        /// </summary>
        public static void ValidateDimensions(this BoardDimensions dimensions)
        {
            var spec = new ValidBoardDimensionsSpecification();
            
            if (!spec.IsSatisfiedBy(dimensions))
            {
                throw new ArgumentOutOfRangeException(nameof(dimensions), "Board dimensions must be positive");
            }
        }

        /// <summary>
        /// Validates positions are within bounds using specification pattern
        /// </summary>
        public static void ValidatePositions(this BoardDimensions dimensions, IEnumerable<Position> positions)
        {
            var spec = new ValidPositionsSpecification(dimensions);
            
            if (!spec.IsSatisfiedBy(positions))
            {
                var invalidPositions = positions.Where(pos => !dimensions.Contains(pos));
                var first = invalidPositions.First();
                throw new ArgumentOutOfRangeException(nameof(positions), $"Position {first} is outside board bounds");
            }
        }
    }

    /// <summary>
    /// Value object to hold board construction parameters for validation
    /// </summary>
    public sealed record BoardConstructionParameters(int Width, int Height, IEnumerable<(int x, int y)> Alive);

    /// <summary>
    /// Represents a validation error with context
    /// </summary>
    public sealed record ValidationError(string ParameterName, string Message);
}