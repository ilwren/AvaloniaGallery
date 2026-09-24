using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using AvaloniaGallery.Controls;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Models;
using AvaloniaGallery.Services;

namespace AvaloniaGallery.Views;

/// <summary>
/// The detail pane: title, metadata chips, then one <see cref="SampleCard"/> per example.
/// <para>
/// Built in code rather than XAML because the shape is uniform across every control and
/// driven entirely by the registry — a template would just be a less direct way to say this.
/// </para>
/// </summary>
public sealed class ControlPageView : UserControl
{
    public ControlPageView(ControlPage page)
    {
        var header = new StackPanel { Spacing = 10 };

        // Title + "new in 12" pill
        var titleRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
        };
        titleRow.Children.Add(new TextBlock
        {
            Text = page.Name,
            Classes = { "pageTitle" },
            VerticalAlignment = VerticalAlignment.Center,
        });

        if (page.IsNew)
            titleRow.Children.Add(Badge(Loc.Get("Page.NewBadge")));

        if (page.IsInfrastructure)
            titleRow.Children.Add(Badge("Base class"));

        if (page.Requires != PlatformRequirement.Any)
            titleRow.Children.Add(Badge(Loc.Get("Platform.DesktopOnly")));

        header.Children.Add(titleRow);

        header.Children.Add(new TextBlock
        {
            Text = page.Summary,
            Classes = { "pageSummary" },
            MaxWidth = 760,
            HorizontalAlignment = HorizontalAlignment.Left,
        });

        // Namespace / assembly / docs / page source
        var meta = new WrapPanel { ItemSpacing = 8, LineSpacing = 8 };
        meta.Children.Add(Chip(page.Namespace));

        // For most controls the namespace and the assembly are both "Avalonia.Controls";
        // showing that twice is just noise, so only add it when it differs.
        if (!string.Equals(page.Assembly, page.Namespace, StringComparison.Ordinal))
            meta.Children.Add(Chip(page.Assembly));

        if (!string.IsNullOrEmpty(page.DocsUrl))
        {
            meta.Children.Add(new HyperlinkButton
            {
                Content = Loc.Get("Page.Docs"),
                NavigateUri = new Uri(page.DocsUrl),
                Padding = new Thickness(0),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
            });
        }

        header.Children.Add(meta);

        var root = new StackPanel { Spacing = 0 };
        root.Children.Add(header);

        // "Page source": the gallery's own C# for this page, mirroring the link WinUI 3
        // Gallery puts on each of its pages.
        if (BuildPageSourceSection(page) is { } pageSource)
            root.Children.Add(pageSource);

        // Samples
        var body = new StackPanel { Spacing = 0, Margin = new Thickness(0, 24, 0, 0) };
        var supported = PlatformCapabilities.IsSupported(page.Requires);

        foreach (var sample in page.Samples)
            body.Children.Add(BuildCard(sample, supported));

        root.Children.Add(body);

        Content = new ScrollViewer
        {
            Padding = new Thickness(40, 32, 40, 48),
            Content = root,
        };
    }

    /// <summary>
    /// Collapsible viewer for the gallery's own implementation of this page, read from the
    /// sources embedded in the assembly.
    /// </summary>
    private static Control? BuildPageSourceSection(ControlPage page)
    {
        if (string.IsNullOrEmpty(page.SourceFile))
            return null;

        var source = PageSourceProvider.GetPageSource(page.Name, page.SourceFile);
        if (string.IsNullOrEmpty(source))
            return null;

        var toggle = new ToggleButton
        {
            Padding = new Thickness(10, 4),
            FontSize = 12,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 12, 0, 0),
        };
        ToolTip.SetTip(toggle, Loc.Get("Page.PageSourceTip"));

        var toggleContent = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
        toggleContent.Children.Add(new PathIcon
        {
            Data = Geometry.Parse("M5.5 4.2 1.7 8l3.8 3.8-1.1 1.1L-.5 8l4.9-4.9zm5 0L14.3 8l-3.8 3.8 1.1 1.1L16.5 8l-4.9-4.9z"),
            Width = 13,
            Height = 13,
        });
        toggleContent.Children.Add(new TextBlock
        {
            Text = $"{Loc.Get("Page.PageSource")} — {page.SourceFile}",
            VerticalAlignment = VerticalAlignment.Center,
        });
        toggle.Content = toggleContent;

        var pane = new Border
        {
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Margin = new Thickness(0, 6, 0, 0),
            Child = new ScrollViewer
            {
                HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                MaxHeight = 460,
                Padding = new Thickness(16, 12),
                Content = new XamlPresenter
                {
                    Source = source,
                    Language = "csharp",
                    FontSize = 12.5,
                    TextWrapping = TextWrapping.NoWrap,
                },
            },
        };
        pane.Bind(Border.BackgroundProperty, new DynamicResourceExtension("GalleryCodeBackgroundBrush"));
        pane.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("GalleryCardBorderBrush"));
        pane.Bind(IsVisibleProperty, new Avalonia.Data.Binding
        {
            Source = toggle,
            Path = nameof(ToggleButton.IsChecked),
        });

        var section = new StackPanel { Spacing = 0 };
        section.Children.Add(toggle);
        section.Children.Add(pane);
        return section;
    }

    private static Control BuildCard(ControlSample sample, bool platformSupported)
    {
        Control preview;

        if (!platformSupported)
        {
            // The sample would not work here, so show why instead of a dead control. The
            // source panes below still carry the code, which is the useful part on a
            // platform that cannot run it.
            preview = UnsupportedCard();
        }
        else
        {
            var host = new SampleHost
            {
                Xaml = sample.Xaml,
                SampleDataContext = sample.DataContextFactory?.Invoke(),
                Attach = sample.Attach,
                LiveContentFactory = sample.LiveContentFactory,
            };

            preview = host;

            // Samples that need room (calendars, tables) declare a height so the card does
            // not collapse around a control that sizes to its content lazily.
            if (sample.PreviewHeight is { } h)
            {
                preview = new Border
                {
                    Height = h,
                    Child = host,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
            }
        }

        return new SampleCard
        {
            Header = sample.Title,
            Description = sample.Description,
            Xaml = sample.Xaml,
            CSharp = sample.CSharpSource,
            Content = preview,
        };
    }

    private static Control UnsupportedCard()
    {
        var text = new TextBlock
        {
            Text = string.Format(
                Loc.Get("Platform.NotSupported"),
                PlatformCapabilities.CurrentPlatformName),
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 520,
            Opacity = 0.85,
        };

        var border = new Border
        {
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(14, 12),
            Child = text,
        };

        border.Bind(Border.BackgroundProperty, new DynamicResourceExtension("GalleryCheckerBrush"));
        border.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("GalleryCardBorderBrush"));
        return border;
    }

    private static Border Badge(string text) => new()
    {
        Classes = { "badge" },
        Child = new TextBlock { Text = text },
    };

    private static Border Chip(string text) => new()
    {
        Classes = { "metaChip" },
        Child = new TextBlock { Text = text },
    };
}
