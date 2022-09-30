using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BioTonFMS.Security.Authentication.Models;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    private string _secret = default!;
    
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(14);

    public string? Issuer { get; set; }
    public string? Audience { get; set; }

    public string Secret
    {
        set
        {
            _secret = value;
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(value));
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
        get => _secret;
    }

    public SigningCredentials SigningCredentials { get; private set; } = default!;
}