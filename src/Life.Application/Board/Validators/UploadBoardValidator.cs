using FluentValidation;
using Life.Application.Board.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Validators
{
    public sealed class UploadBoardValidator : AbstractValidator<UploadBoard>
    {
        public UploadBoardValidator()
        {
            RuleFor(x => x.Request.Width).GreaterThan(0).WithMessage("Board width must be greater than 0");
            RuleFor(x => x.Request.Height).GreaterThan(0).WithMessage("Board height must be greater than 0");
            RuleForEach(x => x.Request.Alive).Must(a => a != null && a.Length == 2).WithMessage("Each alive cell must have exactly 2 coordinates");
            RuleFor(x => x.Request).Must(r => r.Alive.All(a => a[0] >= 0 && a[1] >= 0 && a[0] < r.Width && a[1] < r.Height))
                .WithMessage("All alive cells must be within board boundaries");
        }
    }
}
