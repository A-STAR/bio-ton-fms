using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BioTonFMS.Domain.Identity;
using BioTonFMS.Security.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BioTonFMS.Security.Authentication;

public sealed class JwtGenerator
{
    public const string AccessTokenClaimType = "JWT.AccessToken";
    public const string RefreshTokenClaimType = "JWT.RefreshToken";
    
    private readonly IOptions<ClaimsIdentityOptions> _claimsOptions;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public JwtGenerator(IOptions<JwtSettings> jwtSettings, IOptions<ClaimsIdentityOptions> claimsOptions)
    {
        _jwtSettings = jwtSettings;
        _claimsOptions = claimsOptions;
    }

    public (string AccessToken, string RefreshToken) GenerateTokens(AppUser user)
    {
        var now = DateTime.UtcNow; //TODO: Наверное лучше использовать DateTimeProvider позже 

        var options = _claimsOptions.Value;
        
        var claimId = new Claim(options.UserIdClaimType, user.Id.ToString()); 
        var accessClaims = new[]
        {
            claimId,
            // new Claim(options.UserNameClaimType, user.UserName), // потом добавим клэймы
            // new Claim(options.EmailClaimType, user.Email),
            new Claim(AccessTokenClaimType, string.Empty)
        };
        var accessToken = GenerateToken(now, _jwtSettings.Value.AccessTokenLifetime, accessClaims);
        
        var refreshClaims = new[]
        {
            claimId,
            // new Claim(_claimsOptions.Value.SecurityStampClaimType, user.SecurityStamp), // может быть полезно для валидации позже
            new Claim(RefreshTokenClaimType, string.Empty)
        };
        var refreshToken = GenerateToken(now, _jwtSettings.Value.RefreshTokenLifetime, refreshClaims);

        return (accessToken, refreshToken);
    }
    
    private string GenerateToken(DateTime validFrom, TimeSpan tokenLifeTime, params Claim[] claims)
    {
        var config = _jwtSettings.Value;
        var expireAt = validFrom.Add(tokenLifeTime);
        
        var jwtToken = new JwtSecurityToken(
            config.Issuer,
            config.Audience,
            claims,
            validFrom,
            expireAt,
            config.SigningCredentials);
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}