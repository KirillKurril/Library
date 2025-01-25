using Library.Application.Common.Interfaces;
using Library.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Startup.InfrastructureSe;

public static class PresentationServicesSetup
{
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services)
    {
        services.AddScoped<IBookImageService, LocalBookImageService>();
        services.AddSingleton<IEmailSenderService, EmailSenderService>();
        services.AddHostedService<DebtorNotifierService>();

        return services;
    }

    public static IServiceCollection AddImageHandler(
    this IServiceCollection services)
    {
        services.AddScoped<IBookImageService, LocalBookImageService>();
        return services;
    }

    public static IServiceCollection AddLibrarySettings(
    this IServiceCollection services)
    {
        services.AddSingleton<ILibrarySettings, LibrarySettingsLocalFile>();
        return services;
    }

    public static IServiceCollection AddSmtpSettings(
    this IServiceCollection services)
    {
        services.AddSingleton<ISmtpSettings, SmtpSettingsLocalFile>();
        return services;
    }

}