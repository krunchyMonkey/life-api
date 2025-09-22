using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Contracts
{
    public sealed record UploadRequest(int Width, int Height, int[][] Alive);
    public sealed record UploadResponse(string BoardId);
    public sealed record NextRequest(string BoardId);
    public sealed record NAheadRequest(string BoardId, long N);
    public sealed record FinalRequest(string BoardId);
    public sealed record BoardDto(int Width, int Height, int[][] Alive);
    public sealed record FinalDto(BoardDto Board, bool Stable, bool Cyclic, int Iterations, int? CycleLength);
}
