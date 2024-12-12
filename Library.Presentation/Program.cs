using Library.Application.Common.Settings;
using Library.Persistense;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Library.Application;
using Library.Presentation.Services;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Library.Application.Common.Interfaces;
using Library.Presentation.Services.BookImage;
using Library.Presentation.Middleware;

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

            builder.Services.AddSingleton(keycloakSettings);

            builder.Services.AddHttpClient<ITokenAccessor, TokenAccesor>(opt =>
            {
                opt.BaseAddress = new Uri($"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}/");
            });

            builder.Services.AddSingleton<IUserDataAccessor, UserDataAccessor>();
            builder.Services.AddHttpClient<IUserDataAccessor, UserDataAccessor>(opt =>
            {
                opt.BaseAddress = new Uri($"{keycloakSettings.Host}/admin/realms/{keycloakSettings.Realm}/");
            });

            builder.Services.AddApplication();
            builder.Services.AddPersistence(builder.Configuration);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}";
                options.MetadataAddress = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}",
                    ValidateLifetime = true
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", p => p.RequireRole("admin"));
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                var redisConfig = builder.Configuration.GetSection("Redis");
                options.Configuration = redisConfig["Configuration"];
                options.InstanceName = redisConfig["InstanceName"];
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
            builder.Services.AddHostedService<DebtorNotifierService>();

            builder.Services.AddScoped<IBookImageService, LocalBookImageService>();
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

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseGlobalExceptionHandling();
        }

        private static void ConfigureEndpoints(WebApplication app)
        {
            app.MapControllers();
        }
    }
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
