using Life.Domain.Common;
using Life.Domain.ValueObjects;
using Life.Domain.Services;

namespace Life.Domain.Specifications
{
    /// <summary>
    /// Specification for validating board construction parameters
    /// Encapsulates all the rules for creating a valid Game of Life board
    /// </summary>
    public sealed class ValidBoardConstructionSpecification : Specification<BoardConstructionParameters>
    {
        public override bool IsSatisfiedBy(BoardConstructionParameters parameters)
        {
            return GetValidationErrors(parameters).Count == 0;
        }

        /// <summary>
        /// Gets detailed validation errors for debugging and user feedback
        /// </summary>
        public IReadOnlyList<ValidationError> GetValidationErrors(BoardConstructionParameters parameters)
        {
            var errors = new List<ValidationError>();

            // Validate dimensions
            if (parameters.Width <= 0 || parameters.Height <= 0)
            {
                errors.Add(new ValidationError(nameof(parameters.Width), "Board dimensions must be positive"));
            }

            // Skip position validation if dimensions are invalid
            if (parameters.Width <= 0 || parameters.Height <= 0)
                return errors.AsReadOnly();

            // Validate alive positions are within bounds
            var invalidPositions = parameters.Alive.Where(pos => 
                pos.x < 0 || pos.y < 0 || pos.x >= parameters.Width || pos.y >= parameters.Height);

            foreach (var invalidPos in invalidPositions)
            {
                errors.Add(new ValidationError(nameof(parameters.Alive), 
                    $"Position ({invalidPos.x}, {invalidPos.y}) is outside board bounds"));
                break; // Only report first invalid position to avoid spam
            }

            return errors.AsReadOnly();
        }
    }

    /// <summary>
    /// Specification for validating board dimensions value object
    /// </summary>
    public sealed class ValidBoardDimensionsSpecification : Specification<BoardDimensions>
    {
        public override bool IsSatisfiedBy(BoardDimensions dimensions)
        {
            return dimensions.IsValid;
        }
    }

    /// <summary>
    /// Specification for validating that positions are within board bounds
    /// </summary>
    public sealed class ValidPositionsSpecification : Specification<IEnumerable<Position>>
    {
        private readonly BoardDimensions _dimensions;

        public ValidPositionsSpecification(BoardDimensions dimensions)
        {
            _dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        }

        public override bool IsSatisfiedBy(IEnumerable<Position> positions)
        {
            return positions.All(pos => _dimensions.Contains(pos));
        }
    }

    /// <summary>
    /// Specification for checking if a position is valid for a given board size
    /// </summary>
    public sealed class ValidPositionSpecification : Specification<Position>
    {
        private readonly int _width;
        private readonly int _height;

        public ValidPositionSpecification(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public override bool IsSatisfiedBy(Position position)
        {
            return position.IsWithinBounds(_width, _height);
        }
    }
}