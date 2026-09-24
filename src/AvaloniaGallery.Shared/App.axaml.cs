using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Services;
using AvaloniaGallery.Views;

namespace AvaloniaGallery;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        // Start in the user's own language when we support it, rather than forcing English.
        Loc.UseSystemLanguage();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow();

                // A tray icon that outlives the process is a real problem on some Linux
                // notification areas, so make sure it is torn down explicitly.
                desktop.ShutdownRequested += (_, _) => TrayIconService.Remove();
                break;

            // Kept so the shared view can host a non-desktop head without changes; the
            // gallery itself ships only the desktop head.
            case ISingleViewApplicationLifetime singleView:
                singleView.MainView = new MainView();
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
