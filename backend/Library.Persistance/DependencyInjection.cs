using Library.Application.Common.Interfaces;
using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Library.Persistance.Repositories;
using Library.Persistance.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Persistense;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddSingleton<ILibrarySettings, LibrarySettingsService>();
        services.AddTransient<IRepository<Book>, EfRepository<Book>>();
        services.AddTransient<IDbInitializer, DbInitializer>();
        services.AddTransient<IRepository<Author>, EfRepository<Author>>();
        services.AddTransient<IRepository<Genre>, EfRepository<Genre>>();
        services.AddTransient<IRepository<BookLending>, EfRepository<BookLending>>();

        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseSqlServer(configuration.GetConnectionString("MicrosoftSQLServer"));
        });
        
        return services;
    }
}