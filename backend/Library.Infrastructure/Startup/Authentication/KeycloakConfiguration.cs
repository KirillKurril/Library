using Library.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Authentication
{
    public static class KeycloakConfiguration
    {
        public static IServiceCollection AddKeycloakAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var keycloakSettings = configuration
               .GetSection("Keycloak")
               .Get<KeycloakSettings>();

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
               options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
               {
                   ValidateAudience = false,
                   ValidateIssuer = true,
                   ValidIssuer = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}",
                   ValidateLifetime = true
               };
           });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", p => p.RequireRole("admin"));
            });

            return services;
        }
    }
}
