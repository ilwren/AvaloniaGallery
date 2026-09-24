using Avalonia;
using Avalonia.Headless;
using AvaloniaGallery;

[assembly: AvaloniaTestApplication(typeof(AvaloniaGallery.Tests.TestAppBuilder))]

namespace AvaloniaGallery.Tests;

/// <summary>
/// Boots the real <see cref="App"/> (themes and all) on the headless platform, with Skia
/// enabled so tests exercise genuine layout and rendering rather than a stub.
/// </summary>
public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
