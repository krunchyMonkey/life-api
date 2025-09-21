using System.Collections;
using Life.Domain.Common;
using Life.Domain.ValueObjects;
using Life.Domain.Events;
using Life.Domain.Services;

namespace Life.Domain.Aggregates
{
    /// <summary>
    /// Represents a Game of Life board - a simplified traditional DDD aggregate root
    /// Core state management with business logic delegated to static domain services
    /// </summary>
    public sealed class Board : AggregateRoot<Guid>
    {
        public BoardDimensions Dimensions { get; private set; }
        public int Width => Dimensions.Width;
        public int Height => Dimensions.Height;
        
        private readonly BitArray _bits;

        // Simplified constructors with specification-based validation
        public Board(int width, int height, IEnumerable<(int x, int y)> alive) : base(Guid.NewGuid())
        {
            // Delegate validation to static domain service using specifications
            BoardValidationService.ValidateConstruction(width, height, alive);
            
            Dimensions = new BoardDimensions(width, height);
            _bits = new BitArray(Dimensions.TotalCells);
            
            InitializeCells(alive.Select(c => new Position(c.x, c.y)));
        }

        public Board(BoardDimensions dimensions, IEnumerable<Position> alivePositions) : base(Guid.NewGuid())
        {
            // Delegate validation to static domain service using specifications
            dimensions.ValidateDimensions();
            dimensions.ValidatePositions(alivePositions);
            
            Dimensions = dimensions;
            _bits = new BitArray(dimensions.TotalCells);
            
            InitializeCells(alivePositions);
        }

        // Private constructor for internal board creation
        private Board(BoardDimensions dimensions, BitArray bits, Guid id) : base(id)
        {
            Dimensions = dimensions;
            _bits = new BitArray(bits);
        }

        #region State Management
        
        private void InitializeCells(IEnumerable<Position> alivePositions)
        {
            foreach (var position in alivePositions)
            {
                _bits[position.Y * Width + position.X] = true;
            }
        }

        private BitArray CreateBitsFromPositions(IEnumerable<Position> alivePositions)
        {
            var bits = new BitArray(Dimensions.TotalCells);
            foreach (var position in alivePositions)
            {
                bits[position.Y * Width + position.X] = true;
            }
            return bits;
        }

        #endregion

        #region Core Query Methods
        
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

        #endregion

        #region Business Methods (Delegating to Static Services)
        
        /// <summary>
        /// Creates the next generation board - delegates to CellLifecycleService
        /// </summary>
        public Board NextGeneration()
        {
            var nextBoard = this.GenerateNextGeneration();
            
            // Create new board with proper domain events
            var result = CreateBoardFromPositions(nextBoard.AlivePositions(), Guid.NewGuid());
            
            // Raise domain event for generation advancement
            AddDomainEvent(new BoardGenerationAdvanced(Id, result.Id));
            
            return result;
        }

        /// <summary>
        /// Advances multiple generations - delegates to BoardSimulationService
        /// </summary>
        public Board AdvanceGenerations(long count)
        {
            var finalBoard = BoardSimulationService.AdvanceGenerations(this, count);
            
            // Create new board with proper domain events
            var result = CreateBoardFromPositions(finalBoard.AlivePositions(), Guid.NewGuid());

            // Raise domain event for multiple generation advancement
            AddDomainEvent(new BoardMultipleGenerationsAdvanced(Id, result.Id, count));
            
            return result;
        }

        /// <summary>
        /// Detects final states - delegates to BoardSimulationService
        /// </summary>
        public FinalResult DetectFinalState(int maxIterations, TimeSpan maxTime)
        {
            var result = BoardSimulationService.DetectFinalState(this, maxIterations, maxTime);

            // Raise domain event for final state detection
            AddDomainEvent(new BoardFinalStateDetected(Id, result.Board.Id, result.Cyclic, result.Stable, result.CycleLength));
            
            return result;
        }

        /// <summary>
        /// Checks if this board is identical to another - delegates to BoardComparisonService
        /// </summary>
        public bool IsIdenticalTo(Board other)
        {
            return this.IsBoardIdenticalTo(other);
        }

        #endregion

        #region Helper Methods
        
        /// <summary>
        /// Creates a new board from alive positions with specified ID
        /// </summary>
        private Board CreateBoardFromPositions(IEnumerable<Position> alivePositions, Guid id)
        {
            var bits = CreateBitsFromPositions(alivePositions);
            return new Board(Dimensions, bits, id);
        }

        #endregion
    }
}