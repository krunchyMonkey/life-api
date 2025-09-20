using Life.Domain.Abstractrions;
using Life.Domain.Models;
using Life.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Domain.Algorithms
{
    public sealed class FinalDetector : IFinalDetector
    {
        private readonly BoardHasher _hasher;
        public FinalDetector(BoardHasher hasher) => _hasher = hasher;

        public FinalResult Detect(Board start, Func<Board, Board> next, int maxIterations, TimeSpan maxTime)
        {
            var seen = new Dictionary<ulong, int>();
            var cur = start;
            var startAt = DateTime.UtcNow;
            for (int i = 1; i <= maxIterations; i++)
            {
                var h = _hasher.Hash(cur);
                if (seen.TryGetValue(h, out var at)) return new FinalResult(cur, false, true, i, i - at);
                seen[h] = i;
                var nxt = next(cur);
                if (Equal(cur, nxt)) return new FinalResult(nxt, true, false, i, null);
                if ((DateTime.UtcNow - startAt) > maxTime) throw new TimeoutException();
                cur = nxt;
            }
            // Reached maximum iterations without finding a final state
            throw new TimeoutException($"Could not determine final state within {maxIterations} iterations");
        }

        private static bool Equal(Board a, Board b)
        {
            if (a.Width != b.Width || a.Height != b.Height) return false;
            var aa = a.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            var bb = b.Alive().OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
            if (aa.Length != bb.Length) return false;
            for (int i = 0; i < aa.Length; i++) if (aa[i] != bb[i]) return false;
            return true;
        }
    }

}
