using FluentValidation;

namespace web_programming_lab_2.Entities.Reservations;

public class ReservationValidatorCreate : AbstractValidator<ReservationDtoCreate>
{
    public ReservationValidatorCreate()
    {
        RuleFor(x => x.CheckIn)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Check-in date must be greater than today's date.");
        RuleFor(x => x.CheckOut)
            .GreaterThanOrEqualTo(x => x.CheckIn)
            .WithMessage("Check-out date must be greater than check-in date.");
    }
}