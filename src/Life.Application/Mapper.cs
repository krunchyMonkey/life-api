using Life.Application.Board.Contracts;
using Life.Domain.Aggregates;

namespace Life.Application
{
    public static class Mapper
    {
        public static Life.Domain.Aggregates.Board ToBoard(this UploadRequest r)
        {
            var cells = r.Alive.Select(a => (a[0], a[1]));
            return new Life.Domain.Aggregates.Board(r.Width, r.Height, cells);
        }

        public static BoardDto ToDto(this Life.Domain.Aggregates.Board b)
        {
            var alive = b.Alive().Select(c => new[] { c.x, c.y }).ToArray();
            return new BoardDto(b.Width, b.Height, alive);
        }
    }
}
