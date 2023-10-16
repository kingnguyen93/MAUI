using Microsoft.Extensions.DependencyInjection;
using RocketPDF.AutoMapper;
using RocketPDF.Domain.Entities;

namespace RocketPDF.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection ResolveInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddCustomAutoMapper(mc =>
            {
                typeof(Entity).Assembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(Entity)) && !type.IsAbstract)
                    .ForEach(type => mc.CreateMap(type, type));
            },
            typeof(InfrastructureModule).Assembly);

            return services;
        }
    }
}