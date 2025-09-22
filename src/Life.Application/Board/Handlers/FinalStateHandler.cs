using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using Life.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Life.Application.Board.Handlers
{
    public sealed class FinalStateHandler : IRequestHandler<FinalState, FinalDto>
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IConfiguration _configuration;
        
        public FinalStateHandler(IBoardRepository boardRepository, IConfiguration configuration) 
        { 
            _boardRepository = boardRepository; 
            _configuration = configuration; 
        }
        
        public async Task<FinalDto> Handle(FinalState request, CancellationToken ct)
        {
            var board = await _boardRepository.GetAsync(request.Request.BoardId, ct);
            if (board is null) 
                throw new KeyNotFoundException($"Board with ID '{request.Request.BoardId}' was not found.");
            
            var maxIterations = _configuration.GetValue<int>("Final:MaxIterations");
            var maxMs = _configuration.GetValue<int>("Final:MaxMillis");
            
            // ✅ CORRECT: Application layer only talks to Aggregates
            var result = board.DetectFinalState(maxIterations, TimeSpan.FromMilliseconds(maxMs));
            await _boardRepository.SaveSnapshotAsync(request.Request.BoardId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), result.Board, ct);
            
            return new FinalDto(result.Board.ToDto(), result.Stable, result.Cyclic, result.Iterations, result.CycleLength);
        }
    }
}
