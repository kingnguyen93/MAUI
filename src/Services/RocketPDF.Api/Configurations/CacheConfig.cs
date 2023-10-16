using RocketPDF.Infrastructure.Services;

namespace RocketPDF.Api.Configurations
{
    public static class CacheConfig
    {
        public static IServiceCollection ConfigureCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
            services.AddSingleton<ICacheService, MemoryCacheService>();

            return services;
        }
    }
}