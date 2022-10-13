using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Security.Dtos.Auth;

/// <summary>
/// Данные для входа пользователя в систему
/// </summary>
/// <param name="UserName"></param>
/// <param name="Password"></param>
public record UserLoginDto(string UserName, string Password)
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]    
    public string UserName { get; init; } = UserName;
    /// <summary>
    /// Пароль
    /// </summary>
    [Required]
    public string Password { get; init; } = Password;   
}
