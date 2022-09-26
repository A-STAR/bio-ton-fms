using BioTonFMSApp.Dtos.Auth;
using FluentValidation;

namespace BioTonFMSApp.Validation;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}