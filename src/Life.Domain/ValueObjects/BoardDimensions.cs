using System;

namespace Life.Domain.ValueObjects
{
    /// <summary>
    /// Represents the dimensions of a Game of Life board
    /// </summary>
    public sealed record BoardDimensions(int Width, int Height)
    {
        public BoardDimensions() : this(0, 0) { }

        public static implicit operator (int width, int height)(BoardDimensions dimensions) => (dimensions.Width, dimensions.Height);
        public static implicit operator BoardDimensions((int width, int height) tuple) => new(tuple.width, tuple.height);

        /// <summary>
        /// Gets the total number of cells in the board
        /// </summary>
        public int TotalCells => Width * Height;

        /// <summary>
        /// Validates that the dimensions are positive
        /// </summary>
        public bool IsValid => Width > 0 && Height > 0;

        /// <summary>
        /// Checks if a position is within these dimensions
        /// </summary>
        public bool Contains(Position position)
        {
            return position.IsWithinBounds(Width, Height);
        }
    }
}