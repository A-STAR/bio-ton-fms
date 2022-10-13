using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Security.Dtos.Auth;

/// <summary>
/// Информация о текущем пользователе
/// </summary>
/// <param name="LastName"></param>
/// <param name="FirstName"></param>
/// <param name="MiddleName"></param>
/// <param name="UserName"></param>
public record UserShortInfoDto(string LastName, string FirstName, string? MiddleName, string UserName)
{
    /// <summary>
    /// Фамилия
    /// </summary>
    [Required]
    public string LastName { get; init; } = LastName;
    /// <summary>
    /// Имя
    /// </summary>
    [Required]
    public string FirstName { get; init; } = FirstName;
    /// <summary>
    /// Отчество
    /// </summary>
    public string? MiddleName { get; init; } = MiddleName;
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]
    public string UserName { get; init; } = UserName;
}