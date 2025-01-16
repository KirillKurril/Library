using Library.Application;
using Library.Application.Common.Settings;
using Library.Infrastructure.Authentication;
using Library.Infrastructure.Caching;
using Library.Infrastructure.Persistance;
using Library.Infrastructure.Routing;
using Library.Infrastructure.Security;
using Library.Infrastructure.Swagger;
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

            services.AddSingleton(keycloakSettings);

            services
                .AddRoutingConfiguration()
                .AddApplication()
                .AddPersistence(configuration)
                .AddTokenConfiguration(keycloakSettings)
                .AddPresentationServicesSetup()
                .AddKeycloakAuthentication(configuration)
                .AddRedisCache(configuration)
                .AddCorsPolicy()
                .AddSwaggerConfiguration()
                .InitializeDatabase();

            return services;
        }
    }
}
