using System.Text.Json;
using System.Text.Json.Serialization;

namespace RocketPDF.Core.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, ReferenceHandler = ReferenceHandler.IgnoreCycles };

        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, jsonSerializerOptions);
        }

        public static T Deserialize<T>(string content)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(content, jsonSerializerOptions);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}