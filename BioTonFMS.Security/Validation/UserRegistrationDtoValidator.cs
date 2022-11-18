using BioTonFMS.Security.Dtos.Auth;
using FluentValidation;

namespace BioTonFMSApp.Validation;

public class UserRegistrationDtoValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).Matches("^[А-Яа-яa-zA-Z]*$").WithMessage("Имя пользователя имеет неверный формат, поле допускает использование русских и латинских букв");
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).Matches("^[А-Яа-яa-zA-Z]*$").WithMessage("Фамилия имеет неверный формат, поле допускает использование русских и латинских букв");
        RuleFor(x => x.MiddleName).MaximumLength(50).Matches("^[А-Яа-яa-zA-Z]*$").WithMessage("Отчество имеет неверный формат, поле допускает использование русских и латинских букв");
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(20).Matches("^[А-Яа-яa-zA-Z1-9-_]*$").WithMessage("Логин имеет неверный формат, поле допускает использование русских и латинских букв, цифр, символов '-' и '_'");
        RuleFor(x => x.Password).NotEmpty();
    }
}