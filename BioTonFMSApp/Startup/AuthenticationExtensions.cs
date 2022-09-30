using BioTonFMS.Domain.Identity;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Security.Authentication;
using BioTonFMS.Security.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BioTonFMSApp.Startup;

public static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
        builder.Services.Configure<JwtBearerOptions>(builder.Configuration.GetSection(JwtSettings.SectionName));
        
        builder.Services.AddSingleton<JwtGenerator>();

        builder.Services.AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<BioTonDBContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    var config = new JwtSettings();
                    builder.Configuration.GetSection(JwtSettings.SectionName).Bind(config);
                    builder.Configuration.Bind(JwtSettings.SectionName, options);
                    options.TokenValidationParameters.IssuerSigningKey = config.SigningCredentials.Key;
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(JwtGenerator.RefreshTokenClaimType, policy =>
            {
                policy.AuthenticationSchemes = new List<string> {JwtBearerDefaults.AuthenticationScheme};
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(JwtGenerator.RefreshTokenClaimType);
            });
            options.AddPolicy(JwtGenerator.AccessTokenClaimType, policy =>
            {
                policy.AuthenticationSchemes = new List<string> {JwtBearerDefaults.AuthenticationScheme};
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    if (context.Resource is AuthorizationFilterContext afc)
                    {
                        var claimRequirements = afc.Filters.OfType<AuthorizeFilter>()
                            .SelectMany(x => x.Policy?.Requirements.OfType<ClaimsAuthorizationRequirement>() ?? ArraySegment<ClaimsAuthorizationRequirement>.Empty)
                            .GroupBy(x => x.ClaimType);

                        if (claimRequirements.Any(x => x.Key == JwtGenerator.RefreshTokenClaimType))
                        {
                            foreach (var claimRequirement in claimRequirements.Where(x => x.Key == JwtGenerator.AccessTokenClaimType).SelectMany(r => r))
                            {
                                context.Succeed(claimRequirement);
                            }
                        }
                    }
                    
                    return true;
                });
            });
            options.DefaultPolicy = options.GetPolicy(JwtGenerator.AccessTokenClaimType)!;
        });
        
        return builder;
    }
}