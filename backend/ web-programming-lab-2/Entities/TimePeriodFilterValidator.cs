using FluentValidation;

namespace web_programming_lab_2.Entities;

public class TimePeriodFilterValidator : AbstractValidator<TimePeriodFilter>
{
    public TimePeriodFilterValidator()
    {
        RuleFor(x => x.To)
            .GreaterThan(x => x.From)
            .WithMessage("Check-out date must be greater than check-in date.");
    }
}