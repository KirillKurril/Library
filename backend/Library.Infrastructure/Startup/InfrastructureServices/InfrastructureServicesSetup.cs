using Library.Application.Common.Interfaces;
using Library.Presentation.Services;
using Library.Presentation.Services.BookImage;
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
}