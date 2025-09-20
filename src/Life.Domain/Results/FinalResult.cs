using Life.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Domain.Results
{
    public sealed record FinalResult(Board Board, bool Stable, bool Cyclic, int Iterations, int? CycleLength);
}
