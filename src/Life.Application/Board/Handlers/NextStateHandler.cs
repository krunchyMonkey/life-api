using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using Life.Domain.Services;
using Life.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Life.Application.Board.Handlers
{
    public sealed class NextStateHandler : IRequestHandler<NextState, BoardDto>
    {
        private readonly IBoardRepository _boardRepository;
        private readonly Game _game;
        private readonly ILogger<NextStateHandler>? _logger;

        public NextStateHandler(IBoardRepository boardRepository, Game game, ILogger<NextStateHandler>? logger = null) 
        { 
            _boardRepository = boardRepository; 
            _game = game; 
            _logger = logger;
        }

        public async Task<BoardDto> Handle(NextState request, CancellationToken ct)
        {
            _logger?.LogInformation("Processing NextState request for BoardId: {BoardId}", request.Request.BoardId);
            
            var b = await _boardRepository.GetAsync(request.Request.BoardId, ct);
            if (b is null) 
            {
                _logger?.LogWarning("Board not found for BoardId: {BoardId}", request.Request.BoardId);
                throw new KeyNotFoundException($"Board with ID '{request.Request.BoardId}' was not found.");
            }
            
            _logger?.LogInformation("Found board for BoardId: {BoardId}, computing next generation", request.Request.BoardId);
            
            var next = _game.Next(b);
            await _boardRepository.SaveSnapshotAsync(request.Request.BoardId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), next, ct);
            
            _logger?.LogInformation("Successfully computed and saved next generation for BoardId: {BoardId}", request.Request.BoardId);
            
            return next.ToDto();
        }
    }
}
