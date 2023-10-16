namespace RocketPDF.Infrastructure.Common
{
    public static class Swagger
    {
        public static string EndPoint => $"{Version}/swagger.json";
        public static string ApiName => "RocketPDF API";
        public static string Version => "v1";
    }

    public static class HealthCheck
    {
        public static string EndPoint => "/hc";
        public static string Self => "/liveness";
        public static string UI => "/hc-ui";
    }

    public static class HubEndPoint
    {
        public static string Notification => "/hubs/notification";
    }
}