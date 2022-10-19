using BioTonFMS.Domain;
using BioTonFMS.Security.Controllers;
using BioTonFMS.Telematica.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace BioTonFMSApp.Startup.Swagger;

public static class SwaggerExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerGeneratorOptions.IgnoreObsoleteActions = true;
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "BioTon FMS API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description =
                    "JWT Authorization заголовок использующий схему Bearer. \r\n\r\n Введите 'Bearer' [пробел] и затем ваш токен авторизации в поле ввода внизу.\r\n\r\nПример: \"Bearer 12345abcdef\""
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();

            AddXmlCommentsForAssembly(options, Assembly.GetExecutingAssembly());
            AddXmlCommentsForAssembly(options, typeof(AuthController).Assembly);
            AddXmlCommentsForAssembly(options, typeof(TrackerController).Assembly);
            AddXmlCommentsForAssembly(options, typeof(Vehicle).Assembly);
        });

        return builder;
    }

    private static void AddXmlCommentsForAssembly(SwaggerGenOptions options, Assembly assembly)
    {
        var xmlFilename = $"{assembly.GetName().Name}.xml";
        var xmlFileWithPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        options.IncludeXmlComments(xmlFileWithPath, includeControllerXmlComments: true);
        options.SchemaFilter<EnumTypesSchemaFilter>(xmlFileWithPath);
    }
}