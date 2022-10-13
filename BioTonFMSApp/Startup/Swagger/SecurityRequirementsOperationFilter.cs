using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BioTonFMSApp.Startup.Swagger;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.ApiDescription.TryGetMethodInfo(out MethodInfo methodInfo)) return;

        var actionAttributes = methodInfo.GetCustomAttributes().ToArray();
        var controllerAttributes = methodInfo.DeclaringType?.GetCustomAttributes().ToArray() ?? Array.Empty<Attribute>();

        if (actionAttributes.OfType<AllowAnonymousAttribute>().Any()) return;

        var controllerScopes = controllerAttributes
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.Policy);

        var actionScopes = actionAttributes
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.Policy);

        var requiredScopes = controllerScopes.Union(actionScopes).Distinct().ToArray();

        if (requiredScopes.Any())
        {
            operation.Responses.Add("401", new OpenApiResponse {Description = "Пользователь не авторизован в системе"});
            operation.Responses.Add("403", new OpenApiResponse {Description = "Доступ запрещён"});

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        requiredScopes
                    }
                }
            };
        }
    }
}