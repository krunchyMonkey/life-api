using Life.Domain.Aggregates;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service for generating hash codes for Board instances
    /// </summary>
    public sealed class BoardHasher
    {
        /// <summary>
        /// Generates a hash code for a board based on its live cells and dimensions
        /// </summary>
        public ulong Hash(Board board)
        {
            ulong hash = 1469598103934665603UL;
            
            foreach (var cell in board.Alive())
            {
                hash ^= (ulong)(cell.x + 397 * cell.y);
                hash *= 1099511628211UL;
            }
            
            hash ^= ((ulong)board.Width << 32) ^ (ulong)board.Height;
            return hash;
        }
    }
}