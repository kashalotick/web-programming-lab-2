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
            .GreaterThan(x => x.CheckIn)
            .WithMessage("Check-out date must be greater than check-in date.");
        RuleFor(x => x.GuestCount)
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.GrandTotal)
            .GreaterThan(0);
    }
}
public class ReservationValidatorUpdate : AbstractValidator<ReservationDtoUpdate>
{
    public ReservationValidatorUpdate()
    {
        RuleFor(x => x.CheckIn)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Check-in date must be greater than today's date.");
        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn)
            .WithMessage("Check-out date must be greater than check-in date.");
        RuleFor(x => x.GuestCount)
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.GrandTotal)
            .GreaterThan(0);
    }
}