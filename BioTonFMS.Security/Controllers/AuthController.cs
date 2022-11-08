using BioTonFMS.Domain.Identity;
using BioTonFMS.Security.Authentication;
using BioTonFMS.Security.Dtos.Auth;
using BioTonFMS.Telematica.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioTonFMS.Security.Controllers;

/// <summary>
/// API аутентификации пользователя
/// </summary>
[ApiController]
[Route("/api/auth")]
[Consumes("application/json")]
[Produces("application/json")]
public class AuthController : AuthorizedControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtGenerator _jwtGenerator;
    private readonly IValidator<UserRegistrationDto> _registratioValidator;
    private readonly IValidator<UserLoginDto> _loginValidator;

    public AuthController(
        ILogger<AuthController> logger,
        UserManager<AppUser> userManager,
        JwtGenerator jwtGenerator,
        IValidator<UserRegistrationDto> registratioValidator,
        IValidator<UserLoginDto> loginValidator) : base(logger)
    {
        _userManager = userManager;
        _jwtGenerator = jwtGenerator;
        _registratioValidator = registratioValidator;
        _loginValidator = loginValidator;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="userRegistration">Информация о новом пользователе</param>
    /// <response code="200">Новый пользователь успешно создан</response>
    /// <response code="400">Невозможно зарегестрировать пользователя</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(UserRegistrationDto userRegistration)
    {
        ValidationResult validationResult = _registratioValidator.Validate(userRegistration);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var user = new AppUser
        {
            FirstName = userRegistration.FirstName,
            LastName = userRegistration.LastName,
            MiddleName = userRegistration.MiddleName,
            UserName = userRegistration.UserName,
        };

        var result = await _userManager.CreateAsync(user, userRegistration.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }

    /// <summary>
    /// Вход пользователя в систему
    /// </summary>
    /// <param name="userLogin">Данные для входа пользователя в систему</param>
    /// <returns></returns>
    /// <response code="200">Пользователь успешно вошёл в систему</response>
    /// <response code="400">Неверный логин или пароль</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(UserLoginDto userLogin)
    {
        ValidationResult validationResult = _loginValidator.Validate(userLogin);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var user = await _userManager.FindByNameAsync(userLogin.UserName);
        if (user is not null)
        {
            var success = await _userManager.CheckPasswordAsync(user, userLogin.Password);
            if (success)
            {
                try
                {
                    var tokens = _jwtGenerator.GenerateTokens(user);
                    return Ok(new TokenDto(tokens.AccessToken, tokens.RefreshToken));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при логине пользователя {@UserLogin}", userLogin);
                    throw;
                }
            }
        }

        return BadRequest("Неверный логин или пароль");
    }

    /// <summary>
    /// Обновления токена аутентификации
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Токен аутентификации успешно обновлён</response>
    [HttpPost("refresh")]
    [Authorize(Policy = JwtGenerator.RefreshTokenClaimType)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken()
    {
        var user = await _userManager.FindByIdAsync(GetUserStringId());
        if (user is null)
        {
            return Unauthorized("Пользователь не найден или невалидный токен");
        }

        try
        {
            var tokens = _jwtGenerator.GenerateTokens(user);
            return Ok(new TokenDto(tokens.AccessToken, tokens.RefreshToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении токенов");
            throw;
        }
    }

    /// <summary>
    /// Получение информации по текущему пользователю
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <response code="200">Возвращена информация о текущем пользователе</response>
    /// <response code="404">Текущий пользователь не найден</response>
    [HttpGet("user")]
    [ProducesResponseType(typeof(UserShortInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserShortInfoDto>> GetShortUserInfo()
    {
        var userId = GetUserStringId();
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
        {
            _logger.LogError("Пользователь c id = {@UserId} не найден", userId);
            return NotFound($"Пользователь c id={userId} не найден");
        }
        return new UserShortInfoDto(user.LastName, user.FirstName, user.MiddleName, user.UserName);
    }
}