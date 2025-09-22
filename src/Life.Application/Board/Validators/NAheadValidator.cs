using FluentValidation;
using Life.Application.Board.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Validators
{
    public sealed class NAheadValidator : AbstractValidator<NAhead>
    {
        public NAheadValidator()
        {
            RuleFor(x => x.Request.BoardId).NotEmpty().WithMessage("Board ID is required");
            RuleFor(x => x.Request.N).GreaterThan(0).WithMessage("Number of generations must be greater than 0");
        }
    }
}
