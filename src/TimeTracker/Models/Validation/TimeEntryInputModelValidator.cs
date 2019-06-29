using System;
using FluentValidation;

namespace TimeTracker.Models.Validation
{
    public class TimeEntryInputModelValidator : AbstractValidator<TimeEntryInputModel>
    {
        public TimeEntryInputModelValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.ProjectId)
                .NotEmpty();

            RuleFor(x => x.EntryDate)
                .GreaterThan(new DateTime(2019, 1, 1))
                .LessThan(new DateTime(2100, 1, 1));

            RuleFor(x => x.Hours)
                .ExclusiveBetween(0, 25);

            RuleFor(x => x.HourRate)
                .InclusiveBetween(1, 1000);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(10000);
        }
    }
}
