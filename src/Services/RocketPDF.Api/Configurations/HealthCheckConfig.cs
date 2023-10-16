using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RocketPDF.Api.Configurations
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddNpgSql(configuration.GetConnectionString("DataConnection")!);

            services
                .AddHealthChecksUI(setupSettings: setup =>
                {
                    // setup.SetEvaluationTimeInSeconds(5); //Configures the UI to poll for healthchecks updates every 5 seconds
                    setup.AddHealthCheckEndpoint("Self", "http://localhost:5000/hc");
                })
                .AddInMemoryStorage();

            return services;
        }
    }
}