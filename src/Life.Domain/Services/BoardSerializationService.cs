using Life.Domain.Aggregates;

namespace Life.Domain.Services
{
    /// <summary>
    /// Domain service for board serialization and deserialization operations
    /// Centralizes board state persistence logic that was previously duplicated
    /// </summary>
    public static class BoardSerializationService
    {
        /// <summary>
        /// Serializes a board to binary format
        /// </summary>
        /// <param name="board">The board to serialize</param>
        /// <returns>Binary representation of the board state</returns>
        public static byte[] Serialize(Board board)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            
            // Write board dimensions
            bw.Write(board.Width);
            bw.Write(board.Height);
            
            // Write living cells
            var aliveCells = board.Alive().ToArray();
            bw.Write(aliveCells.Length);
            
            foreach (var (x, y) in aliveCells) 
            { 
                bw.Write(x); 
                bw.Write(y); 
            }
            
            bw.Flush();
            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes binary data back into a Board
        /// </summary>
        /// <param name="data">Binary data representing a board state</param>
        /// <returns>Reconstructed Board instance</returns>
        /// <exception cref="ArgumentException">Thrown when data is null, empty, or invalid</exception>
        public static Board Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Serialized data cannot be null or empty", nameof(data));

            try
            {
                using var ms = new MemoryStream(data);
                using var br = new BinaryReader(ms);
                
                // Read board dimensions
                var width = br.ReadInt32();
                var height = br.ReadInt32();
                var cellCount = br.ReadInt32();
                
                // Validate dimensions
                if (width <= 0 || height <= 0)
                    throw new ArgumentException($"Invalid board dimensions: {width}x{height}");
                
                if (cellCount < 0)
                    throw new ArgumentException($"Invalid cell count: {cellCount}");
                
                // Read living cells
                var cells = new (int x, int y)[cellCount];
                for (int i = 0; i < cellCount; i++) 
                {
                    var x = br.ReadInt32();
                    var y = br.ReadInt32();
                    
                    // Validate cell coordinates
                    if (x < 0 || x >= width || y < 0 || y >= height)
                        throw new ArgumentException($"Invalid cell coordinates: ({x}, {y}) for board {width}x{height}");
                    
                    cells[i] = (x, y);
                }
                
                return new Board(width, height, cells);
            }
            catch (EndOfStreamException ex)
            {
                throw new ArgumentException("Serialized data is incomplete or corrupted", nameof(data), ex);
            }
            catch (IOException ex)
            {
                throw new ArgumentException("Error reading serialized board data", nameof(data), ex);
            }
        }

        /// <summary>
        /// Validates that serialized data represents a valid board state
        /// </summary>
        /// <param name="data">Binary data to validate</param>
        /// <returns>True if data appears to be valid board serialization</returns>
        public static bool IsValidSerializedData(byte[] data)
        {
            if (data == null || data.Length < 12) // Minimum: width(4) + height(4) + count(4)
                return false;

            try
            {
                using var ms = new MemoryStream(data);
                using var br = new BinaryReader(ms);
                
                var width = br.ReadInt32();
                var height = br.ReadInt32();
                var cellCount = br.ReadInt32();
                
                // Basic validation
                if (width <= 0 || height <= 0 || cellCount < 0)
                    return false;
                
                // Check if we have enough data for all cells
                var expectedDataLength = 12 + (cellCount * 8); // 8 bytes per cell (x, y)
                return data.Length >= expectedDataLength;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the board dimensions from serialized data without full deserialization
        /// </summary>
        /// <param name="data">Serialized board data</param>
        /// <returns>Board dimensions</returns>
        public static (int width, int height) GetDimensionsFromSerializedData(byte[] data)
        {
            if (data == null || data.Length < 8)
                throw new ArgumentException("Invalid serialized data", nameof(data));

            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            
            var width = br.ReadInt32();
            var height = br.ReadInt32();
            
            return (width, height);
        }
    }
}