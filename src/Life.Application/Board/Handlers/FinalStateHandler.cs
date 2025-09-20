using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using Life.Domain.Services;
using Life.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Handlers
{
    public sealed class FinalStateHandler : IRequestHandler<FinalState, FinalDto>
    {
        private readonly IBoardRepository _boardRespository;
        private readonly Game _game;
        private readonly IConfiguration _cfg;
        public FinalStateHandler(IBoardRepository boardRespository, Game game, IConfiguration cfg) { _boardRespository = boardRespository; _game = game; _cfg = cfg; }
        public async Task<FinalDto> Handle(FinalState request, CancellationToken ct)
        {
            var b = await _boardRespository.GetAsync(request.Request.BoardId, ct);
            if (b is null) throw new KeyNotFoundException();
            var maxI = _cfg.GetValue<int>("Final:MaxIterations");
            var maxMs = _cfg.GetValue<int>("Final:MaxMillis");
            var r = _game.Final(b, maxI, TimeSpan.FromMilliseconds(maxMs));
            await _boardRespository.SaveSnapshotAsync(request.Request.BoardId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), r.Board, ct);
            return new FinalDto(r.Board.ToDto(), r.Stable, r.Cyclic, r.Iterations, r.CycleLength);
        }
    }
}
