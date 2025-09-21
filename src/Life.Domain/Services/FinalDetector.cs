using Life.Domain.Aggregates;
using Life.Domain.ValueObjects;

namespace Life.Domain.Services
{
    /// <summary>
    /// Legacy FinalDetector that maintains backward compatibility
    /// </summary>
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
                if (seen.TryGetValue(h, out var at)) 
                    return new FinalResult(cur, false, true, i, i - at);
                
                seen[h] = i;
                var nxt = next(cur);
                
                if (cur.IsIdenticalTo(nxt)) 
                    return new FinalResult(nxt, true, false, i, null);
                
                if ((DateTime.UtcNow - startAt) > maxTime) 
                    throw new TimeoutException();
                
                cur = nxt;
            }
            
            // Reached maximum iterations without finding a final state
            throw new TimeoutException($"Could not determine final state within {maxIterations} iterations");
        }
    }
}