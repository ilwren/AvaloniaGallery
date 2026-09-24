using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Services;

namespace AvaloniaGallery.Samples;

/// <summary>
/// The interactive harness behind the TrayIcon sample.
/// <para>
/// A tray icon cannot be expressed as a preview control, because it is registered with the
/// <see cref="Avalonia.Application"/> and drawn by the OS rather than by Avalonia. This
/// builds the button and status line that let a reader switch a real one on and off, while
/// the sample's source pane keeps showing the declarative markup they would actually write.
/// </para>
/// </summary>
internal static class TrayIconDemo
{
    public static Control Create()
    {
        var available = PlatformCapabilities.HasTrayArea;

        var button = new Button
        {
            IsEnabled = available,
            MinWidth = 170,
        };

        var status = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Opacity = 0.85,
            MaxWidth = 460,
        };

        void Refresh()
        {
            button.Content = TrayIconService.IsActive
                ? Loc.Get("Tray.Disable")
                : Loc.Get("Tray.Enable");

            status.Text = !available
                ? Loc.Get("Tray.Unavailable")
                : TrayIconService.IsActive
                    ? Loc.Get("Tray.Active")
                    : Loc.Get("Tray.Inactive");
        }

        button.Click += (_, _) => TrayIconService.Toggle();

        // The service is global, so the page has to re-read it rather than track its own
        // flag: the icon may have been switched off from its own "Quit" menu item.
        void OnStateChanged() => Refresh();
        void OnLanguageChanged() => Refresh();

        TrayIconService.StateChanged += OnStateChanged;
        Loc.LanguageChanged += OnLanguageChanged;

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(button);
        panel.Children.Add(status);

        // Detaching the page must not leave the static events holding this tree alive.
        panel.DetachedFromVisualTree += (_, _) =>
        {
            TrayIconService.StateChanged -= OnStateChanged;
            Loc.LanguageChanged -= OnLanguageChanged;
        };

        Refresh();
        return panel;
    }
}
