using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Library.Infrastructure.Caching
{
    public static class RedisCacheSetup
    {
        public static IServiceCollection AddRedisCache(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                var redisConfig = configuration.GetSection("Redis");
                options.Configuration = redisConfig["Configuration"];
                options.InstanceName = redisConfig["InstanceName"];
            });

            return services;
        }
    }
}
