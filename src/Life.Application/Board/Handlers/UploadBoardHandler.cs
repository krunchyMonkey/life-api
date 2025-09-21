using Life.Application.Board.Commands;
using Life.Infrastructure.Repositories;
using MediatR;

namespace Life.Application.Board.Handlers
{
    public sealed class UploadBoardHandler : IRequestHandler<UploadBoard, string>
    {
        private readonly IBoardRepository _boardRepository;

        public UploadBoardHandler(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public Task<string> Handle(UploadBoard request, CancellationToken ct)
        {
            var board = request.Request.ToBoard();
            return _boardRepository.CreateAsync(board, ct);
        }
    }
}
