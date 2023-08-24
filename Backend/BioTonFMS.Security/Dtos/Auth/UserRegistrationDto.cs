using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Security.Dtos.Auth;
/// <summary>
/// Данные для регистрации пользователя
/// </summary>
/// <param name="LastName"></param>
/// <param name="FirstName"></param>
/// <param name="MiddleName"></param>
/// <param name="UserName"></param>
/// <param name="Password"></param>
public record UserRegistrationDto([Required] string LastName, [Required] string FirstName, string? MiddleName, [Required] string UserName, [Required] string Password)
{
    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; init; } = LastName;
    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; init; } = FirstName;
    /// <summary>
    /// Отчество
    /// </summary>
    public string? MiddleName { get; init; } = MiddleName;
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string UserName { get; init; } = UserName;
    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; init; } = Password;

}
