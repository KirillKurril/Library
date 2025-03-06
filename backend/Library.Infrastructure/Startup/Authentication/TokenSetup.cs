using Library.Application.Common.Interfaces;
using Library.Infrastructure.Services;
using Library.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Authentication;

public static class TokenSetup
{
    public static IServiceCollection AddTokenConfiguration(
        this IServiceCollection services,
        KeycloakSettings keycloakSettings)
    {
        services.AddScoped<ITokenAccessor, TokenAccesor>();
        services.AddHttpClient<ITokenAccessor, TokenAccesor>(opt =>
        {
            opt.BaseAddress = new Uri($"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}/");
            
        });

        services.AddScoped<IUserDataAccessor, UserDataAccessor>();
        services.AddHttpClient<IUserDataAccessor, UserDataAccessor>(opt =>
        {
            opt.BaseAddress = new Uri($"{keycloakSettings.Host}/admin/realms/{keycloakSettings.Realm}/");
        });

        return services;
    }
}