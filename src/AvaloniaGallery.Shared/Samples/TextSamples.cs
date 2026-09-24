using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>Controls that display or capture text.</summary>
internal static class TextSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Text",
        IconData = Icons.Text,
        NameKey = "Cat.Text",
        SourceFile = "TextSamples.cs",
    }
    .With(TextBlockPage(), SelectableTextBlockPage(), TextBoxPage(), MaskedTextBoxPage(),
          AutoCompleteBoxPage(), NumericUpDownPage(), LabelPage());

    private static ControlPage TextBlockPage() => new ControlPage
    {
        Name = "TextBlock",
        Summary = "The lightweight control for displaying read-only text.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/textblock",
        Samples =
        {
            new ControlSample
            {
                Title = "Weight, size and style",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="6">
                  <TextBlock Text="Regular body text" />
                  <TextBlock Text="Semi bold" FontWeight="SemiBold" />
                  <TextBlock Text="Large and light" FontSize="22" FontWeight="Light" />
                  <TextBlock Text="Italic and dimmed" FontStyle="Italic" Opacity="0.6" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Wrapping and trimming",
                Description = "TextWrapping breaks long text across lines; TextTrimming adds an ellipsis instead.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="360">
                  <TextBlock TextWrapping="Wrap"
                             Text="This paragraph wraps onto as many lines as it needs, which is what you usually want for body copy inside a panel of a fixed width." />
                  <TextBlock TextTrimming="CharacterEllipsis"
                             Text="This single line is trimmed with an ellipsis once it runs out of room." />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Inline runs",
                Description = "Mix formatting inside one block using Run and LineBreak.",
                Xaml = """
                <TextBlock xmlns="https://github.com/avaloniaui" TextWrapping="Wrap" Width="380">
                  <Run Text="You can mix " />
                  <Run Text="bold" FontWeight="Bold" />
                  <Run Text=", " />
                  <Run Text="italic" FontStyle="Italic" />
                  <Run Text=" and " />
                  <Run Text="coloured" Foreground="#E81123" />
                  <Run Text=" runs in a single TextBlock." />
                  <LineBreak />
                  <Run Text="LineBreak starts a new line." Foreground="#808080" />
                </TextBlock>
                """,
            },
        },
    };

    private static ControlPage SelectableTextBlockPage() => new ControlPage
    {
        Name = "SelectableTextBlock",
        Summary = "A TextBlock whose text the user can select and copy.",
        Samples =
        {
            new ControlSample
            {
                Title = "Selectable copy",
                Description = "Drag across the text, or press Ctrl+A then Ctrl+C.",
                Xaml = """
                <SelectableTextBlock xmlns="https://github.com/avaloniaui"
                                     Width="400"
                                     TextWrapping="Wrap"
                                     SelectionBrush="#663B78FF"
                                     Text="Select any part of this sentence and copy it with Ctrl+C. Useful for error messages, IDs and log output that people need to paste elsewhere." />
                """,
            },
        },
    };

    private static ControlPage TextBoxPage() => new ControlPage
    {
        Name = "TextBox",
        Summary = "Single or multi line editable text input.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/textbox",
        Samples =
        {
            new ControlSample
            {
                Title = "Common configurations",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="320">
                  <TextBox PlaceholderText="Type something" />
                  <TextBox Text="Pre-filled value" />
                  <TextBox PlaceholderText="Password" PasswordChar="•" />
                  <TextBox Text="Read only" IsReadOnly="True" />
                  <TextBox PlaceholderText="Disabled" IsEnabled="False" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Multi line",
                Description = "AcceptsReturn turns the box into a small editor.",
                Xaml = """
                <TextBox xmlns="https://github.com/avaloniaui"
                         AcceptsReturn="True"
                         TextWrapping="Wrap"
                         Height="120" Width="380"
                         PlaceholderText="Write a few lines…" />
                """,
            },
            new ControlSample
            {
                Title = "With a clear button",
                Description = "The clearButton style class adds an inline clear affordance. "
                            + "Fluent only shows it while the box has focus and is not empty, so click "
                            + "into the field to see it. Note VerticalContentAlignment: the theme binds "
                            + "the inner content presenters to it, and the default (Stretch) would "
                            + "otherwise pin the icon to the top of the box.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="320">
                  <TextBox Classes="clearButton"
                           Text="Click me, then use the ✕"
                           PlaceholderText="Search"
                           VerticalContentAlignment="Center">
                    <TextBox.InnerLeftContent>
                      <PathIcon Data="{StaticResource IconSearch}"
                                Width="13" Height="13"
                                Margin="10,0,4,0"
                                VerticalAlignment="Center" />
                    </TextBox.InnerLeftContent>
                  </TextBox>

                  <TextBox Classes="clearButton"
                           PlaceholderText="Empty boxes show no clear button"
                           VerticalContentAlignment="Center" />
                </StackPanel>
                """,
                CSharp = """
                // The clear button is a style class from the Fluent theme, so it is applied
                // through Classes rather than a property.
                var box = new TextBox
                {
                    Classes = { "clearButton" },
                    Text = "Click me, then use the ✕",
                    PlaceholderText = "Search",

                    // Without this the inner content presenters inherit Stretch and the
                    // icon is drawn against the top edge instead of the middle.
                    VerticalContentAlignment = VerticalAlignment.Center,

                    InnerLeftContent = new PathIcon
                    {
                        Data = Geometry.Parse(
                            "M6.8 1.5a5.3 5.3 0 1 1-3.2 9.5l-2.4 2.4-1.1-1.1 2.4-2.4A5.3 5.3 0 0 1 6.8 1.5z"),
                        Width = 13,
                        Height = 13,
                        Margin = new Thickness(10, 0, 4, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                    },
                };
                """,
            },
        },
    };

    private static ControlPage MaskedTextBoxPage() => new ControlPage
    {
        Name = "MaskedTextBox",
        Summary = "A TextBox that constrains input to a fixed mask.",
        Samples =
        {
            new ControlSample
            {
                Title = "Phone and date masks",
                Description = "0 requires a digit, L a letter. Characters outside the mask are typed for the user.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="280">
                  <MaskedTextBox Mask="(000) 000-0000" />
                  <MaskedTextBox Mask="0000-00-00" />
                  <MaskedTextBox Mask="LL-000" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage AutoCompleteBoxPage() => new ControlPage
    {
        Name = "AutoCompleteBox",
        Summary = "A text box that suggests matches from a list as the user types.",
        Samples =
        {
            new ControlSample
            {
                Title = "Suggesting as you type",
                Description = "Start typing a city name; FilterMode decides how candidates are matched.",
                Xaml = """
                <AutoCompleteBox xmlns="https://github.com/avaloniaui"
                                 ItemsSource="{Binding Cities}"
                                 FilterMode="StartsWith"
                                 PlaceholderText="Type a city"
                                 Width="300" />
                """,
                DataContextFactory = () => new { SampleData.Cities },
            },
            new ControlSample
            {
                Title = "Matching anywhere in the word",
                Xaml = """
                <AutoCompleteBox xmlns="https://github.com/avaloniaui"
                                 ItemsSource="{Binding Cities}"
                                 FilterMode="Contains"
                                 MinimumPrefixLength="1"
                                 PlaceholderText="Contains…"
                                 Width="300" />
                """,
                DataContextFactory = () => new { SampleData.Cities },
            },
        },
    };

    private static ControlPage NumericUpDownPage() => new ControlPage
    {
        Name = "NumericUpDown",
        Summary = "Numeric entry with spin buttons, clamping and formatting.",
        Samples =
        {
            new ControlSample
            {
                Title = "Range and increment",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="240">
                  <NumericUpDown Value="5" Minimum="0" Maximum="10" Increment="1" />
                  <NumericUpDown Value="2.5" Minimum="0" Maximum="5" Increment="0.25"
                                 FormatString="F2" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Formatted as currency and percent",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="240">
                  <NumericUpDown Value="1250" FormatString="C0" Increment="50" />
                  <NumericUpDown Value="0.15" FormatString="P0" Increment="0.05"
                                 Minimum="0" Maximum="1" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage LabelPage() => new ControlPage
    {
        Name = "Label",
        Summary = "A caption that can forward focus to the control it describes.",
        Samples =
        {
            new ControlSample
            {
                Title = "Target and access key",
                Description = "Press Alt+N to move focus to the box via the underscored letter.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="6" Width="300">
                  <Label Content="_Name" Target="{Binding #NameBox}" />
                  <TextBox Name="NameBox" PlaceholderText="Alt+N focuses me" />
                </StackPanel>
                """,
            },
        },
    };
}
