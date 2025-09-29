using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using Life.Domain.Repositories;
using MediatR;

namespace Life.Application.Board.Handlers
{
    public sealed class UploadBoardHandler : IRequestHandler<UploadBoard, UploadResponse>
    {
        private readonly IBoardRepository _boardRepository;

        public UploadBoardHandler(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public async Task<UploadResponse> Handle(UploadBoard request, CancellationToken ct)
        {
            var board = request.Request.ToBoard();
            var boardId = await _boardRepository.CreateAsync(board, ct);
            return new UploadResponse(boardId);
        }
    }
}
