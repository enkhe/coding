using Microsoft.Extensions.Logging;

namespace HybridSample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Enables BlazorWebView and Razor components inside MAUI
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Native services usable from Razor components via @inject
        builder.Services.AddSingleton(IGeolocation.Default);
        builder.Services.AddSingleton(IFileSystem.Default);

        return builder.Build();
    }
}
