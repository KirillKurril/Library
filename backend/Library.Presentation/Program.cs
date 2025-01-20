using Library.Presentation.Middleware;
using Library.Presentation.Swagger;
using Library.Infrastructure;

namespace Library.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddSwaggerConfiguration();

            var app = builder.Build();

            ConfigureMiddleware(app);
            ConfigureEndpoints(app);

            app.Run();
        }
        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");
            app.UseStaticFiles();
            app.UseHttpsRedirection();

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
