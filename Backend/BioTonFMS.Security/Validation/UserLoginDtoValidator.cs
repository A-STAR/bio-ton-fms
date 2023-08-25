using BioTonFMS.Security.Dtos.Auth;
using FluentValidation;

namespace BioTonFMSApp.Validation;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}