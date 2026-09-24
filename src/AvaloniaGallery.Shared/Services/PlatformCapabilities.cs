using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace AvaloniaGallery.Services;

/// <summary>
/// What a control needs from the host in order to actually run.
/// <para>
/// Several Avalonia controls are meaningless outside a desktop session — a
/// <see cref="TrayIcon"/> has nowhere to go in a browser tab, and
/// <see cref="NativeControlHost"/> needs a real window handle. Rather than letting those
/// samples throw or silently render nothing on mobile and WASM, each page declares its
/// requirement and the gallery substitutes an explanatory card.
/// </para>
/// </summary>
public enum PlatformRequirement
{
    /// <summary>Runs anywhere Avalonia runs.</summary>
    Any,

    /// <summary>Needs a classic desktop lifetime (Windows, macOS, Linux).</summary>
    Desktop,

    /// <summary>Needs a desktop session *and* an OS notification area.</summary>
    TrayArea,
}

/// <summary>Answers what the current host actually supports.</summary>
public static class PlatformCapabilities
{
    /// <summary>True when running under a classic desktop lifetime.</summary>
    public static bool IsDesktop =>
        Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime;

    /// <summary>
    /// True when the app can own a tray icon. macOS, Windows and most Linux desktops
    /// qualify; a browser or a phone does not.
    /// </summary>
    public static bool HasTrayArea => IsDesktop;

    /// <summary>Human readable host name, used in the "not supported here" card.</summary>
    public static string CurrentPlatformName => Application.Current?.ApplicationLifetime switch
    {
        IClassicDesktopStyleApplicationLifetime => "Desktop",
        ISingleViewApplicationLifetime => OperatingSystem.IsBrowser() ? "Browser" : "Single view",
        _ => "Unknown",
    };

    public static bool IsSupported(PlatformRequirement requirement) => requirement switch
    {
        PlatformRequirement.Any => true,
        PlatformRequirement.Desktop => IsDesktop,
        PlatformRequirement.TrayArea => HasTrayArea,
        _ => true,
    };
}
