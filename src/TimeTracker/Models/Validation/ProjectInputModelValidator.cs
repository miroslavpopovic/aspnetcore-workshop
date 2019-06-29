using FluentValidation;

namespace TimeTracker.Models.Validation
{
    public class ProjectInputModelValidator : AbstractValidator<ProjectInputModel>
    {
        public ProjectInputModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 100);

            RuleFor(x => x.ClientId)
                .NotEmpty();
        }
    }
}
