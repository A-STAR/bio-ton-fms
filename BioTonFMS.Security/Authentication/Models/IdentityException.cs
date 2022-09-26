using Microsoft.AspNetCore.Identity;

namespace BioTonFMSApp.Authentication.Models;

public class IdentityException : Exception
{
    public IdentityException(IEnumerable<IdentityError> identityErrors)
    {
        Errors = identityErrors;
    }
    
    public IEnumerable<IdentityError> Errors { get; set; }
}