using Library.Application.Common.Interfaces;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Persistance
{
    public static class DbSetup
    {
        public static async Task<IServiceCollection> InitializeDatabase(
            this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
                await dbInitializer.Seed();
            }
            return services;
        }
    }
}
