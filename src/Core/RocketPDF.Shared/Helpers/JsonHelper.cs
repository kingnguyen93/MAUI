using System.Text.Json;
using System.Text.Json.Serialization;

namespace RocketPDF.Shared.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, ReferenceHandler = ReferenceHandler.IgnoreCycles };

        public static string Serialize(object value, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(value, options ?? serializerOptions);
        }

        public static ValueTask<T> DeserializeAsync<T>(Stream utf8Json, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            return JsonSerializer.DeserializeAsync<T>(utf8Json, options ?? serializerOptions, cancellationToken);
        }
    }
}