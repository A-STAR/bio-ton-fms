using BioTonFMSApp.Dtos.Auth;
using FluentValidation;

namespace BioTonFMSApp.Validation;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().Length(3, 16);
        RuleFor(x => x.Password).NotEmpty();
    }
}