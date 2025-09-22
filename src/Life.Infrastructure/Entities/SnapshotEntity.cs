using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Infrastructure.Entities
{
    public sealed class SnapshotEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public string BoardId { get; set; } = "";
        public long Generation { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
