using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace RocketPDF.Api.Configurations
{
    public static class CompressionConfig
    {
        public static IServiceCollection ConfigureResponseCompression(this IServiceCollection services)
        {
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            return services;
        }
    }
}