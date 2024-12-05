using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Library.Persistance.Repositories;
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
        services.AddSingleton<IUnitOfWork, EfUnitOfWork>();
        services.AddTransient<IRepository<Book>, EfRepository<Book>>();
        services.AddTransient<IRepository<Author>, EfRepository<Author>>();
        services.AddTransient<IRepository<Genre>, EfRepository<Genre>>();
        services.AddScoped<AppDbContext>();
        services.AddDbContext<AppDbContext>(opts =>
        {
            var connectionString = configuration.GetConnectionString("MicrosoftSQLServer");
            opts.UseSqlServer(connectionString);
        });
        return services;
    }
}