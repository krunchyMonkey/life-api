using Life.Domain.Models;
using Life.Infrastructure.Data;
using Life.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Infrastructure.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly LifeDbContext _db;

        public BoardRepository(LifeDbContext db) => _db = db;

        public async Task<string> CreateAsync(Board b, CancellationToken ct)
        {
            var id = Guid.NewGuid().ToString("n");
            _db.Boards.Add(new BoardEntity { Id = id, Width = b.Width, Height = b.Height, CreatedUtc = DateTimeOffset.UtcNow });
            _db.Snapshots.Add(new SnapshotEntity { BoardId = id, Generation = 0, Data = Serialize(b) });
            await _db.SaveChangesAsync(ct);
            return id;
        }

        public async Task<Board?> GetAsync(string id, CancellationToken ct)
        {
            var snap = await _db.Snapshots.AsNoTracking().Where(s => s.BoardId == id).OrderByDescending(s => s.Generation).FirstOrDefaultAsync(ct);
            if (snap == null) return null;
            return Deserialize(snap.Data);
        }

        public async Task SaveSnapshotAsync(string id, long generation, Board b, CancellationToken ct)
        {
            _db.Snapshots.Add(new SnapshotEntity { BoardId = id, Generation = generation, Data = Serialize(b) });
            await _db.SaveChangesAsync(ct);
        }

        private static byte[] Serialize(Board b)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(b.Width);
            bw.Write(b.Height);
            var alive = b.Alive().ToArray();
            bw.Write(alive.Length);
            for (int i = 0; i < alive.Length; i++) { bw.Write(alive[i].x); bw.Write(alive[i].y); }
            bw.Flush();
            return ms.ToArray();
        }

        private static Board Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            var w = br.ReadInt32();
            var h = br.ReadInt32();
            var n = br.ReadInt32();
            var cells = new (int, int)[n];
            for (int i = 0; i < n; i++) cells[i] = (br.ReadInt32(), br.ReadInt32());
            return new Board(w, h, cells);
        }
    }
}
