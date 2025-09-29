using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using Life.Domain.Repositories;
using MediatR;

namespace Life.Application.Board.Handlers
{
    public sealed class NAheadHandler : IRequestHandler<NAhead, BoardDto>
    {
        private readonly IBoardRepository _boardRepository;
        
        public NAheadHandler(IBoardRepository boardRepository) 
        { 
            _boardRepository = boardRepository; 
        }
        
        public async Task<BoardDto> Handle(NAhead request, CancellationToken ct)
        {
            var board = await _boardRepository.GetAsync(request.Request.BoardId, ct);
            if (board is null) 
                throw new KeyNotFoundException($"Board with ID '{request.Request.BoardId}' was not found.");
            
            // ✅ CORRECT: Application layer only talks to Aggregates
            var advancedBoard = board.AdvanceGenerations(request.Request.N);
            await _boardRepository.SaveSnapshotAsync(request.Request.BoardId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), advancedBoard, ct);
            
            return advancedBoard.ToDto();
        }
    }
}
