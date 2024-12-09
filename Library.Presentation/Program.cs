using Library.Application.Common.Settings;
using Library.Persistense;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Library.Application;
using Library.Presentation.Services;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Library.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            
            var app = builder.Build();
            await InitializeDatabase(app);
            ConfigureMiddleware(app);
            ConfigureEndpoints(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var keycloakSettings = builder.Configuration
                .GetSection("Keycloak")
                .Get<KeycloakSettings>();

            builder.Services.AddApplication();
            builder.Services.AddPersistence(builder.Configuration);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.MetadataAddress = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}/.well-known/openid-configuration";
                    o.Authority = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}";
                    o.Audience = "account";
                    o.RequireHttpsMetadata = false;
                });

            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("admin", p => p.RequireRole("admin"));
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                var redisConfig = builder.Configuration.GetSection("Redis");
                options.Configuration = redisConfig["Configuration"];
                options.InstanceName = redisConfig["InstanceName"];
            });

            builder.Services.AddHostedService<OverdueBooksNotificationService>();

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IKeycloakService, KeycloakService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }

        private static async Task InitializeDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();
            }
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureEndpoints(WebApplication app)
        {
            app.MapControllers();
        }
    }
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
