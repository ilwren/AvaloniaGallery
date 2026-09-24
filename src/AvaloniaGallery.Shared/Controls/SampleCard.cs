using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
// SetTextAsync lives here as an extension method in Avalonia 12.
using Avalonia.Input.Platform;
using AvaloniaGallery.Localization;

namespace AvaloniaGallery.Controls;

/// <summary>
/// Container for one example: live preview on top, collapsible source underneath.
/// Mirrors the sample card used throughout the WinUI 3 Gallery.
/// </summary>
[TemplatePart("PART_CopyButton", typeof(Button))]
[TemplatePart("PART_CopyLabel", typeof(TextBlock))]
public sealed class SampleCard : ContentControl
{
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<SampleCard, string?>(nameof(Header));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<SampleCard, string?>(nameof(Description));

    public static readonly StyledProperty<string?> XamlProperty =
        AvaloniaProperty.Register<SampleCard, string?>(nameof(Xaml));

    public static readonly StyledProperty<string?> CSharpProperty =
        AvaloniaProperty.Register<SampleCard, string?>(nameof(CSharp));

    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string? Xaml
    {
        get => GetValue(XamlProperty);
        set => SetValue(XamlProperty, value);
    }

    public string? CSharp
    {
        get => GetValue(CSharpProperty);
        set => SetValue(CSharpProperty, value);
    }

    private Button? _copyButton;
    private TextBlock? _copyLabel;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_copyButton is not null)
            _copyButton.Click -= OnCopyClick;

        _copyButton = e.NameScope.Find<Button>("PART_CopyButton");
        _copyLabel = e.NameScope.Find<TextBlock>("PART_CopyLabel");

        if (_copyButton is not null)
            _copyButton.Click += OnCopyClick;
    }

    private async void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var text = Xaml;
        if (string.IsNullOrEmpty(text))
            return;

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
            return;

        try
        {
            await clipboard.SetTextAsync(text);
            await FlashCopiedAsync();
        }
        catch
        {
            // Clipboard access can fail on some Linux sessions; a failed copy should never
            // surface as a crash in a browsing UI.
        }
    }

    private async Task FlashCopiedAsync()
    {
        if (_copyLabel is null)
            return;

        // The label's Text is bound to the localization hub, so a plain assignment would be
        // overwritten the next time the language changes. Clearing the binding for the
        // duration of the flash and restoring it afterwards keeps both behaviours intact.
        _copyLabel.ClearValue(TextBlock.TextProperty);
        _copyLabel.Text = Loc.Get("Page.Copied");

        await Task.Delay(1200);

        // The card may have been recycled while we waited.
        if (_copyLabel is null)
            return;

        _copyLabel.Bind(TextBlock.TextProperty, new Binding
        {
            Source = LocSource.For("Page.Copy"),
            Path = nameof(LocSource.Value),
        });
    }
}
