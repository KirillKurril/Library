using Library.Application;
using Library.Application.Common.Settings;
using Library.Infrastructure.Authentication;
using Library.Infrastructure.Startup.Authentication;
using Library.Infrastructure.Startup.Caching;
using Library.Infrastructure.Startup.InfrastructureSe;
using Library.Infrastructure.Startup.Persistance;
using Library.Infrastructure.Startup.Routing;
using Library.Infrastructure.Startup.Security;
using Library.Persistense;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var keycloakSettings = configuration
                .GetSection("Keycloak")
                .Get<KeycloakSettings>();

            var accessTokenKey = configuration
                .GetValue<string>("Redis:AccessTokenKey");

            services.AddSingleton(keycloakSettings);

            services
                .AddRoutingConfiguration()
                .AddApplication()
                .AddPersistence(configuration)
                .AddTokenConfiguration(keycloakSettings)
                .AddImageHandler()
                .AddKeycloakAuthentication(keycloakSettings)
                .AddRedisCache(configuration)
                .AddCorsPolicy()
                .AddEmailServices()
                .AddEmailServices()
                .AddLibrarySettings()
                .AddSmtpSettings()
                .InitializeDatabase();

            return services;
        }
    }
}
