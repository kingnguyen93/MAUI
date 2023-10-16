using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;
using System.Reflection;

namespace RocketPDF;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureEssentials(essentials =>
            {
                essentials.UseVersionTracking();
            })
            .RegisterApis()
            .RegisterServices()
            .RegisterPages();

        // Register Syncfusion license
        SyncfusionLicenseProvider.RegisterLicense("YOUR LICENSE KEY");

        // Localization
        builder.Services.AddLocalization();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterApis(this MauiAppBuilder builder)
    {
        builder.Services.RegisterApis(new[] { Assembly.Load("RocketPDF.Core") });

        return builder;
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        return builder;
    }

    private static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
    {
        builder.Services.RegisterPageViewModels(new[] { Assembly.GetExecutingAssembly() });

        return builder;
    }
}