using FluentValidation;
using web_programming_lab_2.Entities.Guests;

namespace web_programming_lab_2.Entities.Reservations;

public class ReservationCreateRequestValidator : AbstractValidator<ReservationCreateRequest>
{
    public ReservationCreateRequestValidator()
    {
        RuleFor(x => x.Reservation).SetValidator(new ReservationValidatorCreate());
        RuleFor(x => x.Guest).SetValidator(new GuestValidatorCreate());
    }
}

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
            .GreaterThan(0)
            .When(x => x.GrandTotal.HasValue)
            .WithMessage("Grand total must be greater than 0.");
        ;
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