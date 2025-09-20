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
    public sealed class NAheadHandler : IRequestHandler<NAhead, BoardDto>
    {
        private readonly IBoardRepository _boardRespository;
        private readonly Game _game;
        public NAheadHandler(IBoardRepository boardRespository, Game game) { _boardRespository = boardRespository; _game = game; }
        public async Task<BoardDto> Handle(NAhead request, CancellationToken ct)
        {
            var b = await _boardRespository.GetAsync(request.Request.BoardId, ct);
            if (b is null) throw new KeyNotFoundException();
            var adv = _game.Advance(b, request.Request.N);
            await _boardRespository.SaveSnapshotAsync(request.Request.BoardId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), adv, ct);
            return adv.ToDto();
        }
    }
}
