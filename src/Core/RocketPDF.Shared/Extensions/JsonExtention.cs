using System.Text.Json;

namespace RocketPDF.Shared.Extensions
{
    public static class JsonExtention
    {
        public static T? ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

        public static T? ToObject<T>(this JsonDocument document)
        {
            var json = document.RootElement.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}