using BioTonFMS.Domain.Identity;
using BioTonFMSApp.Authentication;
using BioTonFMSApp.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMSApp.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : AuthorizedControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtGenerator _jwtGenerator;

    public AuthController(
        ILogger<AuthController> logger,
        UserManager<AppUser> userManager, 
        JwtGenerator jwtGenerator): base(logger)
    {
        _userManager = userManager;
        _jwtGenerator = jwtGenerator;
    }
    
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserRegistrationDto userRegistration)
    {
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

        try
        {
            var tokens = _jwtGenerator.GenerateTokens(user);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await _userManager.DeleteAsync(user);
            throw;
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginDto userLogin)
    {
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
                    _logger.LogError(ex.Message);
                    throw;
                }
            }
        }

        return Unauthorized("Неверный Email или пароль");
    }

    [HttpPost("refresh")]
    [Authorize(Policy = JwtGenerator.RefreshTokenClaimType)]
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
            _logger.LogError(ex.Message);
            throw;
        }
    }

    [HttpGet("user")]
    public async Task<UserShortInfoDto> GetShortUserInfo()
    {
        var user = await _userManager.FindByIdAsync(GetUserStringId());
        
        if (user is null)
        {
            _logger.LogError("Пользователь не найден");
            throw new InvalidOperationException("Пользователь не найден");
        }

        return new UserShortInfoDto(user.LastName, user.FirstName, user.MiddleName, user.UserName);
    }
}