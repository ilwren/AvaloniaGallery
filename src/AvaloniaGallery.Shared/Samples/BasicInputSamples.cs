using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>Buttons, toggles and the other primitives users click on.</summary>
internal static class BasicInputSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Basic input",
        IconData = Icons.Cursor,
        NameKey = "Cat.BasicInput",
        SourceFile = "BasicInputSamples.cs",
    }
    .With(Button(), RepeatButton(), ToggleButton(), CheckBoxPage(), RadioButtonPage(),
          ToggleSwitchPage(), SplitButtonPage(), ToggleSplitButtonPage(), DropDownButtonPage(),
          HyperlinkButtonPage(), ButtonSpinnerPage(), SliderPage(), ComboBoxPage());

    private static ControlPage Button() => new ControlPage
    {
        Name = "Button",
        Summary = "Raises a Click event when the user presses it. The workhorse of every UI.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/buttons/button",
        Samples =
        {
            new ControlSample
            {
                Title = "A simple button",
                Description = "Content can be any object; a string gets a TextBlock for free.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="12">
                  <Button Content="Standard" />
                  <Button Content="Accent" Classes="accent" />
                  <Button Content="Disabled" IsEnabled="False" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Rich content",
                Description = "Put a panel inside a Button when a label alone is not enough.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="12">
                  <Button>
                    <StackPanel Orientation="Horizontal" Spacing="8">
                      <PathIcon Width="16" Height="16"
                                Data="M6 2a1 1 0 0 1 1 1v6h6a1 1 0 1 1 0 2H7v6a1 1 0 1 1-2 0v-6H-1a1 1 0 1 1 0-2h6V3a1 1 0 0 1 1-1z" />
                      <TextBlock Text="With an icon" VerticalAlignment="Center" />
                    </StackPanel>
                  </Button>
                  <Button>
                    <StackPanel Spacing="2">
                      <TextBlock Text="Two lines" FontWeight="SemiBold" />
                      <TextBlock Text="with a subtitle" FontSize="11" Opacity="0.7" />
                    </StackPanel>
                  </Button>
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Handling clicks",
                Description = "Bind Command for MVVM, or attach a Click handler in code-behind.",
                // No Click attribute here: the gallery parses this markup at runtime, where
                // there is no code-behind class to resolve a handler name against. The C#
                // tab shows how it is wired in a real project.
                Xaml = """
                <Button xmlns="https://github.com/avaloniaui" Content="Click me" />
                """,
                CSharp = """
                // MyView.axaml
                // <Button Content="Click me" Click="OnButtonClick" />

                private void OnButtonClick(object? sender, RoutedEventArgs e)
                {
                    // The sender is the Button that was pressed.
                    if (sender is Button b)
                        b.Content = "Thanks!";
                }
                """,
            },
        },
    };

    private static ControlPage RepeatButton() => new ControlPage
    {
        Name = "RepeatButton",
        Summary = "Fires Click repeatedly for as long as it stays pressed.",
        Samples =
        {
            new ControlSample
            {
                Title = "Press and hold",
                Description = "Delay sets the pause before repeating; Interval the rate afterwards.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="12">
                  <RepeatButton Content="Hold me" Delay="500" Interval="100" />
                  <RepeatButton Content="Faster" Delay="250" Interval="40" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ToggleButton() => new ControlPage
    {
        Name = "ToggleButton",
        Summary = "A button that stays in an on or off state once clicked.",
        Samples =
        {
            new ControlSample
            {
                Title = "Two state",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="12">
                  <ToggleButton Content="Off by default" />
                  <ToggleButton Content="On by default" IsChecked="True" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Three state",
                Description = "With IsThreeState the button also accepts an indeterminate value.",
                Xaml = """
                <ToggleButton xmlns="https://github.com/avaloniaui"
                              Content="Cycles through three states"
                              IsThreeState="True" />
                """,
            },
        },
    };

    private static ControlPage CheckBoxPage() => new ControlPage
    {
        Name = "CheckBox",
        Summary = "Lets the user select one or more independent options.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/checkbox",
        Samples =
        {
            new ControlSample
            {
                Title = "Checked, unchecked, indeterminate",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui"
                            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            Spacing="8">
                  <CheckBox Content="Unchecked" />
                  <CheckBox Content="Checked" IsChecked="True" />
                  <CheckBox Content="Indeterminate" IsChecked="{x:Null}" IsThreeState="True" />
                  <CheckBox Content="Disabled" IsEnabled="False" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "A parent that summarises its children",
                Description = "A three state parent is the usual way to show a partial selection.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui"
                            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            Spacing="6">
                  <CheckBox Content="Select all" IsThreeState="True" IsChecked="{x:Null}" />
                  <StackPanel Margin="24,0,0,0" Spacing="6">
                    <CheckBox Content="Documents" IsChecked="True" />
                    <CheckBox Content="Pictures" />
                    <CheckBox Content="Music" IsChecked="True" />
                  </StackPanel>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage RadioButtonPage() => new ControlPage
    {
        Name = "RadioButton",
        Summary = "Picks exactly one option from a set. Buttons sharing a GroupName are mutually exclusive.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/radiobutton",
        Samples =
        {
            new ControlSample
            {
                Title = "A group of options",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="8">
                  <RadioButton GroupName="Size" Content="Small" />
                  <RadioButton GroupName="Size" Content="Medium" IsChecked="True" />
                  <RadioButton GroupName="Size" Content="Large" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Two independent groups",
                Description = "GroupName keeps the columns from interfering with each other.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="48">
                  <StackPanel Spacing="8">
                    <TextBlock Text="Size" FontWeight="SemiBold" />
                    <RadioButton GroupName="S" Content="Small" IsChecked="True" />
                    <RadioButton GroupName="S" Content="Large" />
                  </StackPanel>
                  <StackPanel Spacing="8">
                    <TextBlock Text="Colour" FontWeight="SemiBold" />
                    <RadioButton GroupName="C" Content="Red" IsChecked="True" />
                    <RadioButton GroupName="C" Content="Blue" />
                  </StackPanel>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ToggleSwitchPage() => new ControlPage
    {
        Name = "ToggleSwitch",
        Summary = "An on/off switch. Prefer it over a CheckBox when the change applies immediately.",
        Samples =
        {
            new ControlSample
            {
                Title = "Default and custom labels",
                Description = "OnContent and OffContent replace the default On / Off wording.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12">
                  <ToggleSwitch IsChecked="True" />
                  <ToggleSwitch OnContent="Enabled" OffContent="Disabled" />
                  <ToggleSwitch OnContent="Connected" OffContent="Offline" IsEnabled="False" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage SplitButtonPage() => new ControlPage
    {
        Name = "SplitButton",
        Summary = "A primary action plus a dropdown holding related, secondary actions.",
        Samples =
        {
            new ControlSample
            {
                Title = "Action with a flyout",
                Xaml = """
                <SplitButton xmlns="https://github.com/avaloniaui" Content="Save">
                  <SplitButton.Flyout>
                    <MenuFlyout Placement="Bottom">
                      <MenuItem Header="Save as…" />
                      <MenuItem Header="Save a copy" />
                      <Separator />
                      <MenuItem Header="Save all" />
                    </MenuFlyout>
                  </SplitButton.Flyout>
                </SplitButton>
                """,
            },
        },
    };

    private static ControlPage ToggleSplitButtonPage() => new ControlPage
    {
        Name = "ToggleSplitButton",
        Summary = "A SplitButton whose primary half toggles instead of invoking.",
        Samples =
        {
            new ControlSample
            {
                Title = "Toggle plus options",
                Xaml = """
                <ToggleSplitButton xmlns="https://github.com/avaloniaui" Content="Bullets" IsChecked="True">
                  <ToggleSplitButton.Flyout>
                    <MenuFlyout Placement="Bottom">
                      <MenuItem Header="Disc" />
                      <MenuItem Header="Circle" />
                      <MenuItem Header="Square" />
                    </MenuFlyout>
                  </ToggleSplitButton.Flyout>
                </ToggleSplitButton>
                """,
            },
        },
    };

    private static ControlPage DropDownButtonPage() => new ControlPage
    {
        Name = "DropDownButton",
        Summary = "Opens a flyout on click. Unlike SplitButton it has no separate primary action.",
        Samples =
        {
            new ControlSample
            {
                Title = "Menu on click",
                Xaml = """
                <DropDownButton xmlns="https://github.com/avaloniaui" Content="Sort by">
                  <DropDownButton.Flyout>
                    <MenuFlyout Placement="Bottom">
                      <MenuItem Header="Name" />
                      <MenuItem Header="Date modified" />
                      <MenuItem Header="Size" />
                    </MenuFlyout>
                  </DropDownButton.Flyout>
                </DropDownButton>
                """,
            },
        },
    };

    private static ControlPage HyperlinkButtonPage() => new ControlPage
    {
        Name = "HyperlinkButton",
        Summary = "A button styled as a link that can open a URI directly.",
        Samples =
        {
            new ControlSample
            {
                Title = "Navigating to a URI",
                Description = "Setting NavigateUri lets the control open the link itself.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="8" HorizontalAlignment="Left">
                  <HyperlinkButton Content="Avalonia documentation"
                                   NavigateUri="https://docs.avaloniaui.net" />
                  <HyperlinkButton Content="Already visited"
                                   NavigateUri="https://avaloniaui.net"
                                   IsVisited="True" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ButtonSpinnerPage() => new ControlPage
    {
        Name = "ButtonSpinner",
        Summary = "Wraps arbitrary content with increase and decrease spin buttons.",
        Samples =
        {
            new ControlSample
            {
                Title = "Spinner placement",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" HorizontalAlignment="Left">
                  <ButtonSpinner Content="Right hand spinner" Width="240" />
                  <ButtonSpinner Content="Left hand spinner" Width="240"
                                 ButtonSpinnerLocation="Left" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage SliderPage() => new ControlPage
    {
        Name = "Slider",
        Summary = "Selects a value from a continuous or stepped range by dragging a thumb.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/slider",
        Samples =
        {
            new ControlSample
            {
                Title = "Horizontal and vertical",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="32">
                  <Slider Minimum="0" Maximum="100" Value="40" Width="240" />
                  <Slider Minimum="0" Maximum="100" Value="60"
                          Orientation="Vertical" Height="140" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Ticks and snapping",
                Description = "TickFrequency spaces the marks; IsSnapToTickEnabled forces the thumb onto them.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="16">
                  <Slider Minimum="0" Maximum="10" Value="4" Width="320"
                          TickFrequency="1" TickPlacement="BottomRight"
                          IsSnapToTickEnabled="True" />
                  <Slider Minimum="0" Maximum="10" Value="7" Width="320"
                          TickFrequency="2.5" TickPlacement="Outside" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Showing the current value",
                Description = "Bind another control to the slider with an element name binding.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="10" Width="320">
                  <Slider Name="Volume" Minimum="0" Maximum="100" Value="35" />
                  <TextBlock Text="{Binding #Volume.Value, StringFormat='Volume: {0:F0}%'}" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ComboBoxPage() => new ControlPage
    {
        Name = "ComboBox",
        Summary = "A dropdown list for choosing a single item from many.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/combobox",
        Samples =
        {
            new ControlSample
            {
                Title = "Inline items",
                Xaml = """
                <ComboBox xmlns="https://github.com/avaloniaui" SelectedIndex="0" Width="220">
                  <ComboBoxItem>Light</ComboBoxItem>
                  <ComboBoxItem>Dark</ComboBoxItem>
                  <ComboBoxItem>Follow system</ComboBoxItem>
                </ComboBox>
                """,
            },
            new ControlSample
            {
                Title = "Bound to a collection",
                Description = "PlaceholderText shows while nothing is selected.",
                Xaml = """
                <ComboBox xmlns="https://github.com/avaloniaui"
                          ItemsSource="{Binding Fruits}"
                          PlaceholderText="Pick a fruit"
                          Width="220" />
                """,
                DataContextFactory = () => new { SampleData.Fruits },
            },
            new ControlSample
            {
                Title = "Custom item template",
                Xaml = """
                <ComboBox xmlns="https://github.com/avaloniaui"
                          ItemsSource="{Binding People}"
                          SelectedIndex="0"
                          Width="260">
                  <ComboBox.ItemTemplate>
                    <DataTemplate>
                      <StackPanel Orientation="Horizontal" Spacing="10">
                        <Border Width="24" Height="24" CornerRadius="12"
                                Background="{DynamicResource SystemAccentColorLight1}">
                          <TextBlock Text="{Binding First[0]}"
                                     Foreground="White"
                                     HorizontalAlignment="Center"
                                     VerticalAlignment="Center" />
                        </Border>
                        <TextBlock Text="{Binding Full}" VerticalAlignment="Center" />
                      </StackPanel>
                    </DataTemplate>
                  </ComboBox.ItemTemplate>
                </ComboBox>
                """,
                DataContextFactory = () => new { SampleData.People },
            },
        },
    };
}
