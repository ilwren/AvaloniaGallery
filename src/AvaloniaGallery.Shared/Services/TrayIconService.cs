using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaGallery.Localization;

namespace AvaloniaGallery.Services;

/// <summary>
/// Registers a real, working tray icon so the TrayIcon page can be more than a code listing.
/// <para>
/// The icon carries a live native menu (show / hide / quit) and is created entirely in code
/// because the gallery draws its own bitmap rather than shipping an .ico — that keeps the
/// sample honest about what <see cref="TrayIcon"/> needs (a <see cref="WindowIcon"/>) while
/// avoiding a binary asset in a source-only deliverable.
/// </para>
/// </summary>
public static class TrayIconService
{
    private static TrayIcons? _icons;

    /// <summary>True when a tray icon is currently registered with the OS.</summary>
    public static bool IsActive => _icons is { Count: > 0 };

    /// <summary>Raised after <see cref="Toggle"/> so the page can re-letter its button.</summary>
    public static event Action? StateChanged;

    /// <summary>Adds or removes the tray icon. No-op when the platform has no tray area.</summary>
    public static void Toggle()
    {
        if (!PlatformCapabilities.HasTrayArea)
            return;

        if (IsActive)
            Remove();
        else
            Install();

        StateChanged?.Invoke();
    }

    /// <summary>Removes the icon on shutdown so it does not linger in the notification area.</summary>
    public static void Remove()
    {
        if (Application.Current is { } app)
            TrayIcon.SetIcons(app, new TrayIcons());

        _icons?.Clear();
        _icons = null;
    }

    private static void Install()
    {
        if (Application.Current is not { } app)
            return;

        var icon = new TrayIcon
        {
            Icon = BuildIcon(),
            ToolTipText = Loc.Get("Tray.ToolTip"),
            IsVisible = true,
            Menu = BuildMenu(),
        };

        // Left clicking the icon brings the window back, which is what users expect and
        // what the menu's "Show" item does.
        icon.Clicked += (_, _) => ShowMainWindow();

        _icons = new TrayIcons { icon };
        TrayIcon.SetIcons(app, _icons);
    }

    private static NativeMenu BuildMenu()
    {
        var show = new NativeMenuItem(Loc.Get("Tray.MenuShow"));
        show.Click += (_, _) => ShowMainWindow();

        var hide = new NativeMenuItem(Loc.Get("Tray.MenuHide"));
        hide.Click += (_, _) =>
        {
            if (MainWindow is { } w)
                w.Hide();
        };

        var quit = new NativeMenuItem(Loc.Get("Tray.MenuQuit"));
        quit.Click += (_, _) =>
        {
            Remove();
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        };

        return new NativeMenu { Items = { show, hide, new NativeMenuItemSeparator(), quit } };
    }

    private static Window? MainWindow =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

    private static void ShowMainWindow()
    {
        if (MainWindow is not { } window)
            return;

        window.Show();
        window.WindowState = WindowState.Normal;
        window.Activate();
    }

    /// <summary>Loads the Avalonia logo shipped in Assets for use as the tray image.</summary>
    private static WindowIcon BuildIcon()
    {
        using var asset = AssetLoader.Open(new Uri("avares://AvaloniaGallery/Assets/avalonia-logo.png"));
        return new WindowIcon(asset);
    }
}
