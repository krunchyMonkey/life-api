using Life.Application.Board.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Commands
{
    public sealed record NAhead(NAheadRequest Request) : IRequest<BoardDto>;
}
