using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RocketPDF.EntityFrameworkCore
{
    public static class EntityFrameworkCoreModule
    {
        public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, string? connectionString)
        {
            services.AddScoped<NpgsqlScopedFactory>();
            services.AddScoped(sp => sp.GetRequiredService<NpgsqlScopedFactory>().CreateDbContext());

            // Add framework services.
            services
                .AddPooledDbContextFactory<NpgsqlContext>(options =>
                {
                    options
                        .UseNpgsql(connectionString,
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                            })
#if DEBUG
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
#endif
                        .UseSnakeCaseNamingConvention()
                        .ConfigureWarnings(x => x.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
                }, poolSize: 100);

            return services;
        }
    }
}