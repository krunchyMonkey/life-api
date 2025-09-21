using System.Collections;
using Life.Domain.Common;
using Life.Domain.ValueObjects;

namespace Life.Domain.Aggregates
{
    /// <summary>
    /// Represents a Game of Life board - the main aggregate root
    /// </summary>
    public sealed class Board : AggregateRoot<Guid>
    {
        public BoardDimensions Dimensions { get; private set; }
        public int Width => Dimensions.Width;
        public int Height => Dimensions.Height;
        
        private readonly BitArray _bits;

        public Board(int width, int height, IEnumerable<(int x, int y)> alive) : base(Guid.NewGuid())
        {
            var dimensions = new BoardDimensions(width, height);
            if (!dimensions.IsValid) 
                throw new ArgumentOutOfRangeException(nameof(dimensions), "Board dimensions must be positive");
            
            Dimensions = dimensions;
            _bits = new BitArray(dimensions.TotalCells);
            
            foreach (var c in alive)
            {
                var position = new Position(c.x, c.y);
                if (!dimensions.Contains(position)) 
                    throw new ArgumentOutOfRangeException(nameof(alive), $"Position {position} is outside board bounds");
                _bits[c.y * width + c.x] = true;
            }
        }

        public Board(BoardDimensions dimensions, IEnumerable<Position> alivePositions) : base(Guid.NewGuid())
        {
            if (!dimensions.IsValid) 
                throw new ArgumentOutOfRangeException(nameof(dimensions), "Board dimensions must be positive");
            
            Dimensions = dimensions;
            _bits = new BitArray(dimensions.TotalCells);
            
            foreach (var position in alivePositions)
            {
                if (!dimensions.Contains(position)) 
                    throw new ArgumentOutOfRangeException(nameof(alivePositions), $"Position {position} is outside board bounds");
                _bits[position.Y * dimensions.Width + position.X] = true;
            }
        }

        /// <summary>
        /// Gets whether a cell at the specified position is alive
        /// </summary>
        public bool Get(int x, int y)
        {
            var position = new Position(x, y);
            if (!Dimensions.Contains(position))
                return false;
            return _bits[y * Width + x];
        }

        /// <summary>
        /// Gets whether a cell at the specified position is alive
        /// </summary>
        public bool Get(Position position)
        {
            return Get(position.X, position.Y);
        }

        /// <summary>
        /// Gets all alive positions on the board
        /// </summary>
        public IEnumerable<(int x, int y)> Alive()
        {
            for (var i = 0; i < _bits.Length; i++) 
                if (_bits[i]) 
                    yield return (i % Width, i / Width);
        }

        /// <summary>
        /// Gets all alive positions as Position value objects
        /// </summary>
        public IEnumerable<Position> AlivePositions()
        {
            return Alive().Select(c => new Position(c.x, c.y));
        }

        /// <summary>
        /// Counts the number of live neighbors around a position
        /// </summary>
        public int CountLiveNeighbors(Position position)
        {
            return position.GetNeighbors()
                .Where(neighbor => Dimensions.Contains(neighbor))
                .Count(neighbor => Get(neighbor));
        }

        /// <summary>
        /// Determines if a cell should be alive in the next generation based on Conway's rules
        /// </summary>
        public bool ShouldCellBeAlive(Position position)
        {
            var isCurrentlyAlive = Get(position);
            var liveNeighbors = CountLiveNeighbors(position);

            // Conway's Game of Life rules:
            // 1. Any live cell with fewer than two live neighbors dies (underpopulation)
            // 2. Any live cell with two or three live neighbors lives on to the next generation
            // 3. Any live cell with more than three live neighbors dies (overpopulation)
            // 4. Any dead cell with exactly three live neighbors becomes a live cell (reproduction)
            
            return isCurrentlyAlive ? (liveNeighbors == 2 || liveNeighbors == 3) : (liveNeighbors == 3);
        }

        /// <summary>
        /// Creates the next generation board according to Conway's Game of Life rules
        /// </summary>
        public Board NextGeneration()
        {
            var aliveInNextGeneration = new List<Position>();
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var position = new Position(x, y);
                    if (ShouldCellBeAlive(position))
                    {
                        aliveInNextGeneration.Add(position);
                    }
                }
            }
            
            return new Board(Dimensions, aliveInNextGeneration);
        }

        /// <summary>
        /// Checks if this board is identical to another board
        /// </summary>
        public bool IsIdenticalTo(Board other)
        {
            if (other is null) return false;
            if (Dimensions != other.Dimensions) return false;
            
            var thisAlive = Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            var otherAlive = other.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            
            return thisAlive.SequenceEqual(otherAlive);
        }
    }
}