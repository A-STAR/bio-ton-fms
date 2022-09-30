namespace BioTonFMS.Security.Dtos.Auth;

public record UserRegistrationDto(string LastName, string FirstName, string? MiddleName, string UserName, string Password);