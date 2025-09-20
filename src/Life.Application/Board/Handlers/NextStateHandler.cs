using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using Life.Domain.Services;
using Life.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Handlers
{
    public sealed class NextStateHandler : IRequestHandler<NextState, BoardDto>
    {
        private readonly IBoardRepository _boardRespository;
        private readonly Game _game;
        public NextStateHandler(IBoardRepository boardRespository, Game game) { _boardRespository = boardRespository; _game = game; }
        public async Task<BoardDto> Handle(NextState request, CancellationToken ct)
        {
            var b = await _boardRespository.GetAsync(request.Request.BoardId, ct);
            if (b is null) throw new KeyNotFoundException();
            var next = _game.Next(b);
            await _boardRespository.SaveSnapshotAsync(request.Request.BoardId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), next, ct);
            return next.ToDto();
        }
    }
}
