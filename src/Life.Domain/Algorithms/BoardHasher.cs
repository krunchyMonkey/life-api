using Life.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Domain.Algorithms
{
    public sealed class BoardHasher
    {
        public ulong Hash(Board b)
        {
            ulong h = 1469598103934665603UL;
            foreach (var c in b.Alive())
            {
                h ^= (ulong)(c.x + 397 * c.y);
                h *= 1099511628211UL;
            }
            h ^= ((ulong)b.Width << 32) ^ (ulong)b.Height;
            return h;
        }
    }

}
