using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RocketPDF.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Register services by assemblies and pattern
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="lifetime"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterAssemblyServices(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Scoped, string pattern = "Service")
        {
            assemblies.SelectMany(a => a.GetTypes().Where(a => a.Name.EndsWith(pattern) && !a.IsAbstract && !a.IsInterface))
                .Select(a => new { AssignedType = a, ServiceTypes = a.GetInterfaces().ToList() }).ToList()
                .ForEach(typesToRegister => typesToRegister.ServiceTypes.ForEach(typeToRegister => services.Add(new ServiceDescriptor(typeToRegister, typesToRegister.AssignedType, lifetime))));
            return services;
        }
    }
}