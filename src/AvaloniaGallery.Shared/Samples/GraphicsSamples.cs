using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>Shapes, images and icons.</summary>
internal static class GraphicsSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Graphics and media",
        IconData = Icons.Shapes,
        NameKey = "Cat.Graphics",
        SourceFile = "GraphicsSamples.cs",
    }
    .With(ShapesPage(), ArcSectorPage(), ImagePage(), PathIconPage(), BrushesPage(),
          ExperimentalAcrylicBorderPage());

    private static ControlPage ShapesPage() => new ControlPage
    {
        Name = "Shapes",
        Summary = "Rectangle, Ellipse, Line, Path, Polygon and Polyline draw vector geometry.",
        Samples =
        {
            new ControlSample
            {
                Title = "The basic shapes",
                Xaml = """
                <WrapPanel xmlns="https://github.com/avaloniaui" Width="460">
                  <Rectangle Width="80" Height="60" Fill="#3B78FF" RadiusX="8" RadiusY="8" Margin="6" />
                  <Ellipse Width="80" Height="60" Fill="#107C10" Margin="6" />
                  <Line StartPoint="0,0" EndPoint="80,60" Stroke="#C239B3" StrokeThickness="3" Margin="6" />
                  <Polygon Points="40,0 80,60 0,60" Fill="#CA5010" Margin="6" />
                  <Polyline Points="0,50 20,10 40,40 60,0 80,30"
                            Stroke="#0099BC" StrokeThickness="3" Margin="6" />
                  <Path Data="M 0,30 C 20,-10 60,70 80,30" Stroke="#8764B8" StrokeThickness="3" Margin="6" />
                </WrapPanel>
                """,
            },
            new ControlSample
            {
                Title = "Strokes and dashes",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="16">
                  <Rectangle Width="90" Height="60" Stroke="#3B78FF" StrokeThickness="2" />
                  <Rectangle Width="90" Height="60" Stroke="#3B78FF" StrokeThickness="2"
                             StrokeDashArray="4,2" />
                  <Ellipse Width="90" Height="60" Stroke="#107C10" StrokeThickness="3"
                           StrokeDashArray="1,2" StrokeLineCap="Round" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ArcSectorPage() => new ControlPage
    {
        Name = "Arc and Sector",
        Summary = "Arc draws a portion of an ellipse outline; Sector fills a pie slice.",
        Samples =
        {
            new ControlSample
            {
                Title = "Angles and sweeps",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="20">
                  <Arc Width="90" Height="90" StartAngle="0" SweepAngle="270"
                       Stroke="#3B78FF" StrokeThickness="6" />
                  <Sector Width="90" Height="90" StartAngle="0" SweepAngle="120" Fill="#107C10" />
                  <Sector Width="90" Height="90" StartAngle="30" SweepAngle="240" Fill="#C239B3" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ImagePage() => new ControlPage
    {
        Name = "Image",
        Summary = "Displays a bitmap or vector drawing, with control over how it stretches.",
        Samples =
        {
            new ControlSample
            {
                Title = "Stretch modes",
                Description = "A DrawingImage stands in for a bitmap so the sample needs no asset file.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui"
                            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            Orientation="Horizontal" Spacing="16">
                  <StackPanel.Resources>
                    <DrawingImage x:Key="Swatch">
                      <GeometryDrawing Brush="#3B78FF"
                                       Geometry="M0,0 L100,0 L100,60 L0,60 Z" />
                    </DrawingImage>
                  </StackPanel.Resources>
                  <Border BorderBrush="#40808080" BorderThickness="1" Width="110" Height="90">
                    <Image Source="{StaticResource Swatch}" Stretch="Uniform" />
                  </Border>
                  <Border BorderBrush="#40808080" BorderThickness="1" Width="110" Height="90">
                    <Image Source="{StaticResource Swatch}" Stretch="Fill" />
                  </Border>
                  <Border BorderBrush="#40808080" BorderThickness="1" Width="110" Height="90">
                    <Image Source="{StaticResource Swatch}" Stretch="None" />
                  </Border>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage PathIconPage() => new ControlPage
    {
        Name = "PathIcon",
        Summary = "Renders a path geometry as a glyph that follows the current foreground brush.",
        Samples =
        {
            new ControlSample
            {
                Title = "Icons at several sizes",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal"
                            Spacing="20" VerticalAlignment="Center">
                  <PathIcon Width="16" Height="16"
                            Data="M8 1l2.2 4.5L15 6.2l-3.5 3.4.8 4.8L8 12.1 3.7 14.4l.8-4.8L1 6.2l4.8-.7z" />
                  <PathIcon Width="24" Height="24" Foreground="#3B78FF"
                            Data="M8 1l2.2 4.5L15 6.2l-3.5 3.4.8 4.8L8 12.1 3.7 14.4l.8-4.8L1 6.2l4.8-.7z" />
                  <PathIcon Width="36" Height="36" Foreground="#C239B3"
                            Data="M8 1l2.2 4.5L15 6.2l-3.5 3.4.8 4.8L8 12.1 3.7 14.4l.8-4.8L1 6.2l4.8-.7z" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage BrushesPage() => new ControlPage
    {
        Name = "Brushes",
        Summary = "Solid, linear, radial and conic gradients used as fills.",
        Samples =
        {
            new ControlSample
            {
                Title = "Gradient fills",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="16">
                  <Border Width="110" Height="90" CornerRadius="6">
                    <Border.Background>
                      <LinearGradientBrush StartPoint="0%,0%" EndPoint="100%,100%">
                        <GradientStop Color="#3B78FF" Offset="0" />
                        <GradientStop Color="#C239B3" Offset="1" />
                      </LinearGradientBrush>
                    </Border.Background>
                  </Border>
                  <Border Width="110" Height="90" CornerRadius="6">
                    <Border.Background>
                      <RadialGradientBrush>
                        <GradientStop Color="#FFD700" Offset="0" />
                        <GradientStop Color="#CA5010" Offset="1" />
                      </RadialGradientBrush>
                    </Border.Background>
                  </Border>
                  <Border Width="110" Height="90" CornerRadius="6">
                    <Border.Background>
                      <ConicGradientBrush>
                        <GradientStop Color="#3B78FF" Offset="0" />
                        <GradientStop Color="#107C10" Offset="0.33" />
                        <GradientStop Color="#C239B3" Offset="0.66" />
                        <GradientStop Color="#3B78FF" Offset="1" />
                      </ConicGradientBrush>
                    </Border.Background>
                  </Border>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ExperimentalAcrylicBorderPage() => new ControlPage
    {
        Name = "ExperimentalAcrylicBorder",
        Summary = "A translucent, blurred material. Support varies by platform.",
        Samples =
        {
            new ControlSample
            {
                Title = "Acrylic material",
                Xaml = """
                <Panel xmlns="https://github.com/avaloniaui" Width="380" Height="150">
                  <Border CornerRadius="8">
                    <Border.Background>
                      <LinearGradientBrush StartPoint="0%,0%" EndPoint="100%,100%">
                        <GradientStop Color="#3B78FF" Offset="0" />
                        <GradientStop Color="#C239B3" Offset="1" />
                      </LinearGradientBrush>
                    </Border.Background>
                  </Border>
                  <ExperimentalAcrylicBorder CornerRadius="8" Margin="30">
                    <ExperimentalAcrylicBorder.Material>
                      <ExperimentalAcrylicMaterial
                          BackgroundSource="Digger"
                          TintColor="Black"
                          TintOpacity="0.5"
                          MaterialOpacity="0.4" />
                    </ExperimentalAcrylicBorder.Material>
                    <TextBlock Text="Acrylic over a gradient"
                               Foreground="White"
                               HorizontalAlignment="Center"
                               VerticalAlignment="Center" />
                  </ExperimentalAcrylicBorder>
                </Panel>
                """,
            },
        },
    };
}
