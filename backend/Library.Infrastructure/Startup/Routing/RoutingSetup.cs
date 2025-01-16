using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Startup.Routing;

public static class RoutingSetup
{
    public static IServiceCollection AddRoutingConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddHttpContextAccessor();

        services.AddRouting(options =>
        {
            options.ConstraintMap.Add("string", typeof(string));
        });
        return services;
    }
}