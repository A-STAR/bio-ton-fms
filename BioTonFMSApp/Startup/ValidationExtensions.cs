using BioTonFMS.Telematica.Validation;
using BioTonFMSApp.Validation;
using FluentValidation;

namespace BioTonFMSApp.Startup;

public static class ValidationExtensions
{
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<UserRegistrationDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateTrackerDtoValidator>();

        return builder;
    }
}