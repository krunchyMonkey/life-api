using Life.Domain.Aggregates;
using Life.Domain.Repositories;
using Life.Infrastructure.Data;
using Life.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Life.Infrastructure.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly LifeDbContext _db;
        private readonly ILogger<BoardRepository>? _logger;

        public BoardRepository(LifeDbContext db, ILogger<BoardRepository>? logger = null) 
        {
            _db = db;
            _logger = logger;
        }

        public async Task<string> CreateAsync(Board b, CancellationToken ct)
        {
            var id = Guid.NewGuid().ToString("n");
            _logger?.LogInformation("Creating board with ID: {BoardId}", id);
            
            _db.Boards.Add(new BoardEntity { Id = id, Width = b.Width, Height = b.Height, CreatedUtc = DateTimeOffset.UtcNow });
            _db.Snapshots.Add(new SnapshotEntity { BoardId = id, Generation = 0, Data = Serialize(b) });
            await _db.SaveChangesAsync(ct);
            
            _logger?.LogInformation("Successfully created board with ID: {BoardId}", id);
            return id;
        }

        public async Task<Board?> GetAsync(string id, CancellationToken ct)
        {
            _logger?.LogInformation("Searching for board with ID: {BoardId}", id);
            
            // First check if the board exists
            var boardExists = await _db.Boards.AsNoTracking()
                .AnyAsync(b => b.Id == id, ct);
            
            _logger?.LogInformation("Board exists: {BoardExists}", boardExists);
            
            // Then get the latest snapshot
            var snap = await _db.Snapshots.AsNoTracking()
                .Where(s => s.BoardId == id)
                .OrderByDescending(s => s.Generation)
                .FirstOrDefaultAsync(ct);
            
            if (snap == null) 
            {
                _logger?.LogWarning("No snapshot found for BoardId: {BoardId}", id);
                return null;
            }
            
            _logger?.LogInformation("Found snapshot for BoardId: {BoardId}, Generation: {Generation}", id, snap.Generation);
            
            try
            {
                return Deserialize(snap.Data);
            }
            catch (Exception ex) when (ex is EndOfStreamException || ex is IOException)
            {
                _logger?.LogError(ex, "Failed to deserialize board data for BoardId: {BoardId}", id);
                throw new InvalidOperationException($"Failed to deserialize board data for BoardId: {id}. Data may be corrupted.", ex);
            }
        }

        public async Task SaveSnapshotAsync(string id, long generation, Board b, CancellationToken ct)
        {
            _logger?.LogInformation("Saving snapshot for BoardId: {BoardId}, Generation: {Generation}", id, generation);
            
            _db.Snapshots.Add(new SnapshotEntity { BoardId = id, Generation = generation, Data = Serialize(b) });
            await _db.SaveChangesAsync(ct);
            
            _logger?.LogInformation("Successfully saved snapshot for BoardId: {BoardId}, Generation: {Generation}", id, generation);
        }

        private static byte[] Serialize(Board b)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(b.Width);
            bw.Write(b.Height);
            var alive = b.Alive().ToArray();
            bw.Write(alive.Length);
            for (int i = 0; i < alive.Length; i++) 
            { 
                bw.Write(alive[i].x); 
                bw.Write(alive[i].y); 
            }
            bw.Flush();
            return ms.ToArray();
        }

        private static Board Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty", nameof(data));

            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            
            // Ensure we have at least enough data for width, height, and count
            if (data.Length < 12) // 3 integers * 4 bytes each
                throw new InvalidDataException("Data is too short to contain valid board information");

            var w = br.ReadInt32();
            var h = br.ReadInt32();
            var n = br.ReadInt32();
            
            // Validate the dimensions
            if (w <= 0 || h <= 0)
                throw new InvalidDataException($"Invalid board dimensions: {w}x{h}");
                
            if (n < 0)
                throw new InvalidDataException($"Invalid number of alive cells: {n}");
                
            // Ensure we have enough data for all the cells
            var expectedDataLength = 12 + (n * 8); // 3 ints + n pairs of ints
            if (data.Length < expectedDataLength)
                throw new InvalidDataException($"Data length {data.Length} is insufficient for {n} cells. Expected at least {expectedDataLength} bytes.");

            var cells = new (int, int)[n];
            for (int i = 0; i < n; i++) 
            {
                var x = br.ReadInt32();
                var y = br.ReadInt32();
                cells[i] = (x, y);
            }
            
            return new Board(w, h, cells);
        }
    }
}
