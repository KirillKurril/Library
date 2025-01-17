using Library.Application.Common.Interfaces;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Library.Infrastructure.Startup.Persistance
{
    internal class DatabaseInitializer { }

    public static class DbSetup
    {
        public static async Task<IServiceCollection> InitializeDatabase(
            this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<DatabaseInitializer>>();
                try 
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

#if DEBUG
                    try
                    {
                        // Удаляем все внешние ключи
                        await context.Database.ExecuteSqlRawAsync(@"
                            DECLARE @sql NVARCHAR(MAX) = N'';
                            SELECT @sql += N'
                                ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id))
                                + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
                                ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                            FROM sys.foreign_keys;
                            EXEC sp_executesql @sql;
                        ");

                        foreach (var entityType in context.Model.GetEntityTypes())
                        {
                            var tableName = entityType.GetTableName();
                            if (tableName != null)
                            {
                                await context.Database.ExecuteSqlRawAsync($"DROP TABLE IF EXISTS [{tableName}]");
                            }
                        }
                        await context.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS [__EFMigrationsHistory]");

                        await context.Database.MigrateAsync();
                        await dbInitializer.Seed();
                    }
                    catch (Exception ex)
                    {
                        logger?.LogWarning($"Error during database cleanup: {ex.Message}");
                    }
#else
                    await context.Database.MigrateAsync();
#endif
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Critical error during database initialization");
                    throw;
                }
            }
            return services;
        }
    }
}
