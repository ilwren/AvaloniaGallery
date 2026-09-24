using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Services;
using AvaloniaGallery.Views;

namespace AvaloniaGallery.Samples;

/// <summary>
/// Interactive harness for the desktop-only window materials: Mica, Acrylic and blur-behind.
/// <para>
/// These cannot be shown as a parsed XAML snippet, because the effect belongs to the
/// <see cref="Window"/> that hosts the gallery rather than to any control inside the sample.
/// Applying it for real is also the only honest demo: the hint is a *preference list*, and
/// what you get depends on the OS, the compositor and the theme. The harness therefore
/// applies the level and then reports <see cref="TopLevel.ActualTransparencyLevel"/>, which
/// is what the platform actually granted.
/// </para>
/// </summary>
internal static class WindowEffectsDemo
{
    /// <summary>The materials offered, in the order a reader should try them.</summary>
    private static readonly (string Label, WindowTransparencyLevel Level, string Note)[] Levels =
    {
        ("None", WindowTransparencyLevel.None,
            "Opaque. The window background brush is painted as-is."),
        ("Transparent", WindowTransparencyLevel.Transparent,
            "The window is see-through wherever nothing is drawn."),
        ("Blur", WindowTransparencyLevel.Blur,
            "Blur-behind: whatever sits under the window is blurred."),
        ("AcrylicBlur", WindowTransparencyLevel.AcrylicBlur,
            "Acrylic: a stronger blur tinted by the material brush. Falls back to Blur."),
        ("Mica", WindowTransparencyLevel.Mica,
            "Mica: the desktop wallpaper tints the window. Windows 11; falls back elsewhere."),
    };

    public static Control Create()
    {
        // Off the desktop there is no window to apply a material to.
        if (!PlatformCapabilities.IsDesktop)
            return UnsupportedCard();

        var status = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Opacity = 0.85,
            Margin = new Thickness(0, 4, 0, 0),
        };

        var note = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Opacity = 0.7,
            FontSize = 12,
        };

        var buttons = new WrapPanel { Orientation = Orientation.Horizontal, ItemSpacing = 8, LineSpacing = 8 };

        var root = new StackPanel { Spacing = 10 };
        root.Children.Add(buttons);
        root.Children.Add(status);
        root.Children.Add(note);

        // The acrylic tint only has meaning while an acrylic-ish level is active, so it is
        // built once and shown alongside the readout.
        var preview = new Border
        {
            Height = 64,
            CornerRadius = new CornerRadius(6),
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(Color.Parse("#40808080")),
            Child = new TextBlock
            {
                Text = "The window behind this card is what changes.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                Opacity = 0.75,
            },
        };
        root.Children.Add(preview);

        void Apply(WindowTransparencyLevel level, string label, string description)
        {
            // The shell owns its own chrome, so it is what knows how to get out of the way.
            var shell = root.FindAncestorOfType<MainView>();
            if (shell is null)
            {
                status.Text = "This sample needs the gallery shell to host it.";
                return;
            }

            var granted = shell.ApplyWindowMaterial(Fallbacks(level));

            note.Text = description;
            Report(label, granted, status);
        }

        foreach (var (label, level, description) in Levels)
        {
            var button = new Button { Content = label, MinWidth = 104 };
            button.Click += (_, _) => Apply(level, label, description);
            buttons.Children.Add(button);
        }

        // Show the starting state without changing anything.
        status.Text = "Pick a material above.";

        // ActualTransparencyLevel settles after the platform has answered, so keep the
        // readout bound to it rather than sampling once and hoping.
        root.AttachedToVisualTree += (_, _) =>
        {
            if (TopLevel.GetTopLevel(root) is not Window window)
                return;

            window.PropertyChanged += (_, e) =>
            {
                if (e.Property == TopLevel.ActualTransparencyLevelProperty)
                    status.Text = $"Active material: {e.NewValue}.  ({PlatformCapabilities.CurrentPlatformName})";
            };
        };

        return root;
    }

    /// <summary>
    /// Builds the preference list. Asking for Mica alone on a machine that cannot do Mica
    /// yields plain opaque; listing the softer materials after it degrades gracefully.
    /// </summary>
    private static IReadOnlyList<WindowTransparencyLevel> Fallbacks(WindowTransparencyLevel level)
    {
        if (level == WindowTransparencyLevel.None)
            return new[] { WindowTransparencyLevel.None };

        var chain = new List<WindowTransparencyLevel> { level };

        if (level == WindowTransparencyLevel.Mica)
            chain.Add(WindowTransparencyLevel.AcrylicBlur);

        if (level != WindowTransparencyLevel.Blur)
            chain.Add(WindowTransparencyLevel.Blur);

        chain.Add(WindowTransparencyLevel.None);
        return chain;
    }

    /// <summary>
    /// Reports what the platform granted. Requested and actual routinely differ, and saying
    /// so is more useful than letting the reader guess why nothing looks different.
    /// </summary>
    private static void Report(string requested, WindowTransparencyLevel actual, TextBlock status)
    {
        // WindowTransparencyLevel is a record struct whose value is only exposed via ToString.
        var actualName = actual.ToString();

        var granted = string.Equals(actualName, requested, StringComparison.OrdinalIgnoreCase)
            ? $"Requested {requested} — granted."
            : $"Requested {requested} — the platform gave {actualName}.";

        status.Text = $"{granted}  ({PlatformCapabilities.CurrentPlatformName})";
    }

    private static Control UnsupportedCard() => new Border
    {
        Padding = new Thickness(14),
        CornerRadius = new CornerRadius(6),
        Background = new SolidColorBrush(Color.Parse("#20808080")),
        Child = new TextBlock
        {
            Text = Loc.Get("Platform.DesktopOnly"),
            TextWrapping = TextWrapping.Wrap,
            Opacity = 0.85,
        },
    };
}
