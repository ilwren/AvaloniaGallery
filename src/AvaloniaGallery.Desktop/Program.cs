using Avalonia;
using AvaloniaGallery;

namespace AvaloniaGallery.Desktop;

internal static class Program
{
    // Avalonia configuration must not be moved or renamed: the visual designer looks for it.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    [STAThread]
    public static int Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
}
