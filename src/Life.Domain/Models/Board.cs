using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Domain.Models
{
    public sealed class Board
    {
        public int Width { get; }
        public int Height { get; }
        private readonly BitArray _bits;

        public Board(int width, int height, IEnumerable<(int x, int y)> alive)
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException();
            Width = width;
            Height = height;
            _bits = new BitArray(width * height);
            foreach (var c in alive)
            {
                if (c.x < 0 || c.y < 0 || c.x >= width || c.y >= height) throw new ArgumentOutOfRangeException();
                _bits[c.y * width + c.x] = true;
            }
        }

        public bool Get(int x, int y) => _bits[y * Width + x];

        public IEnumerable<(int x, int y)> Alive()
        {
            for (var i = 0; i < _bits.Length; i++) if (_bits[i]) yield return (i % Width, i / Width);
        }
    }
}
