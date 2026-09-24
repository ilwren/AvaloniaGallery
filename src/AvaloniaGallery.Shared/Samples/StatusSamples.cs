using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>Progress, notifications and other status surfaces.</summary>
internal static class StatusSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Status and info",
        IconData = Icons.Status,
        NameKey = "Cat.Status",
        SourceFile = "StatusSamples.cs",
    }
    .With(ProgressBarPage(), PipsPagerPage(), RefreshContainerPage(),
          WindowNotificationManagerPage(), ToolTipServicePage(), DataValidationErrorsPage());

    private static ControlPage ProgressBarPage() => new ControlPage
    {
        Name = "ProgressBar",
        Summary = "Shows determinate progress or an indeterminate busy state.",
        Samples =
        {
            new ControlSample
            {
                Title = "Determinate and indeterminate",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="16" Width="320">
                  <ProgressBar Value="35" Maximum="100" />
                  <ProgressBar Value="70" Maximum="100" ShowProgressText="True" />
                  <ProgressBar IsIndeterminate="True" />
                </StackPanel>
                """,
            },
            new ControlSample
            {
                Title = "Vertical orientation",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="24">
                  <ProgressBar Orientation="Vertical" Value="60" Height="120" />
                  <ProgressBar Orientation="Vertical" IsIndeterminate="True" Height="120" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage PipsPagerPage() => new ControlPage
    {
        Name = "PipsPager",
        Summary = "Dot based paging indicator, typically paired with a Carousel. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Dots with navigation buttons",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="16">
                  <PipsPager NumberOfPages="5" SelectedPageIndex="0" />
                  <PipsPager NumberOfPages="8"
                             MaxVisiblePips="5"
                             IsPreviousButtonVisible="True"
                             IsNextButtonVisible="True" />
                  <PipsPager NumberOfPages="4" Orientation="Vertical" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage RefreshContainerPage() => new ControlPage
    {
        Name = "RefreshContainer",
        Summary = "Adds pull to refresh to a scrollable region.",
        Samples =
        {
            new ControlSample
            {
                Title = "Pull down to refresh",
                Description = "Drag the list downwards past its top edge to trigger the visualiser.",
                PreviewHeight = 230,
                Xaml = """
                <RefreshContainer xmlns="https://github.com/avaloniaui" Width="300" Height="200">
                  <ScrollViewer>
                    <ItemsControl ItemsSource="{Binding Fruits}">
                      <ItemsControl.ItemTemplate>
                        <DataTemplate>
                          <Border Padding="10,8" Margin="0,0,0,2"
                                  Background="{DynamicResource GalleryCheckerBrush}">
                            <TextBlock Text="{Binding}" />
                          </Border>
                        </DataTemplate>
                      </ItemsControl.ItemTemplate>
                    </ItemsControl>
                  </ScrollViewer>
                </RefreshContainer>
                """,
                DataContextFactory = () => new { SampleData.Fruits },
            },
        },
    };

    private static ControlPage WindowNotificationManagerPage() => new ControlPage
    {
        Name = "WindowNotificationManager",
        Summary = "Displays toast style notifications layered over a window.",
        Samples =
        {
            new ControlSample
            {
                Title = "Showing a toast",
                Description = "The manager attaches to a TopLevel and is driven from code.",
                Xaml = """
                <NotificationCard xmlns="https://github.com/avaloniaui" Width="340">
                  <StackPanel Spacing="4">
                    <TextBlock Text="Upload complete" FontWeight="SemiBold" />
                    <TextBlock Text="3 files were copied to the server." TextWrapping="Wrap" />
                  </StackPanel>
                </NotificationCard>
                """,
                CSharp = """
                // Create once per window, usually in OnApplyTemplate or the constructor.
                var manager = new WindowNotificationManager(TopLevel.GetTopLevel(this))
                {
                    Position = NotificationPosition.TopRight,
                    MaxItems = 3,
                };

                manager.Show(new Notification(
                    "Upload complete",
                    "3 files were copied to the server.",
                    NotificationType.Success));
                """,
            },
        },
    };

    private static ControlPage ToolTipServicePage() => new ControlPage
    {
        Name = "NotificationCard",
        Summary = "The card used to render a single notification. Shown here on its own.",
        Samples =
        {
            new ControlSample
            {
                Title = "Notification styles",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="10" Width="340">
                  <NotificationCard Classes="information">
                    <TextBlock Text="An informational message." Margin="4" />
                  </NotificationCard>
                  <NotificationCard Classes="success">
                    <TextBlock Text="Everything worked." Margin="4" />
                  </NotificationCard>
                  <NotificationCard Classes="warning">
                    <TextBlock Text="Something needs attention." Margin="4" />
                  </NotificationCard>
                  <NotificationCard Classes="error">
                    <TextBlock Text="Something went wrong." Margin="4" />
                  </NotificationCard>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage DataValidationErrorsPage() => new ControlPage
    {
        Name = "DataValidationErrors",
        Summary = "Renders validation messages produced by bindings next to the offending control.",
        Samples =
        {
            new ControlSample
            {
                Title = "Error adorner",
                Description = "Attach errors to any control to show the standard red adorner.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="6" Width="300">
                  <TextBlock Text="Email" FontWeight="SemiBold" FontSize="12" />
                  <TextBox Text="not-an-email" Classes="error" />
                  <TextBlock Text="Enter a valid email address."
                             Foreground="#E81123" FontSize="12" />
                </StackPanel>
                """,
                CSharp = """
                // In a view model, implement INotifyDataErrorInfo and the binding engine
                // will route messages into DataValidationErrors automatically.
                public IEnumerable GetErrors(string? propertyName) =>
                    propertyName == nameof(Email) && !Email.Contains('@')
                        ? new[] { "Enter a valid email address." }
                        : Array.Empty<string>();
                """,
            },
        },
    };
}
