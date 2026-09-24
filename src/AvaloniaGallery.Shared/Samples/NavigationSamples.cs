using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>
/// The Page family introduced in Avalonia 12, plus the content hosts it builds on.
/// </summary>
internal static class NavigationSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Navigation",
        IconData = Icons.Navigation,
        NameKey = "Cat.Navigation",
        SourceFile = "NavigationSamples.cs",
    }
    .With(ContentControlPage(), UserControlPage(), TransitioningContentControlPage(),
          ContentPagePage(), NavigationPagePage(), TabbedPagePage(), DrawerPagePage(),
          CarouselPagePage(), PopupPage());

    private static ControlPage ContentControlPage() => new ControlPage
    {
        Name = "ContentControl",
        Summary = "Hosts a single piece of content, rendered through a DataTemplate when needed.",
        Samples =
        {
            new ControlSample
            {
                Title = "Content plus template",
                Description = "Bind Content to an object and let ContentTemplate decide how it looks.",
                Xaml = """
                <ContentControl xmlns="https://github.com/avaloniaui" Content="{Binding Selected}">
                  <ContentControl.ContentTemplate>
                    <DataTemplate>
                      <Border Background="{DynamicResource GalleryCheckerBrush}"
                              CornerRadius="6" Padding="14">
                        <StackPanel Spacing="4">
                          <TextBlock Text="{Binding Full}" FontWeight="SemiBold" />
                          <TextBlock Text="{Binding Age, StringFormat='Age {0}'}" Opacity="0.7" />
                        </StackPanel>
                      </Border>
                    </DataTemplate>
                  </ContentControl.ContentTemplate>
                </ContentControl>
                """,
                DataContextFactory = () => new { Selected = SampleData.People[0] },
            },
        },
    };

    private static ControlPage UserControlPage() => new ControlPage
    {
        Name = "UserControl",
        Summary = "The base class for composing your own reusable views in XAML.",
        Samples =
        {
            new ControlSample
            {
                Title = "Defining a view",
                Description = "A UserControl is just a ContentControl you subclass in your own project.",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        Background="{DynamicResource GalleryCheckerBrush}"
                        CornerRadius="6" Padding="16" Width="340">
                  <StackPanel Spacing="8">
                    <TextBlock Text="A composed view" FontWeight="SemiBold" />
                    <TextBlock TextWrapping="Wrap" Opacity="0.75"
                               Text="In a real project the markup below would live in its own .axaml file." />
                  </StackPanel>
                </Border>
                """,
                CSharp = """
                // MyView.axaml.cs
                public partial class MyView : UserControl
                {
                    public MyView() => InitializeComponent();
                }
                """,
            },
        },
    };

    private static ControlPage TransitioningContentControlPage() => new ControlPage
    {
        Name = "TransitioningContentControl",
        Summary = "A ContentControl that animates whenever its content is replaced.",
        Samples =
        {
            new ControlSample
            {
                Title = "Cross fade between views",
                Description = "Pick a colour: the old panel fades out while the new one fades in. "
                            + "Binding Content to the selected item is what makes the transition run — "
                            + "toggling IsVisible on a single child would just pop it in and out.",
                PreviewHeight = 240,
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="380">
                  <ListBox Name="ViewPicker" SelectedIndex="0">
                    <ListBox.ItemsPanel>
                      <ItemsPanelTemplate>
                        <StackPanel Orientation="Horizontal" Spacing="8" />
                      </ItemsPanelTemplate>
                    </ListBox.ItemsPanel>
                    <ListBoxItem Content="Ocean" />
                    <ListBoxItem Content="Forest" />
                    <ListBoxItem Content="Sunset" />
                  </ListBox>

                  <TransitioningContentControl Height="130"
                                               Content="{Binding #ViewPicker.SelectedItem}">
                    <TransitioningContentControl.PageTransition>
                      <CrossFade Duration="0:0:0.45" />
                    </TransitioningContentControl.PageTransition>
                    <TransitioningContentControl.ContentTemplate>
                      <DataTemplate>
                        <Border CornerRadius="8" Background="#3B78FF">
                          <TextBlock Text="{Binding Content}"
                                     Foreground="White"
                                     FontSize="22"
                                     HorizontalAlignment="Center"
                                     VerticalAlignment="Center" />
                        </Border>
                      </DataTemplate>
                    </TransitioningContentControl.ContentTemplate>
                  </TransitioningContentControl>
                </StackPanel>
                """,
                CSharp = """
                // The transition runs on every Content change, so drive Content from your
                // selection rather than toggling the visibility of a fixed child.
                var host = new TransitioningContentControl
                {
                    Height = 130,
                    PageTransition = new CrossFade(TimeSpan.FromSeconds(0.45)),
                };

                picker.SelectionChanged += (_, _) => host.Content = picker.SelectedItem;

                // Other built-in transitions:
                //   new PageSlide(TimeSpan.FromSeconds(0.3), PageSlide.SlideAxis.Horizontal)
                //   new CompositePageTransition { PageTransitions = { … } }
                """,
            },
        },
    };

    private static ControlPage ContentPagePage() => new ControlPage
    {
        Name = "ContentPage",
        Summary = "A page with a header and optional command bars. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Header plus content",
                PreviewHeight = 240,
                Xaml = """
                <ContentPage xmlns="https://github.com/avaloniaui"
                             Header="Page title"
                             Width="420" Height="200">
                  <StackPanel Margin="16" Spacing="8">
                    <TextBlock TextWrapping="Wrap"
                               Text="ContentPage gives a page a header and safe area padding, which matters on mobile." />
                    <Button Content="An action" />
                  </StackPanel>
                </ContentPage>
                """,
            },
        },
    };

    private static ControlPage NavigationPagePage() => new ControlPage
    {
        Name = "NavigationPage",
        Summary = "A stack based navigation host with a title bar and back button. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Push and pop",
                Description = "NavigationPage keeps a stack of pages and animates between them.",
                PreviewHeight = 240,
                Xaml = """
                <NavigationPage xmlns="https://github.com/avaloniaui" Width="420" Height="200">
                  <ContentPage Header="Root page">
                    <TextBlock Margin="16" TextWrapping="Wrap"
                               Text="Call PushAsync to add a page to the stack; the back button appears automatically." />
                  </ContentPage>
                </NavigationPage>
                """,
                CSharp = """
                // Navigate forward
                await navigationPage.PushAsync(new ContentPage { Header = "Details" });

                // And back again
                await navigationPage.PopAsync();
                """,
            },
        },
    };

    private static ControlPage TabbedPagePage() => new ControlPage
    {
        Name = "TabbedPage",
        Summary = "Hosts several pages behind a tab bar. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Tabs of pages",
                PreviewHeight = 260,
                Xaml = """
                <TabbedPage xmlns="https://github.com/avaloniaui" Width="420" Height="220">
                  <ContentPage Header="Home">
                    <TextBlock Margin="16" Text="The first tab." />
                  </ContentPage>
                  <ContentPage Header="Search">
                    <TextBlock Margin="16" Text="The second tab." />
                  </ContentPage>
                  <ContentPage Header="Profile">
                    <TextBlock Margin="16" Text="The third tab." />
                  </ContentPage>
                </TabbedPage>
                """,
            },
        },
    };

    private static ControlPage DrawerPagePage() => new ControlPage
    {
        Name = "DrawerPage",
        Summary = "A page with a slide-in drawer, the usual mobile navigation pattern. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Slide in drawer",
                PreviewHeight = 260,
                Xaml = """
                <DrawerPage xmlns="https://github.com/avaloniaui" IsOpen="True"
                            DrawerLength="150" Width="440" Height="220">
                  <DrawerPage.Drawer>
                    <Border Background="{DynamicResource GalleryCheckerBrush}">
                      <StackPanel Margin="14" Spacing="10">
                        <TextBlock Text="Menu" FontWeight="SemiBold" />
                        <TextBlock Text="Home" Opacity="0.75" />
                        <TextBlock Text="Library" Opacity="0.75" />
                        <TextBlock Text="Settings" Opacity="0.75" />
                      </StackPanel>
                    </Border>
                  </DrawerPage.Drawer>
                  <ContentPage Header="Content">
                    <TextBlock Margin="16" Text="The drawer sits beside or over this area." />
                  </ContentPage>
                </DrawerPage>
                """,
            },
        },
    };

    private static ControlPage CarouselPagePage() => new ControlPage
    {
        Name = "CarouselPage",
        Summary = "Swipes horizontally between a set of pages. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Swipeable pages",
                Description = "Drag horizontally across the coloured area, or use the buttons, to "
                            + "move between pages. SelectedIndex is bound both ways, so the counter "
                            + "below follows a swipe and the buttons drive the carousel.",
                PreviewHeight = 330,
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="12" Width="440">
                  <CarouselPage Name="Pages" Height="200" SelectedIndex="0">
                    <CarouselPage.PageTransition>
                      <PageSlide Duration="0:0:0.35" Orientation="Horizontal" />
                    </CarouselPage.PageTransition>

                    <ContentPage Header="One">
                      <Border Background="#3B78FF">
                        <TextBlock Text="Page 1 — drag me sideways"
                                   Foreground="White" FontSize="18"
                                   HorizontalAlignment="Center" VerticalAlignment="Center" />
                      </Border>
                    </ContentPage>
                    <ContentPage Header="Two">
                      <Border Background="#107C10">
                        <TextBlock Text="Page 2"
                                   Foreground="White" FontSize="18"
                                   HorizontalAlignment="Center" VerticalAlignment="Center" />
                      </Border>
                    </ContentPage>
                    <ContentPage Header="Three">
                      <Border Background="#C239B3">
                        <TextBlock Text="Page 3"
                                   Foreground="White" FontSize="18"
                                   HorizontalAlignment="Center" VerticalAlignment="Center" />
                      </Border>
                    </ContentPage>
                  </CarouselPage>

                  <StackPanel Orientation="Horizontal" Spacing="10" HorizontalAlignment="Center">
                    <TextBlock Text="Page index" VerticalAlignment="Center" />
                    <NumericUpDown Width="130"
                                   Minimum="0" Maximum="2" Increment="1"
                                   FormatString="0"
                                   Value="{Binding #Pages.SelectedIndex, Mode=TwoWay}" />
                  </StackPanel>
                </StackPanel>
                """,
                CSharp = """
                // CarouselPage derives from SelectingMultiPage, so SelectedIndex is a direct
                // property: it round-trips with a two-way binding and follows swipe gestures.
                var pages = new CarouselPage
                {
                    Height = 200,
                    SelectedIndex = 0,
                    PageTransition = new PageSlide(TimeSpan.FromSeconds(0.35)),
                };

                pages.SelectionChanged += (_, _) =>
                    Console.WriteLine($"Now on page {pages.SelectedIndex}");

                // There is no Next()/Previous() helper — move by assigning SelectedIndex.
                pages.SelectedIndex = Math.Min(pages.SelectedIndex + 1, 2);
                """,
            },
        },
    };

    private static ControlPage PopupPage() => new ControlPage
    {
        Name = "Popup",
        Summary = "Hosts content in a window layered above the rest of the UI.",
        Samples =
        {
            new ControlSample
            {
                Title = "Anchored popup",
                Description = "Toggle the button to open the popup beneath it.",
                PreviewHeight = 190,
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="8">
                  <ToggleButton Name="PopupToggle" Content="Show popup" />
                  <Popup IsOpen="{Binding #PopupToggle.IsChecked}"
                         PlacementTarget="{Binding #PopupToggle}"
                         Placement="Bottom"
                         IsLightDismissEnabled="True">
                    <Border Background="{DynamicResource SystemRegionBrush}"
                            BorderBrush="{DynamicResource GalleryCardBorderBrush}"
                            BorderThickness="1" CornerRadius="6" Padding="14">
                      <StackPanel Spacing="6" Width="220">
                        <TextBlock Text="Popup content" FontWeight="SemiBold" />
                        <TextBlock TextWrapping="Wrap" Opacity="0.75"
                                   Text="Click elsewhere to dismiss, because light dismiss is on." />
                      </StackPanel>
                    </Border>
                  </Popup>
                </StackPanel>
                """,
            },
        },
    };
}
