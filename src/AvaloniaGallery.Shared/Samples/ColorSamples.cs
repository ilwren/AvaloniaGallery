using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>
/// The Avalonia.Controls.ColorPicker package. Shipped separately from the main
/// Avalonia package, so its theme has to be merged in explicitly.
/// </summary>
internal static class ColorSamples
{
    private const string Assembly = "Avalonia.Controls.ColorPicker";

    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Color",
        IconData = Icons.Color,
        NameKey = "Cat.Color",
        SourceFile = "ColorSamples.cs",
    }
    .With(ColorPickerPage(), ColorViewPage(), ColorSpectrumPage(), ColorSliderPage(), ColorPreviewerPage());

    private static ControlPage ColorPickerPage() => new ControlPage
    {
        Name = "ColorPicker",
        Summary = "A dropdown button that opens the full colour editing surface.",
        Assembly = Assembly,
        Samples =
        {
            new ControlSample
            {
                Title = "Picking a colour",
                Description = "Click the swatch to open the spectrum, sliders and palette.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="16">
                  <ColorPicker Color="#3B78FF" />
                  <ColorPicker Color="#107C10" IsAlphaEnabled="True" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ColorViewPage() => new ControlPage
    {
        Name = "ColorView",
        Summary = "The full colour editor as an inline control rather than a dropdown.",
        Assembly = Assembly,
        Samples =
        {
            new ControlSample
            {
                Title = "Inline editor",
                PreviewHeight = 420,
                Xaml = """
                <ColorView xmlns="https://github.com/avaloniaui"
                           Color="#C239B3"
                           IsAlphaEnabled="True"
                           Width="340" Height="380" />
                """,
            },
        },
    };

    private static ControlPage ColorSpectrumPage() => new ControlPage
    {
        Name = "ColorSpectrum",
        Summary = "The two dimensional hue and saturation surface on its own.",
        Assembly = Assembly,
        Samples =
        {
            new ControlSample
            {
                Title = "Box and ring shapes",
                PreviewHeight = 260,
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="20">
                  <ColorSpectrum Color="#3B78FF" Shape="Box" Width="200" Height="200" />
                  <ColorSpectrum Color="#CA5010" Shape="Ring" Width="200" Height="200" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ColorSliderPage() => new ControlPage
    {
        Name = "ColorSlider",
        Summary = "A single channel slider: hue, saturation, value or alpha.",
        Assembly = Assembly,
        Samples =
        {
            new ControlSample
            {
                Title = "Individual channels",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="14" Width="320">
                  <ColorSlider Color="#3B78FF" ColorComponent="Component1" Height="24" />
                  <ColorSlider Color="#3B78FF" ColorComponent="Component2" Height="24" />
                  <ColorSlider Color="#3B78FF" ColorComponent="Component3" Height="24" />
                  <ColorSlider Color="#3B78FF" ColorComponent="Alpha" Height="24" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ColorPreviewerPage() => new ControlPage
    {
        Name = "ColorPreviewer",
        Summary = "Shows the current colour beside lighter and darker variants.",
        Assembly = Assembly,
        Samples =
        {
            new ControlSample
            {
                Title = "Accent variants",
                Xaml = """
                <ColorPreviewer xmlns="https://github.com/avaloniaui"
                                HsvColor="hsv(210, 0.77, 1.0)"
                                IsAccentColorsVisible="True"
                                Width="320" />
                """,
            },
        },
    };
}
