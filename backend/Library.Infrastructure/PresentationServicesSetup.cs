using Library.Application.Common.Interfaces;
using Library.Presentation.Services;
using Library.Presentation.Services.BookImage;
using Library.Persistance.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Library.Domain.Abstractions;

namespace Library.Infrastructure;

public static class PresentationServicesSetup
{
    public static IServiceCollection AddPresentationServicesSetup(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IBookImageService, LocalBookImageService>();
        services.AddSingleton<IEmailSenderService, EmailSenderService>();
        services.AddHostedService<DebtorNotifierService>();

        return services;
    }
}