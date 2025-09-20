using FluentValidation;
using Life.Application.Board.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Validators
{
    public sealed class NAheadValidator : AbstractValidator<NAheadRequest>
    {
        public NAheadValidator()
        {
            RuleFor(x => x.BoardId).NotEmpty();
            RuleFor(x => x.N).GreaterThanOrEqualTo(0);
        }
    }
}
