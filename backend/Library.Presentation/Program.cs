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

            app.Run();
        }
        private static void ConfigureMiddleware(WebApplication app)
        {
            app.UseGlobalExceptionHandling();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
