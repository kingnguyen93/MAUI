using Refit;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace RocketPDF.Core.Services
{
    public static class RefitGenerator
    {
        public static T Create<T>()
        {
            var httpClient = new HttpClient(new AuthHeaderHandler() { InnerHandler = new HttpClientHandler() })
            {
                BaseAddress = new Uri(AppSettings.BaseUrl),
                Timeout = TimeSpan.FromSeconds(60)
            };
            return RestService.For<T>(httpClient, new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    Converters = { new JsonStringEnumConverter() }
                })
            });
        }
    }
}
