using CommunityToolkit.Mvvm.ComponentModel;

namespace RocketPDF
{
    public partial class AppSettings
    {
        public static string BaseUrl => "http://10.0.2.2:5000";

        public static string DeviceId { get; set; }
    }
}