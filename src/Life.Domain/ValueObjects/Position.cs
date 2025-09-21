using System;

namespace Life.Domain.ValueObjects
{
    /// <summary>
    /// Represents a position coordinate in the Game of Life board
    /// </summary>
    public sealed record Position(int X, int Y)
    {
        public Position() : this(0, 0) { }

        public static implicit operator (int x, int y)(Position position) => (position.X, position.Y);
        public static implicit operator Position((int x, int y) tuple) => new(tuple.x, tuple.y);

        /// <summary>
        /// Gets the 8 neighboring positions around this position
        /// </summary>
        public IEnumerable<Position> GetNeighbors()
        {
            yield return new Position(X - 1, Y - 1);
            yield return new Position(X, Y - 1);
            yield return new Position(X + 1, Y - 1);
            yield return new Position(X - 1, Y);
            yield return new Position(X + 1, Y);
            yield return new Position(X - 1, Y + 1);
            yield return new Position(X, Y + 1);
            yield return new Position(X + 1, Y + 1);
        }

        /// <summary>
        /// Checks if this position is within the given bounds
        /// </summary>
        public bool IsWithinBounds(int width, int height)
        {
            return X >= 0 && Y >= 0 && X < width && Y < height;
        }
    }
}