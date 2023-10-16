using Hangfire;
using Hangfire.PostgreSql;

namespace RocketPDF.Api.Configurations
{
    public static class HangfireConfig
    {
        public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .UseSerilogLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(configuration.GetConnectionString("DataConnection")));

            services.AddHangfireServer(config =>
            {
                config.SchedulePollingInterval = TimeSpan.FromSeconds(10);
                config.WorkerCount = 16;
            });

            return services;
        }
    }
}