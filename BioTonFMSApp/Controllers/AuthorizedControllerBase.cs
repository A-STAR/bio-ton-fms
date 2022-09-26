using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioTonFMSApp.Controllers;

[Authorize]
public abstract class AuthorizedControllerBase : ControllerBase
{
    protected int GetUserId()
    {
        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException(
            "�� ������� �������� UserId ��� � Identity ��� claims ���� NameIdentifier");
    }
    
    protected string GetUserStringId(bool required = true)
    {
        var userStringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (required && userStringId is null)
        {
            throw new InvalidOperationException(
                "�� ������� �������� UserId ��� � Identity ��� claims ���� NameIdentifier"); 
        }

        return userStringId;
    }
}