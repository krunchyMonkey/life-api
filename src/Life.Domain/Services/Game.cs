using Life.Domain.Abstractrions;
using Life.Domain.Models;
using Life.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Domain.Services
{
    public sealed class Game : IGame
    {
        private static readonly (int dx, int dy)[] N = { (-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1) };

        public Board Next(Board b)
        {
            var alive = new List<(int x, int y)>();
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    int n = 0;
                    for (int i = 0; i < N.Length; i++)
                    {
                        int nx = x + N[i].dx;
                        int ny = y + N[i].dy;
                        if (nx >= 0 && ny >= 0 && nx < b.Width && ny < b.Height && b.Get(nx, ny)) n++;
                    }
                    bool nextAlive = (b.Get(x, y) && (n == 2 || n == 3)) || (!b.Get(x, y) && n == 3);
                    if (nextAlive) alive.Add((x, y));
                }
            return new Board(b.Width, b.Height, alive);
        }

        public Board Advance(Board b, long n)
        {
            var cur = b;
            for (long i = 0; i < n; i++) cur = Next(cur);
            return cur;
        }

        public FinalResult Final(Board start, int maxIterations, TimeSpan maxTime)
        {
            var detector = new Algorithms.FinalDetector(new Algorithms.BoardHasher());
            return detector.Detect(start, Next, maxIterations, maxTime);
        }
    }
}
