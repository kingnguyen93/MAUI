using Microsoft.Extensions.DependencyInjection;
using RocketPDF.Application.Services;
using RocketPDF.EntityFrameworkCore;
using RocketPDF.EntityFrameworkCore.Repositories;
using RocketPDF.Infrastructure.Mapper;
using RocketPDF.Infrastructure.Repositories;
using RocketPDF.Infrastructure.Services;
using RocketPDF.Infrastructure.Threading;

namespace RocketPDF.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection ResolveApplicationDependencies(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<ILockService, LockService>();
            services.AddSingleton<IObjectMapper, AutoMapperObjectMapper>();

            services.AddScoped<IInjector, Injector>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICancellationTokenProvider, HttpContextCancellationTokenProvider>();

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<NpgsqlContext>());
            services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
            services.AddScoped(typeof(IGenericService<,,>), typeof(GenericService<,,>));
            services.AddScoped(typeof(IGenericService<,,,>), typeof(GenericService<,,,>));
            services.AddScoped(typeof(IGenericService<,,,,>), typeof(GenericService<,,,,>));
            services.AddScoped(typeof(IGenericService<,,,,,>), typeof(GenericService<,,,,,>));
            services.AddScoped(typeof(IMultiKeyGenericService<>), typeof(MultiKeyGenericService<>));
            services.AddScoped(typeof(IMultiKeyGenericService<,>), typeof(MultiKeyGenericService<,>));
            services.AddScoped(typeof(IMultiKeyGenericService<,,>), typeof(MultiKeyGenericService<,,>));
            services.AddScoped(typeof(IMultiKeyGenericService<,,,>), typeof(MultiKeyGenericService<,,,>));
            services.AddScoped(typeof(IMultiKeyGenericService<,,,,>), typeof(MultiKeyGenericService<,,,,>));
            services.AddScoped(typeof(IMultiKeyGenericService<,,,,,>), typeof(MultiKeyGenericService<,,,,,>));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped(typeof(IQueryableRepository<>), typeof(QueryableRepository<>));
            services.AddScoped(typeof(IQueryableRepository<,>), typeof(QueryableRepository<,>));
            services.AddScoped(typeof(IMapperRepository<>), typeof(MapperRepository<>));
            services.AddScoped(typeof(IMapperRepository<,>), typeof(MapperRepository<,>));

            services.RegisterAssemblyServices(new[] { typeof(ApplicationModule).Assembly }, pattern: "Service");
            services.RegisterAssemblyServices(new[] { typeof(EntityFrameworkCoreModule).Assembly }, pattern: "Repository");

            return services;
        }
    }
}