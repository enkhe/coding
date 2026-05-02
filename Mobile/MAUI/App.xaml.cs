using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MyApp.ViewModels;
using MyApp.Views;

namespace MyApp;

// MAUI 10 entry point. `MauiProgram` builds the app host (services, fonts, handlers),
// then `App` configures the visual root via Shell.
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // DI registrations (same IServiceCollection you know from ASP.NET Core)
        builder.Services.AddSingleton<IGreetingService, GreetingService>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}

public partial class App : Application
{
    public App() => InitializeComponent();

    protected override Window CreateWindow(IActivationState? activationState)
        => new(new AppShell());
}

internal sealed class GreetingService : IGreetingService
{
    public Task<string> GetGreetingAsync(string name, CancellationToken ct = default)
        => Task.FromResult(string.IsNullOrWhiteSpace(name)
            ? "Hello, world!"
            : $"Hello, {name}!");
}
