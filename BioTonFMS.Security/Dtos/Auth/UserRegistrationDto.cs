namespace BioTonFMSApp.Dtos.Auth;

public record UserRegistrationDto(string LastName, string FirstName, string? MiddleName, string Email, string Password);