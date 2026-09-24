using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaGallery.Localization;

namespace AvaloniaGallery.Controls;

/// <summary>
/// Parses a XAML snippet at runtime and displays the result.
/// <para>
/// This is what keeps the gallery honest: the "Source" pane prints the same string that is
/// fed to <see cref="AvaloniaRuntimeXamlLoader"/>, so a sample can never claim to show code
/// that differs from the control rendered next to it.
/// </para>
/// <para>
/// A malformed snippet renders an inline error card instead of tearing down the app, which
/// keeps one bad sample from taking the whole gallery with it.
/// </para>
/// </summary>
public sealed class SampleHost : ContentControl
{
    public static readonly StyledProperty<string?> XamlProperty =
        AvaloniaProperty.Register<SampleHost, string?>(nameof(Xaml));

    /// <summary>Data context applied to the parsed control (not to the host itself).</summary>
    public static readonly StyledProperty<object?> SampleDataContextProperty =
        AvaloniaProperty.Register<SampleHost, object?>(nameof(SampleDataContext));

    public string? Xaml
    {
        get => GetValue(XamlProperty);
        set => SetValue(XamlProperty, value);
    }

    public object? SampleDataContext
    {
        get => GetValue(SampleDataContextProperty);
        set => SetValue(SampleDataContextProperty, value);
    }

    /// <summary>
    /// Optional hook run once the preview is attached to a window; see
    /// <see cref="Models.ControlSample.Attach"/>.
    /// </summary>
    public Func<Control, IDisposable?>? Attach { get; set; }

    /// <summary>
    /// Builds the preview in code instead of parsing <see cref="Xaml"/>; see
    /// <see cref="Models.ControlSample.LiveContentFactory"/>.
    /// </summary>
    public Func<Control>? LiveContentFactory
    {
        get => _liveContentFactory;
        set
        {
            _liveContentFactory = value;

            // Xaml is usually assigned first by the object initialiser, which already
            // triggered a Rebuild that tried to parse markup this sample never intended to
            // mount. Rebuilding here is what lets the factory win.
            Rebuild();
        }
    }

    private Func<Control>? _liveContentFactory;

    private IDisposable? _attachment;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == XamlProperty || change.Property == SampleDataContextProperty)
            Rebuild();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (Content is null)
            Rebuild();

        // Run the hook only once there is a TopLevel to reach: samples like NativeMenuBar
        // need the window, which does not exist while the tree is being built.
        //
        // The hook receives the host rather than the parsed control on purpose. The content
        // presenter has not mounted the parsed tree yet at this point, so
        // TopLevel.GetTopLevel on it would return null; the host itself is already attached.
        if (_attachment is null && Attach is not null)
            _attachment = Attach(this);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        _attachment?.Dispose();
        _attachment = null;
    }

    private void Rebuild()
    {
        if (LiveContentFactory is not null)
        {
            Content = LiveContentFactory();
            return;
        }

        var xaml = Xaml;
        if (string.IsNullOrWhiteSpace(xaml))
        {
            Content = null;
            return;
        }

        try
        {
            var parsed = AvaloniaRuntimeXamlLoader.Parse(xaml);

            if (parsed is StyledElement element && SampleDataContext is { } dc)
                element.DataContext = dc;

            Content = parsed;
        }
        catch (Exception ex)
        {
            Content = BuildErrorCard(ex);
        }
    }

    private static Control BuildErrorCard(Exception ex) => new Border
    {
        Background = new SolidColorBrush(Color.FromArgb(24, 0xE8, 0x11, 0x23)),
        BorderBrush = new SolidColorBrush(Color.FromArgb(120, 0xE8, 0x11, 0x23)),
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(6),
        Padding = new Thickness(14, 12),
        Child = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new TextBlock
                {
                    Text = Loc.Get("Sample.Failed"),
                    FontWeight = FontWeight.SemiBold,
                },
                new TextBlock
                {
                    Text = ex.Message,
                    TextWrapping = TextWrapping.Wrap,
                    Opacity = 0.85,
                    FontSize = 12,
                },
            },
        },
    };
}
