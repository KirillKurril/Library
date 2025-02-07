using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Library.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace Library.Infrastructure.Startup.Authentication;

public static class KeycloakConfiguration
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        KeycloakSettings keycloakSettings)
    {
        services.AddSingleton(keycloakSettings);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options =>
       {
           options.Authority = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}";
           options.MetadataAddress = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}/.well-known/openid-configuration";
           options.RequireHttpsMetadata = false;
           options.SaveToken = true;

           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}",
               ValidAudience = "aspnet-api",
               RoleClaimType = ClaimTypes.Role
           };

           options.Events = new JwtBearerEvents
           {
               OnTokenValidated = context =>
               {
                   var identity = context.Principal?.Identity as ClaimsIdentity;
                   if (identity == null) return Task.CompletedTask;

                   var resourceAccessClaim = identity.Claims
                       .FirstOrDefault(c => c.Type == "resource_access")?.Value;

                   var resourceAccess = JObject.Parse(resourceAccessClaim);
                   
                   var apiRoles = resourceAccess?["aspnet-api"]?["roles"] as JArray;

                   if(apiRoles != null)
                   foreach (var role in apiRoles.Values<string>())
                       identity.AddClaim(new Claim(ClaimTypes.Role, role));

                   return Task.CompletedTask;
               }
           };
       });

       services.AddAuthorization(options =>
       {
           options.AddPolicy("admin", p => p.RequireRole("admin"));
       });

        return services;
    }
}
