using Life.Application.Board.Commands;
using Life.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Handlers
{
    public sealed class UploadBoardHandler : IRequestHandler<UploadBoard, string>
    {
        private readonly IBoardRepository _boardRespository;
        public UploadBoardHandler(IBoardRepository boardRespository) => _boardRespository = boardRespository;
        public Task<string> Handle(UploadBoard request, CancellationToken ct) => _boardRespository.CreateAsync(request.Request.ToBoard(), ct);
    }
}
