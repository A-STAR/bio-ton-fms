using Microsoft.AspNetCore.Identity;

namespace BioTonFMS.Domain.Identity;

public class AppUser : IdentityUser<int>
{
    public string FirstName { get; set; } = String.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = String.Empty;
}