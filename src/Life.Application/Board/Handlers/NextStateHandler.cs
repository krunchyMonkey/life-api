using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using Life.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Life.Application.Board.Handlers
{
    public sealed class NextStateHandler : IRequestHandler<NextState, BoardDto>
    {
        private readonly IBoardRepository _boardRepository;
        private readonly ILogger<NextStateHandler>? _logger;

        public NextStateHandler(IBoardRepository boardRepository, ILogger<NextStateHandler>? logger = null) 
        { 
            _boardRepository = boardRepository; 
            _logger = logger;
        }

        public async Task<BoardDto> Handle(NextState request, CancellationToken ct)
        {
            _logger?.LogInformation("Processing NextState request for BoardId: {BoardId}", request.Request.BoardId);
            
            var board = await _boardRepository.GetAsync(request.Request.BoardId, ct);
            if (board is null) 
            {
                _logger?.LogWarning("Board not found for BoardId: {BoardId}", request.Request.BoardId);
                throw new KeyNotFoundException($"Board with ID '{request.Request.BoardId}' was not found.");
            }
            
            _logger?.LogInformation("Found board for BoardId: {BoardId}, computing next generation", request.Request.BoardId);
            
            // ✅ CORRECT: Application layer only talks to Aggregates
            var nextBoard = board.NextGeneration();
            await _boardRepository.SaveSnapshotAsync(request.Request.BoardId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), nextBoard, ct);
            
            _logger?.LogInformation("Successfully computed and saved next generation for BoardId: {BoardId}", request.Request.BoardId);
            
            return nextBoard.ToDto();
        }
    }
}
