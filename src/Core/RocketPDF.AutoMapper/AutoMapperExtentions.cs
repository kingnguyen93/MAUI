using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RocketPDF.AutoMapper
{
    public static class AutoMapperExtentions
    {
        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            return AddCustomAutoMapper(services, default, assemblies);
        }

        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, params Assembly[] assemblies)
        {
            services.AddAutoMapper(mc =>
            {
                mc.Internal().AllowAdditiveTypeMapCreation = true;
                mc.AllowNullCollections = true;
                mc.AddCollectionMappers();
                mc.FindAndAutoMapTypes(assemblies);
                mc.ApplyMappingsFromAssembly(assemblies);
                mc.AddIgnoreMapAttribute();
                configAction?.Invoke(mc);
            },
            assemblies,
            ServiceLifetime.Singleton);

            return services;
        }

        private static void FindAndAutoMapTypes(this IMapperConfigurationExpression configuration, params Assembly[] assemblies)
        {
            var types = GetAllTypes(assemblies).Where(type =>
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                       typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                       typeInfo.IsDefined(typeof(AutoMapToAttribute));
            });

            foreach (var type in types)
            {
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private static List<Type> GetAllTypes(params Assembly[] assemblies)
        {
            Type[] allTypes;

            try
            {
                allTypes = assemblies.SelectMany(t => t.GetTypes()).ToArray();
            }
            catch (ReflectionTypeLoadException ex)
            {
                allTypes = ex.Types;
            }

            return allTypes.Where(type => type != null).ToList();
        }

        private static void ApplyMappingsFromAssembly(this IMapperConfigurationExpression configuration, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(t => t.GetExportedTypes())
                .Where(t => Array.Exists(t.GetInterfaces(), i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAutoMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { configuration });
            }
        }

        public static void ForAllMaps(this IMapperConfigurationExpression configurationProvider, Action<TypeMap, IMappingExpression> configuration) => configurationProvider.Internal().ForAllMaps(configuration);

        public static void ForAllPropertyMaps(this IMapperConfigurationExpression configurationProvider, Func<PropertyMap, bool> condition, Action<PropertyMap, IMemberConfigurationExpression> memberOptions) => configurationProvider.Internal().ForAllPropertyMaps(condition, memberOptions);

        public static void AddIgnoreMapAttribute(this IMapperConfigurationExpression configuration)
        {
            //configuration.ForAllMaps((_, mapExpression) => mapExpression.ForAllMembers(memberOptions =>
            //{
            //    if (memberOptions.DestinationMember.Has<IgnoreMapAttribute>())
            //    {
            //        memberOptions.Ignore();
            //    }
            //}));
            configuration.ForAllPropertyMaps(propertyMap => propertyMap.DestinationMember.Has<IgnoreMapAttribute>(), (_, memberOptions) => memberOptions.Ignore());
            // configuration.ForAllPropertyMaps(propertyMap => propertyMap.SourceMember?.Has<IgnoreMapAttribute>() == true, (_, memberOptions) => memberOptions.Ignore());
        }
    }
}