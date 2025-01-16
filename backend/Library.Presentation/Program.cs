using Library.Application.Common.Settings;
using Library.Persistense;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Library.Application;
using Library.Domain.Abstractions;
using Library.Presentation.Services;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Library.Application.Common.Interfaces;
using Library.Presentation.Services.BookImage;
using Library.Presentation.Middleware;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Library.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            
            var app = builder.Build();
            ConfigureMiddleware(app);
            ConfigureEndpoints(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {

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
