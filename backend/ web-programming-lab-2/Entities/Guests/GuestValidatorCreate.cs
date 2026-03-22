using FluentValidation;

namespace web_programming_lab_2.Entities.Guests;

public class GuestValidatorCreate : AbstractValidator<GuestDtoCreate>
{
    public GuestValidatorCreate()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}