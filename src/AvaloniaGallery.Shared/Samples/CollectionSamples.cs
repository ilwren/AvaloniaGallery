using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>Controls that present a collection of items.</summary>
internal static class CollectionSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Collections",
        IconData = Icons.Collections,
        NameKey = "Cat.Collections",
        SourceFile = "CollectionSamples.cs",
    }
    .With(ItemsControlPage(), ListBoxPage(), TableViewPage(), DataGridPage(), TreeViewPage(),
          TabControlPage(), TabStripPage(), CarouselPage(), VirtualizingStackPanelPage());

    private static ControlPage ItemsControlPage() => new ControlPage
    {
        Name = "ItemsControl",
        Summary = "Repeats a template over a collection with no selection or chrome of its own.",
        Samples =
        {
            new ControlSample
            {
                Title = "Templated repetition",
                Description = "ItemsPanel swaps the layout; here a WrapPanel turns the list into chips.",
                Xaml = """
                <ItemsControl xmlns="https://github.com/avaloniaui" ItemsSource="{Binding Fruits}" Width="420">
                  <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                      <WrapPanel />
                    </ItemsPanelTemplate>
                  </ItemsControl.ItemsPanel>
                  <ItemsControl.ItemTemplate>
                    <DataTemplate>
                      <Border Margin="0,0,6,6" Padding="10,4"
                              CornerRadius="10"
                              Background="{DynamicResource SystemAccentColorLight2}">
                        <TextBlock Text="{Binding}" FontSize="12" />
                      </Border>
                    </DataTemplate>
                  </ItemsControl.ItemTemplate>
                </ItemsControl>
                """,
                DataContextFactory = () => new { SampleData.Fruits },
            },
        },
    };

    private static ControlPage ListBoxPage() => new ControlPage
    {
        Name = "ListBox",
        Summary = "A scrollable, selectable list. Virtualised by default.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/listbox",
        Samples =
        {
            new ControlSample
            {
                Title = "Single selection",
                Xaml = """
                <ListBox xmlns="https://github.com/avaloniaui"
                         ItemsSource="{Binding Fruits}"
                         SelectedIndex="2"
                         Width="260" Height="180" />
                """,
                DataContextFactory = () => new { SampleData.Fruits },
            },
            new ControlSample
            {
                Title = "Multiple selection",
                Description = "Ctrl+click to add to the selection, Shift+click to extend it.",
                Xaml = """
                <ListBox xmlns="https://github.com/avaloniaui"
                         ItemsSource="{Binding Cities}"
                         SelectionMode="Multiple"
                         Width="260" Height="180" />
                """,
                DataContextFactory = () => new { SampleData.Cities },
            },
            new ControlSample
            {
                Title = "Rich rows",
                Xaml = """
                <ListBox xmlns="https://github.com/avaloniaui"
                         ItemsSource="{Binding People}"
                         Width="320" Height="200">
                  <ListBox.ItemTemplate>
                    <DataTemplate>
                      <Grid ColumnDefinitions="Auto,*,Auto" Margin="0,4">
                        <Border Grid.Column="0" Width="32" Height="32" CornerRadius="16"
                                Background="{DynamicResource SystemAccentColor}">
                          <TextBlock Text="{Binding First[0]}" Foreground="White"
                                     HorizontalAlignment="Center" VerticalAlignment="Center" />
                        </Border>
                        <StackPanel Grid.Column="1" Margin="10,0" VerticalAlignment="Center">
                          <TextBlock Text="{Binding Full}" FontWeight="SemiBold" />
                          <TextBlock Text="Contributor" FontSize="11" Opacity="0.65" />
                        </StackPanel>
                        <TextBlock Grid.Column="2" Text="{Binding Age}"
                                   VerticalAlignment="Center" Opacity="0.7" />
                      </Grid>
                    </DataTemplate>
                  </ListBox.ItemTemplate>
                </ListBox>
                """,
                DataContextFactory = () => new { SampleData.People },
            },
        },
    };

    private static ControlPage TableViewPage() => new ControlPage
    {
        Name = "TableView",
        Summary = "Read-only tabular data with resizable columns. New in Avalonia 12 and fully open source.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Columns bound to properties",
                Description = "Each TableViewColumn takes a Binding; Width accepts star or absolute sizes.",
                PreviewHeight = 300,
                Xaml = """
                <TableView xmlns="https://github.com/avaloniaui"
                           ItemsSource="{Binding Countries}"
                           CanUserResizeColumns="True"
                           Height="260" Width="620">
                  <TableView.Columns>
                    <TableViewColumn Header="Country" Binding="{Binding Name}" Width="2*" />
                    <TableViewColumn Header="Region" Binding="{Binding Region}" Width="1.5*" />
                    <TableViewColumn Header="Population"
                                     Binding="{Binding Population, StringFormat=N0}"
                                     Width="1.5*"
                                     HorizontalContentAlignment="Right" />
                    <TableViewColumn Header="Area km²"
                                     Binding="{Binding Area, StringFormat=N0}"
                                     Width="1.2*"
                                     HorizontalContentAlignment="Right" />
                  </TableView.Columns>
                </TableView>
                """,
                DataContextFactory = () => new { SampleData.Countries },
            },
        },
    };

    private static ControlPage DataGridPage() => new ControlPage
    {
        Name = "DataGrid",
        Summary = "Editable grid with sorting and grouping. Ships in the separate Avalonia.Controls.DataGrid package.",
        Namespace = "Avalonia.Controls",
        Assembly = "Avalonia.Controls.DataGrid",
        Samples =
        {
            new ControlSample
            {
                Title = "Auto generated columns",
                Description = "Requires the Avalonia.Controls.DataGrid package and its theme include.",
                PreviewHeight = 300,
                Xaml = """
                <DataGrid xmlns="https://github.com/avaloniaui"
                          ItemsSource="{Binding Countries}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          GridLinesVisibility="Horizontal"
                          Height="260" Width="620">
                  <DataGrid.Columns>
                    <DataGridTextColumn Header="Country" Binding="{Binding Name}" Width="2*" />
                    <DataGridTextColumn Header="Region" Binding="{Binding Region}" Width="*" />
                    <DataGridTextColumn Header="Population"
                                        Binding="{Binding Population, StringFormat=N0}" Width="*" />
                  </DataGrid.Columns>
                </DataGrid>
                """,
                DataContextFactory = () => new { SampleData.Countries },
            },
        },
    };

    private static ControlPage TreeViewPage() => new ControlPage
    {
        Name = "TreeView",
        Summary = "Displays hierarchical data with expandable nodes.",
        Samples =
        {
            new ControlSample
            {
                Title = "Hierarchical template",
                Description = "TreeDataTemplate binds ItemsSource to the child collection of each node.",
                PreviewHeight = 280,
                Xaml = """
                <TreeView xmlns="https://github.com/avaloniaui"
                          ItemsSource="{Binding Tree}"
                          Width="320" Height="240">
                  <TreeView.ItemTemplate>
                    <TreeDataTemplate ItemsSource="{Binding Children}">
                      <TextBlock Text="{Binding Title}" />
                    </TreeDataTemplate>
                  </TreeView.ItemTemplate>
                </TreeView>
                """,
                DataContextFactory = () => new { SampleData.Tree },
            },
        },
    };

    private static ControlPage TabControlPage() => new ControlPage
    {
        Name = "TabControl",
        Summary = "Switches between pages of content using a strip of tabs.",
        Samples =
        {
            new ControlSample
            {
                Title = "Tabs with content",
                Xaml = """
                <TabControl xmlns="https://github.com/avaloniaui" Width="420" Height="180">
                  <TabItem Header="Overview">
                    <TextBlock Margin="12" TextWrapping="Wrap"
                               Text="Each TabItem holds arbitrary content, shown when its tab is active." />
                  </TabItem>
                  <TabItem Header="Details">
                    <StackPanel Margin="12" Spacing="8">
                      <TextBox PlaceholderText="A field on the second tab" />
                      <CheckBox Content="A checkbox" />
                    </StackPanel>
                  </TabItem>
                  <TabItem Header="Disabled" IsEnabled="False" />
                </TabControl>
                """,
            },
            new ControlSample
            {
                Title = "Tabs on the left",
                Xaml = """
                <TabControl xmlns="https://github.com/avaloniaui" TabStripPlacement="Left"
                            Width="420" Height="160">
                  <TabItem Header="General">
                    <TextBlock Margin="12" Text="TabStripPlacement moves the strip." />
                  </TabItem>
                  <TabItem Header="Advanced">
                    <TextBlock Margin="12" Text="Handy for settings dialogs." />
                  </TabItem>
                </TabControl>
                """,
            },
        },
    };

    private static ControlPage TabStripPage() => new ControlPage
    {
        Name = "TabStrip",
        Summary = "Just the tab headers, without the content host. Useful for driving your own views.",
        Samples =
        {
            new ControlSample
            {
                Title = "Headers only",
                Xaml = """
                <TabStrip xmlns="https://github.com/avaloniaui" SelectedIndex="0">
                  <TabStripItem>All</TabStripItem>
                  <TabStripItem>Unread</TabStripItem>
                  <TabStripItem>Flagged</TabStripItem>
                </TabStrip>
                """,
            },
        },
    };

    private static ControlPage CarouselPage() => new ControlPage
    {
        Name = "Carousel",
        Summary = "Shows one item at a time with an optional transition between them.",
        Samples =
        {
            new ControlSample
            {
                Title = "Sliding between pages",
                Description = "Use the buttons to move; PageTransition animates the change.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="10">
                  <Carousel Name="Gallery" Width="360" Height="140" SelectedIndex="0">
                    <Carousel.PageTransition>
                      <PageSlide Duration="0:0:0.3" Orientation="Horizontal" />
                    </Carousel.PageTransition>
                    <Border Background="#3B78FF" CornerRadius="6">
                      <TextBlock Text="First" Foreground="White" FontSize="24"
                                 HorizontalAlignment="Center" VerticalAlignment="Center" />
                    </Border>
                    <Border Background="#107C10" CornerRadius="6">
                      <TextBlock Text="Second" Foreground="White" FontSize="24"
                                 HorizontalAlignment="Center" VerticalAlignment="Center" />
                    </Border>
                    <Border Background="#C239B3" CornerRadius="6">
                      <TextBlock Text="Third" Foreground="White" FontSize="24"
                                 HorizontalAlignment="Center" VerticalAlignment="Center" />
                    </Border>
                  </Carousel>
                  <StackPanel Orientation="Horizontal" Spacing="8">
                    <Button Content="Previous" Command="{Binding #Gallery.Previous}" />
                    <Button Content="Next" Command="{Binding #Gallery.Next}" />
                  </StackPanel>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage VirtualizingStackPanelPage() => new ControlPage
    {
        Name = "VirtualizingStackPanel",
        Summary = "Realises only the visible containers, keeping huge lists responsive.",
        Samples =
        {
            new ControlSample
            {
                Title = "Ten thousand rows",
                Description = "Scrolling stays smooth because only the visible rows exist as controls.",
                PreviewHeight = 280,
                Xaml = """
                <ListBox xmlns="https://github.com/avaloniaui"
                         ItemsSource="{Binding ManyItems}"
                         Width="300" Height="240">
                  <ListBox.ItemsPanel>
                    <ItemsPanelTemplate>
                      <VirtualizingStackPanel />
                    </ItemsPanelTemplate>
                  </ListBox.ItemsPanel>
                </ListBox>
                """,
                DataContextFactory = () => new { SampleData.ManyItems },
            },
        },
    };
}
