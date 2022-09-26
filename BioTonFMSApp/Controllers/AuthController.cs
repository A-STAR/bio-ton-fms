using BioTonFMS.Domain.Identity;
using BioTonFMSApp.Authentication;
using BioTonFMSApp.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BioTonFMSApp.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : AuthorizedControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtGenerator _jwtGenerator;

    public AuthController(UserManager<AppUser> userManager, JwtGenerator jwtGenerator)
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
            UserName = userRegistration.Email,
            Email = userRegistration.Email,
        };
        
        var result = await _userManager.CreateAsync(user, userRegistration.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        try
        {
            var tokens = _jwtGenerator.GenerateTokens(user);
            return Ok(new TokenDto(tokens.AccessToken, tokens.RefreshToken));
        }
        catch (Exception)
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginDto userLogin)
    {
        var user = await _userManager.FindByEmailAsync(userLogin.Email);
        if (user is not null)
        {
            var success = await _userManager.CheckPasswordAsync(user, userLogin.Password);
            if (success)
            {
                var tokens = _jwtGenerator.GenerateTokens(user);
                return Ok(new TokenDto(tokens.AccessToken, tokens.RefreshToken));
            }
        }

        return Unauthorized("Неверный Email или пароль");
    }
    
    [HttpPost("refresh")]
    [Authorize(Policy = "JWT.RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        var user = await _userManager.FindByIdAsync(GetUserStringId());
        if (user is null)
        {
            return Unauthorized("Пользователь не найден или невалидный токен");
        }
        
        var tokens = _jwtGenerator.GenerateTokens(user);
        return Ok(new TokenDto(tokens.AccessToken, tokens.RefreshToken));
    }

    [HttpGet("user")]
    public async Task<UserShortInfoDto> GetShortUserInfo()
    {
        var user = await _userManager.FindByIdAsync(GetUserStringId());
        if (user is null)
        {
            throw new InvalidOperationException("Пользователь не найден");
        }

        return new UserShortInfoDto(user.LastName, user.FirstName, user.MiddleName, user.Email);
    }
}