using System.Security.Claims;
using BioTonFMS.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Security.Controllers;

[Authorize]
public abstract class AuthorizedControllerBase : ValidationControllerBase
{
    public readonly ILogger<AuthController> _logger;

    public AuthorizedControllerBase(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    protected string GetUserStringId(bool required = true)
    {
        var userStringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (required && userStringId is null)
        {
            var exMessage = "Не удается получить UserId или в Identity нет claim типа NameIdentifier";
            _logger.LogError(exMessage);
            throw new InvalidOperationException(exMessage); 
        }

        return userStringId;
    }
}