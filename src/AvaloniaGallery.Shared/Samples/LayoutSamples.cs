using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>Panels and containers that arrange other controls.</summary>
internal static class LayoutSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Layout",
        IconData = Icons.Layout,
        NameKey = "Cat.Layout",
        SourceFile = "LayoutSamples.cs",
    }
    .With(StackPanelPage(), GridPage(), DockPanelPage(), WrapPanelPage(), UniformGridPage(),
          CanvasPage(), RelativePanelPage(), BorderPage(), ScrollViewerPage(), ViewboxPage(),
          GroupBoxPage(), ExpanderPage(), SplitViewPage(), GridSplitterPage(),
          LayoutTransformControlPage(), ThemeVariantScopePage());

    private static ControlPage StackPanelPage() => new ControlPage
    {
        Name = "StackPanel",
        Summary = "Stacks children in a single row or column.",
        Samples =
        {
            new ControlSample
            {
                Title = "Orientation and spacing",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="16">
                  <StackPanel Orientation="Horizontal" Spacing="8">
                    <Button Content="One" />
                    <Button Content="Two" />
                    <Button Content="Three" />
                  </StackPanel>
                  <StackPanel Orientation="Vertical" Spacing="8" Width="120">
                    <Button Content="Top" />
                    <Button Content="Middle" />
                    <Button Content="Bottom" />
                  </StackPanel>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage GridPage() => new ControlPage
    {
        Name = "Grid",
        Summary = "Arranges children in rows and columns with star, auto and fixed sizing.",
        DocsUrl = "https://docs.avaloniaui.net/docs/reference/controls/grid",
        Samples =
        {
            new ControlSample
            {
                Title = "Star and auto sizing",
                Description = "The shorthand attributes keep simple grids to a single line.",
                Xaml = """
                <Grid xmlns="https://github.com/avaloniaui"
                      ColumnDefinitions="Auto,*,2*"
                      RowDefinitions="Auto,Auto"
                      Width="440">
                  <Border Grid.Row="0" Grid.Column="0" Background="#3B78FF" Height="44" Margin="2">
                    <TextBlock Text="Auto" Foreground="White" Margin="10"
                               HorizontalAlignment="Center" VerticalAlignment="Center" />
                  </Border>
                  <Border Grid.Row="0" Grid.Column="1" Background="#107C10" Height="44" Margin="2">
                    <TextBlock Text="*" Foreground="White"
                               HorizontalAlignment="Center" VerticalAlignment="Center" />
                  </Border>
                  <Border Grid.Row="0" Grid.Column="2" Background="#C239B3" Height="44" Margin="2">
                    <TextBlock Text="2*" Foreground="White"
                               HorizontalAlignment="Center" VerticalAlignment="Center" />
                  </Border>
                  <Border Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="3"
                          Background="#5D5A58" Height="36" Margin="2">
                    <TextBlock Text="ColumnSpan=3" Foreground="White"
                               HorizontalAlignment="Center" VerticalAlignment="Center" />
                  </Border>
                </Grid>
                """,
            },
        },
    };

    private static ControlPage DockPanelPage() => new ControlPage
    {
        Name = "DockPanel",
        Summary = "Docks children to an edge; the last child fills what is left.",
        Samples =
        {
            new ControlSample
            {
                Title = "Docking to all four edges",
                Xaml = """
                <DockPanel xmlns="https://github.com/avaloniaui" Width="420" Height="180">
                  <Border DockPanel.Dock="Top" Background="#3B78FF" Height="36" Margin="2">
                    <TextBlock Text="Top" Foreground="White" Margin="8" />
                  </Border>
                  <Border DockPanel.Dock="Bottom" Background="#107C10" Height="36" Margin="2">
                    <TextBlock Text="Bottom" Foreground="White" Margin="8" />
                  </Border>
                  <Border DockPanel.Dock="Left" Background="#C239B3" Width="80" Margin="2">
                    <TextBlock Text="Left" Foreground="White" Margin="8" />
                  </Border>
                  <Border Background="#5D5A58" Margin="2">
                    <TextBlock Text="Fills the rest" Foreground="White" Margin="8" />
                  </Border>
                </DockPanel>
                """,
            },
        },
    };

    private static ControlPage WrapPanelPage() => new ControlPage
    {
        Name = "WrapPanel",
        Summary = "Lays children out in sequence, wrapping onto a new line when out of room.",
        Samples =
        {
            new ControlSample
            {
                Title = "Wrapping chips",
                Xaml = """
                <WrapPanel xmlns="https://github.com/avaloniaui" Width="380">
                  <Button Content="Alpha" Margin="0,0,6,6" />
                  <Button Content="Beta" Margin="0,0,6,6" />
                  <Button Content="Gamma" Margin="0,0,6,6" />
                  <Button Content="Delta" Margin="0,0,6,6" />
                  <Button Content="Epsilon" Margin="0,0,6,6" />
                  <Button Content="Zeta" Margin="0,0,6,6" />
                  <Button Content="Eta" Margin="0,0,6,6" />
                  <Button Content="Theta" Margin="0,0,6,6" />
                </WrapPanel>
                """,
            },
        },
    };

    private static ControlPage UniformGridPage() => new ControlPage
    {
        Name = "UniformGrid",
        Summary = "A grid where every cell is the same size.",
        Samples =
        {
            new ControlSample
            {
                Title = "Fixed number of columns",
                Xaml = """
                <UniformGrid xmlns="https://github.com/avaloniaui" Columns="4" Width="360" Height="120">
                  <Border Background="#3B78FF" Margin="2" />
                  <Border Background="#107C10" Margin="2" />
                  <Border Background="#C239B3" Margin="2" />
                  <Border Background="#CA5010" Margin="2" />
                  <Border Background="#5D5A58" Margin="2" />
                  <Border Background="#0099BC" Margin="2" />
                  <Border Background="#7A7574" Margin="2" />
                  <Border Background="#8764B8" Margin="2" />
                </UniformGrid>
                """,
            },
        },
    };

    private static ControlPage CanvasPage() => new ControlPage
    {
        Name = "Canvas",
        Summary = "Absolute positioning via attached Left, Top, Right and Bottom properties.",
        Samples =
        {
            new ControlSample
            {
                Title = "Absolute coordinates",
                Xaml = """
                <Canvas xmlns="https://github.com/avaloniaui" Width="380" Height="160"
                        Background="{DynamicResource GalleryCheckerBrush}">
                  <Border Canvas.Left="20" Canvas.Top="20" Width="90" Height="50"
                          Background="#3B78FF" CornerRadius="4" />
                  <Border Canvas.Left="140" Canvas.Top="60" Width="90" Height="50"
                          Background="#107C10" CornerRadius="4" />
                  <Border Canvas.Right="20" Canvas.Bottom="20" Width="90" Height="50"
                          Background="#C239B3" CornerRadius="4" />
                </Canvas>
                """,
            },
        },
    };

    private static ControlPage RelativePanelPage() => new ControlPage
    {
        Name = "RelativePanel",
        Summary = "Positions children relative to each other or to the panel edges.",
        Samples =
        {
            new ControlSample
            {
                Title = "Aligning to siblings",
                Xaml = """
                <RelativePanel xmlns="https://github.com/avaloniaui" Width="380" Height="140">
                  <Border Name="Anchor" Width="110" Height="46" Background="#3B78FF" CornerRadius="4" />
                  <Border Width="110" Height="46" Background="#107C10" CornerRadius="4"
                          RelativePanel.RightOf="Anchor" Margin="8,0,0,0" />
                  <Border Width="110" Height="46" Background="#C239B3" CornerRadius="4"
                          RelativePanel.Below="Anchor" Margin="0,8,0,0" />
                  <Border Width="110" Height="46" Background="#CA5010" CornerRadius="4"
                          RelativePanel.AlignRightWithPanel="True"
                          RelativePanel.AlignBottomWithPanel="True" />
                </RelativePanel>
                """,
            },
        },
    };

    private static ControlPage BorderPage() => new ControlPage
    {
        Name = "Border",
        Summary = "Draws a background, border and rounded corners around a single child.",
        Samples =
        {
            new ControlSample
            {
                Title = "Corners, thickness and shadow",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="16">
                  <Border Background="#3B78FF" Width="110" Height="80" CornerRadius="4">
                    <TextBlock Text="Square" Foreground="White"
                               HorizontalAlignment="Center" VerticalAlignment="Center" />
                  </Border>
                  <Border Background="#107C10" Width="110" Height="80" CornerRadius="20">
                    <TextBlock Text="Rounded" Foreground="White"
                               HorizontalAlignment="Center" VerticalAlignment="Center" />
                  </Border>
                  <Border BorderBrush="#C239B3" BorderThickness="2" Width="110" Height="80"
                          CornerRadius="8" BoxShadow="0 4 12 0 #33000000">
                    <TextBlock Text="Outlined" HorizontalAlignment="Center" VerticalAlignment="Center" />
                  </Border>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ScrollViewerPage() => new ControlPage
    {
        Name = "ScrollViewer",
        Summary = "Adds scrolling to content larger than the space available.",
        Samples =
        {
            new ControlSample
            {
                Title = "Scrollbar visibility",
                Xaml = """
                <ScrollViewer xmlns="https://github.com/avaloniaui"
                              Width="320" Height="160"
                              VerticalScrollBarVisibility="Auto"
                              HorizontalScrollBarVisibility="Disabled">
                  <StackPanel Spacing="8" Margin="4">
                    <TextBlock TextWrapping="Wrap"
                               Text="Scroll to see the rest of this content. The vertical bar appears only when it is needed because the visibility is set to Auto." />
                    <Border Height="70" Background="#3B78FF" CornerRadius="4" />
                    <Border Height="70" Background="#107C10" CornerRadius="4" />
                    <Border Height="70" Background="#C239B3" CornerRadius="4" />
                  </StackPanel>
                </ScrollViewer>
                """,
            },
        },
    };

    private static ControlPage ViewboxPage() => new ControlPage
    {
        Name = "Viewbox",
        Summary = "Scales its child to fill the available space.",
        Samples =
        {
            new ControlSample
            {
                Title = "Stretch modes",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="16">
                  <Border BorderBrush="#40808080" BorderThickness="1" Width="120" Height="90">
                    <Viewbox Stretch="Uniform">
                      <TextBlock Text="Uniform" />
                    </Viewbox>
                  </Border>
                  <Border BorderBrush="#40808080" BorderThickness="1" Width="120" Height="90">
                    <Viewbox Stretch="Fill">
                      <TextBlock Text="Fill" />
                    </Viewbox>
                  </Border>
                  <Border BorderBrush="#40808080" BorderThickness="1" Width="120" Height="90">
                    <Viewbox Stretch="UniformToFill">
                      <TextBlock Text="UTF" />
                    </Viewbox>
                  </Border>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage GroupBoxPage() => new ControlPage
    {
        Name = "GroupBox",
        Summary = "A titled frame around related controls. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Grouping related settings",
                Xaml = """
                <GroupBox xmlns="https://github.com/avaloniaui" Header="Notifications" Width="300">
                  <StackPanel Spacing="8" Margin="4">
                    <CheckBox Content="Email" IsChecked="True" />
                    <CheckBox Content="Push" />
                    <CheckBox Content="SMS" />
                  </StackPanel>
                </GroupBox>
                """,
            },
        },
    };

    private static ControlPage ExpanderPage() => new ControlPage
    {
        Name = "Expander",
        Summary = "A header the user can click to reveal or hide a panel of content.",
        Samples =
        {
            new ControlSample
            {
                Title = "Expand direction",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="10" Width="340">
                  <Expander Header="Expands downwards" IsExpanded="True">
                    <TextBlock Margin="4" TextWrapping="Wrap"
                               Text="Content revealed when the expander is open." />
                  </Expander>
                  <Expander Header="Expands upwards" ExpandDirection="Up">
                    <TextBlock Margin="4" Text="This one opens towards the top." />
                  </Expander>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage SplitViewPage() => new ControlPage
    {
        Name = "SplitView",
        Summary = "A collapsible side pane next to the main content. The basis of most nav shells.",
        Samples =
        {
            new ControlSample
            {
                Title = "Inline pane",
                Description = "Toggle the button to open and close the pane.",
                PreviewHeight = 230,
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Spacing="8">
                  <ToggleButton Name="PaneToggle" Content="Toggle pane" IsChecked="True" />
                  <SplitView IsPaneOpen="{Binding #PaneToggle.IsChecked}"
                             DisplayMode="Inline"
                             OpenPaneLength="150"
                             Width="440" Height="150">
                    <SplitView.Pane>
                      <StackPanel Margin="10" Spacing="8">
                        <TextBlock Text="Pane" FontWeight="SemiBold" />
                        <TextBlock Text="Navigation" Opacity="0.7" />
                        <TextBlock Text="Settings" Opacity="0.7" />
                      </StackPanel>
                    </SplitView.Pane>
                    <Border Background="{DynamicResource GalleryCheckerBrush}">
                      <TextBlock Text="Main content" Margin="16" />
                    </Border>
                  </SplitView>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage GridSplitterPage() => new ControlPage
    {
        Name = "GridSplitter",
        Summary = "Lets the user drag to resize adjacent grid rows or columns.",
        Samples =
        {
            new ControlSample
            {
                Title = "Resizable columns",
                Description = "Drag the divider to redistribute space.",
                Xaml = """
                <Grid xmlns="https://github.com/avaloniaui"
                      ColumnDefinitions="*,Auto,*" Width="440" Height="130">
                  <Border Grid.Column="0" Background="#3B78FF">
                    <TextBlock Text="Left" Foreground="White" Margin="10" />
                  </Border>
                  <GridSplitter Grid.Column="1" Width="6" Background="#40808080" />
                  <Border Grid.Column="2" Background="#107C10">
                    <TextBlock Text="Right" Foreground="White" Margin="10" />
                  </Border>
                </Grid>
                """,
            },
        },
    };

    private static ControlPage LayoutTransformControlPage() => new ControlPage
    {
        Name = "LayoutTransformControl",
        Summary = "Applies a transform that the layout system accounts for, unlike RenderTransform.",
        Samples =
        {
            new ControlSample
            {
                Title = "Rotation that affects layout",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="24">
                  <LayoutTransformControl>
                    <LayoutTransformControl.LayoutTransform>
                      <RotateTransform Angle="-90" />
                    </LayoutTransformControl.LayoutTransform>
                    <TextBlock Text="Rotated label" />
                  </LayoutTransformControl>
                  <LayoutTransformControl>
                    <LayoutTransformControl.LayoutTransform>
                      <ScaleTransform ScaleX="1.6" ScaleY="1.6" />
                    </LayoutTransformControl.LayoutTransform>
                    <Button Content="Scaled" />
                  </LayoutTransformControl>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage ThemeVariantScopePage() => new ControlPage
    {
        Name = "ThemeVariantScope",
        Summary = "Overrides the light or dark variant for one part of the tree.",
        Samples =
        {
            new ControlSample
            {
                Title = "Light and dark side by side",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="16">
                  <ThemeVariantScope RequestedThemeVariant="Light">
                    <Border Background="{DynamicResource SystemRegionBrush}" Padding="16" CornerRadius="6">
                      <StackPanel Spacing="8">
                        <TextBlock Text="Always light" />
                        <Button Content="Button" />
                        <CheckBox Content="Checkbox" IsChecked="True" />
                      </StackPanel>
                    </Border>
                  </ThemeVariantScope>
                  <ThemeVariantScope RequestedThemeVariant="Dark">
                    <Border Background="{DynamicResource SystemRegionBrush}" Padding="16" CornerRadius="6">
                      <StackPanel Spacing="8">
                        <TextBlock Text="Always dark" />
                        <Button Content="Button" />
                        <CheckBox Content="Checkbox" IsChecked="True" />
                      </StackPanel>
                    </Border>
                  </ThemeVariantScope>
                </StackPanel>
                """,
            },
        },
    };
}
