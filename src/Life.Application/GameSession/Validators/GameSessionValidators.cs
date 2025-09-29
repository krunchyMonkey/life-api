using FluentValidation;
using Life.Application.GameSession.Commands;

namespace Life.Application.GameSession.Validators
{
    public sealed class CreateSessionValidator : AbstractValidator<CreateSession>
    {
        public CreateSessionValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Session name is required")
                .MaximumLength(100).WithMessage("Session name cannot exceed 100 characters");
                
            RuleFor(x => x.Request.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required");
                
            RuleFor(x => x.Request.Width)
                .GreaterThan(0).WithMessage("Board width must be greater than 0");
                
            RuleFor(x => x.Request.Height)
                .GreaterThan(0).WithMessage("Board height must be greater than 0");
                
            RuleFor(x => x.Request.InitialCells)
                .Must(cells => cells.All(c => c.X >= 0 && c.Y >= 0))
                .WithMessage("All cell coordinates must be non-negative")
                .When(x => x.Request.InitialCells.Any());
                
            RuleFor(x => x.Request)
                .Must(r => r.InitialCells.All(c => c.X < r.Width && c.Y < r.Height))
                .WithMessage("All cells must be within board boundaries")
                .When(x => x.Request.InitialCells.Any());
        }
    }

    public sealed class CreateSessionFromPatternValidator : AbstractValidator<CreateSessionFromPattern>
    {
        public CreateSessionFromPatternValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Session name is required")
                .MaximumLength(100).WithMessage("Session name cannot exceed 100 characters");
                
            RuleFor(x => x.Request.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required");
                
            RuleFor(x => x.Request.Width)
                .GreaterThan(0).WithMessage("Board width must be greater than 0");
                
            RuleFor(x => x.Request.Height)
                .GreaterThan(0).WithMessage("Board height must be greater than 0");
                
            RuleFor(x => x.Request.Pattern)
                .IsInEnum().WithMessage("Invalid pattern type");
        }
    }

    public sealed class AdvanceGenerationsValidator : AbstractValidator<AdvanceGenerations>
    {
        public AdvanceGenerationsValidator()
        {
            RuleFor(x => x.Request.SessionId)
                .NotEmpty().WithMessage("Session ID is required");
                
            RuleFor(x => x.Request.Count)
                .GreaterThan(0).WithMessage("Generation count must be greater than 0")
                .LessThanOrEqualTo(10000).WithMessage("Generation count cannot exceed 10,000");
        }
    }

    public sealed class RunToCompletionValidator : AbstractValidator<RunToCompletion>
    {
        public RunToCompletionValidator()
        {
            RuleFor(x => x.Request.SessionId)
                .NotEmpty().WithMessage("Session ID is required");
                
            RuleFor(x => x.Request.MaxIterations)
                .GreaterThan(0).WithMessage("Max iterations must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Max iterations cannot exceed 100,000");
                
            RuleFor(x => x.Request.MaxTimeMinutes)
                .GreaterThan(0).WithMessage("Max time must be greater than 0")
                .LessThanOrEqualTo(60).WithMessage("Max time cannot exceed 60 minutes");
        }
    }

    public sealed class CompareSessionsValidator : AbstractValidator<CompareSessions>
    {
        public CompareSessionsValidator()
        {
            RuleFor(x => x.Request.Session1Id)
                .NotEmpty().WithMessage("Session1 ID is required");
                
            RuleFor(x => x.Request.Session2Id)
                .NotEmpty().WithMessage("Session2 ID is required");
                
            RuleFor(x => x.Request)
                .Must(r => r.Session1Id != r.Session2Id)
                .WithMessage("Cannot compare a session with itself");
                
            RuleFor(x => x.Request.GenerationsToCompare)
                .GreaterThan(0).WithMessage("Generations to compare must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Generations to compare cannot exceed 100");
        }
    }
}