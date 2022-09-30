using BioTonFMS.Security.Dtos.Auth;
using FluentValidation;

namespace BioTonFMSApp.Validation;

public class UserRegistrationDtoValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty().Length(3, 16);
        RuleFor(x => x.Password).NotEmpty();
    }
}