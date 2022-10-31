using System.Net;
using BioTonFMS.Telematica.Validation;
using BioTonFMSApp.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace BioTonFMSApp.Startup;

public static class ValidationExtensions
{
    [Obsolete]
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        // TODO: Поменять на автоматический сбор валидации
        builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserRegistrationDtoValidator>());
        builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserLoginDtoValidator>());
        builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateTrackerDtoValidator>());

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Type = "/bad-request",
                    Title = "Запрос не соответствует требованиям API",
                    Detail = "Произошла ошибка"
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        return builder;
    }
}