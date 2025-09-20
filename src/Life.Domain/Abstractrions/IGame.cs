using Life.Domain.Models;
using Life.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Domain.Abstractrions
{
    public interface IGame
    {
        Board Next(Board b);
        Board Advance(Board b, long n);
        FinalResult Final(Board start, int maxIterations, TimeSpan maxTime);
    }
}
