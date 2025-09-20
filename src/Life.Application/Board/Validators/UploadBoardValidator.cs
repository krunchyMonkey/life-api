using FluentValidation;
using Life.Application.Board.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Application.Board.Validators
{
    public sealed class UploadBoardValidator : AbstractValidator<UploadRequest>
    {
        public UploadBoardValidator()
        {
            RuleFor(x => x.Width).GreaterThan(0);
            RuleFor(x => x.Height).GreaterThan(0);
            RuleForEach(x => x.Alive).Must(a => a != null && a.Length == 2);
            RuleFor(x => x).Must(x => x.Alive.All(a => a[0] >= 0 && a[1] >= 0 && a[0] < x.Width && a[1] < x.Height));
        }
    }
}
