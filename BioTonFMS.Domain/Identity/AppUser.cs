using Microsoft.AspNetCore.Identity;

namespace BioTonFMS.Domain.Identity;

public class AppUser : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
}