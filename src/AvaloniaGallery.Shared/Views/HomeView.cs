using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Models;
using AvaloniaGallery.Samples;

namespace AvaloniaGallery.Views;

/// <summary>
/// Landing page: a short hero, the headline counts, and a tile per category.
/// </summary>
public sealed class HomeView : UserControl
{
    public HomeView(Action<ControlPage> navigate)
    {
        var root = new StackPanel { Spacing = 24 };

        root.Children.Add(BuildHero());
        root.Children.Add(BuildStats());
        root.Children.Add(new TextBlock { Text = Loc.Get("Home.Browse"), Classes = { "sectionHeading" } });
        root.Children.Add(BuildTiles(navigate));

        Content = new ScrollViewer
        {
            Padding = new Thickness(40, 32, 40, 48),
            Content = root,
        };
    }

    private static Control BuildHero()
    {
        var stack = new StackPanel { Spacing = 10, MaxWidth = 780, HorizontalAlignment = HorizontalAlignment.Left };

        stack.Children.Add(new TextBlock
        {
            Text = Loc.Get("Home.Title"),
            FontSize = 34,
            FontWeight = FontWeight.SemiBold,
        });

        stack.Children.Add(new TextBlock
        {
            Text = Loc.Get("Home.Intro"),
            TextWrapping = TextWrapping.Wrap,
            Opacity = 0.8,
            FontSize = 14,
        });

        // Provenance belongs where people actually look, not only in the README.
        stack.Children.Add(new Border
        {
            Margin = new Thickness(0, 4, 0, 0),
            Padding = new Thickness(10, 7, 10, 7),
            CornerRadius = new CornerRadius(5),
            Background = new SolidColorBrush(Color.Parse("#14808080")),
            Child = new TextBlock
            {
                Text = Loc.Get("Home.AiNotice"),
                TextWrapping = TextWrapping.Wrap,
                Opacity = 0.75,
                FontSize = 12,
            },
        });

        return new Border
        {
            Background = Application.Current?.FindResource("GalleryHeroBrush") as IBrush,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(28, 24),
            Child = stack,
        };
    }

    private static Control BuildStats()
    {
        var row = new WrapPanel { ItemSpacing = 12, LineSpacing = 12 };

        row.Children.Add(Stat(SampleRegistry.TotalControls.ToString(), Loc.Get("Home.Controls")));
        row.Children.Add(Stat(SampleRegistry.TotalSamples.ToString(), Loc.Get("Home.Samples")));
        row.Children.Add(Stat(SampleRegistry.Categories.Count.ToString(), Loc.Get("Home.Categories")));
        row.Children.Add(Stat(SampleRegistry.NewInTwelve.ToString(), Loc.Get("Home.New")));

        return row;
    }

    private static Control Stat(string value, string label)
    {
        var stack = new StackPanel { Spacing = 2 };

        stack.Children.Add(new TextBlock
        {
            Text = value,
            FontSize = 26,
            FontWeight = FontWeight.SemiBold,
        });

        stack.Children.Add(new TextBlock
        {
            Text = label,
            FontSize = 12,
            Opacity = 0.7,
        });

        return new Border
        {
            Background = Application.Current?.FindResource("GalleryCardBackgroundBrush") as IBrush,
            BorderBrush = Application.Current?.FindResource("GalleryCardBorderBrush") as IBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20, 14),
            MinWidth = 170,
            Child = stack,
        };
    }

    private static Control BuildTiles(Action<ControlPage> navigate)
    {
        var grid = new WrapPanel { ItemSpacing = 14, LineSpacing = 14 };

        foreach (var category in SampleRegistry.Categories)
        {
            var content = new StackPanel { Spacing = 8, Width = 240 };

            var titleRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 9 };
            titleRow.Children.Add(new PathIcon
            {
                Data = Geometry.Parse(category.IconData),
                Width = 15,
                Height = 15,
                VerticalAlignment = VerticalAlignment.Center,
            });
            titleRow.Children.Add(new TextBlock
            {
                Text = Loc.Get(category.NameKey ?? category.Name),
                FontWeight = FontWeight.SemiBold,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
            });
            content.Children.Add(titleRow);

            content.Children.Add(new TextBlock
            {
                Text = string.Format(Loc.Get("Home.ControlsCount"), category.Pages.Count),
                FontSize = 12,
                Opacity = 0.65,
            });

            // A few example names give the tile some texture.
            content.Children.Add(new TextBlock
            {
                Text = string.Join(", ", category.Pages.Take(4).Select(p => p.Name))
                     + (category.Pages.Count > 4 ? "…" : string.Empty),
                FontSize = 11,
                Opacity = 0.5,
                TextWrapping = TextWrapping.Wrap,
            });

            var first = category.Pages.FirstOrDefault();
            var button = new Button
            {
                Classes = { "tile" },
                Width = 276,
                Content = content,
            };

            if (first is not null)
                button.Click += (_, _) => navigate(first);

            grid.Children.Add(button);
        }

        return grid;
    }
}
