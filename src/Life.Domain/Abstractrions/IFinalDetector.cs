using Life.Domain.Models;
using Life.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Domain.Abstractrions
{
    public interface IFinalDetector
    {
        FinalResult Detect(Board start, Func<Board, Board> next, int maxIterations, TimeSpan maxTime);
    }
}
