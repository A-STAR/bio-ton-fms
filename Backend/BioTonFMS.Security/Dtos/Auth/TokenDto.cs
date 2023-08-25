namespace BioTonFMS.Security.Dtos.Auth;
/// <summary>
/// Набор токенов для авторизации
/// </summary>
/// <param name="AccessToken"></param>
/// <param name="RefreshToken"></param>
public record TokenDto(string AccessToken, string RefreshToken)
{
    /// <summary>
    /// JWT Токен авторизации
    /// </summary>
    public string AccessToken { get; init; } = AccessToken;

    /// <summary>
    /// Токен обновления
    /// </summary>
    public string RefreshToken { get; init; } = RefreshToken;
}
