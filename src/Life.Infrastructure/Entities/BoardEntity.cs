using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Infrastructure.Entities
{
    public sealed class BoardEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
    }
}
