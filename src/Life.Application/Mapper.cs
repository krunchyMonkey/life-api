using Life.Application.Board.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Life.Domain.Models;

namespace Life.Application
{
    public static class Mapper
    {
        public static Domain.Models.Board ToBoard(this UploadRequest r)
        {
            var cells = r.Alive.Select(a => (a[0], a[1]));
            return new Domain.Models.Board(r.Width, r.Height, cells);
        }

        public static BoardDto ToDto(this Domain.Models.Board b)
        {
            var alive = b.Alive().Select(c => new[] { c.x, c.y }).ToArray();
            return new BoardDto(b.Width, b.Height, alive);
        }
    }
}
