using Avalonia.Controls;
using AvaloniaGallery.Services;
using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>Menus, flyouts, toolbars and tooltips.</summary>
internal static class MenuSamples
{
    /// <summary>
    /// The menu the NativeMenuBar sample attaches to the window. Built in code because
    /// <see cref="NativeMenu"/> is exported to the OS rather than rendered from the
    /// snippet's own tree.
    /// </summary>
    private static NativeMenu BuildDemoMenu() => new()
    {
        Items =
        {
            new NativeMenuItem("File")
            {
                Menu = new NativeMenu
                {
                    Items =
                    {
                        new NativeMenuItem("New"),
                        new NativeMenuItem("Open…"),
                        new NativeMenuItemSeparator(),
                        new NativeMenuItem("Save"),
                    },
                },
            },
            new NativeMenuItem("Edit")
            {
                Menu = new NativeMenu
                {
                    Items = { new NativeMenuItem("Undo"), new NativeMenuItem("Redo") },
                },
            },
            new NativeMenuItem("View")
            {
                Menu = new NativeMenu
                {
                    Items = { new NativeMenuItem("Zoom in"), new NativeMenuItem("Zoom out") },
                },
            },
        },
    };

    public static ControlCategory Build() => new ControlCategory
    {
        Name = "Menus and toolbars",
        IconData = Icons.Menus,
        NameKey = "Cat.Menus",
        SourceFile = "MenuSamples.cs",
    }
    .With(MenuPage(), ContextMenuPage(), FlyoutPage(), MenuFlyoutPage(), CommandBarPage(),
          ToolTipPage(), SeparatorPage(), NativeMenuBarPage());

    private static ControlPage MenuPage() => new ControlPage
    {
        Name = "Menu",
        Summary = "A classic application menu bar with nested items.",
        Samples =
        {
            new ControlSample
            {
                Title = "Menu bar with submenus",
                PreviewHeight = 120,
                Xaml = """
                <Menu xmlns="https://github.com/avaloniaui" Width="420" VerticalAlignment="Top">
                  <MenuItem Header="_File">
                    <MenuItem Header="_New" InputGesture="Ctrl+N" />
                    <MenuItem Header="_Open…" InputGesture="Ctrl+O" />
                    <Separator />
                    <MenuItem Header="Recent">
                      <MenuItem Header="Project A" />
                      <MenuItem Header="Project B" />
                    </MenuItem>
                    <Separator />
                    <MenuItem Header="E_xit" />
                  </MenuItem>
                  <MenuItem Header="_Edit">
                    <MenuItem Header="Cut" InputGesture="Ctrl+X" />
                    <MenuItem Header="Copy" InputGesture="Ctrl+C" />
                    <MenuItem Header="Paste" InputGesture="Ctrl+V" />
                  </MenuItem>
                  <MenuItem Header="_View">
                    <MenuItem Header="Show sidebar" ToggleType="CheckBox" IsChecked="True" />
                    <MenuItem Header="Show status bar" ToggleType="CheckBox" />
                  </MenuItem>
                </Menu>
                """,
            },
        },
    };

    private static ControlPage ContextMenuPage() => new ControlPage
    {
        Name = "ContextMenu",
        Summary = "A menu shown on right click, attached to any control.",
        Samples =
        {
            new ControlSample
            {
                Title = "Right click the panel",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        Width="320" Height="110"
                        Background="{DynamicResource GalleryCheckerBrush}"
                        CornerRadius="6">
                  <Border.ContextMenu>
                    <ContextMenu>
                      <MenuItem Header="Cut" />
                      <MenuItem Header="Copy" />
                      <MenuItem Header="Paste" />
                      <Separator />
                      <MenuItem Header="Properties" />
                    </ContextMenu>
                  </Border.ContextMenu>
                  <TextBlock Text="Right click anywhere in this box"
                             HorizontalAlignment="Center" VerticalAlignment="Center" />
                </Border>
                """,
            },
        },
    };

    private static ControlPage FlyoutPage() => new ControlPage
    {
        Name = "Flyout",
        Summary = "Lightweight popup anchored to a control. Not a Control itself but attached to one.",
        Namespace = "Avalonia.Controls",
        Samples =
        {
            new ControlSample
            {
                Title = "Confirmation flyout",
                Xaml = """
                <Button xmlns="https://github.com/avaloniaui" Content="Delete">
                  <Button.Flyout>
                    <Flyout Placement="Bottom">
                      <StackPanel Spacing="10" Width="220">
                        <TextBlock Text="Delete this item?" FontWeight="SemiBold" />
                        <TextBlock Text="This cannot be undone." TextWrapping="Wrap" Opacity="0.75" />
                        <Button Content="Delete" Classes="accent" HorizontalAlignment="Right" />
                      </StackPanel>
                    </Flyout>
                  </Button.Flyout>
                </Button>
                """,
            },
        },
    };

    private static ControlPage MenuFlyoutPage() => new ControlPage
    {
        Name = "MenuFlyout",
        Summary = "A flyout whose content is a list of menu items.",
        Samples =
        {
            new ControlSample
            {
                Title = "Menu attached to a button",
                Xaml = """
                <Button xmlns="https://github.com/avaloniaui" Content="Options">
                  <Button.Flyout>
                    <MenuFlyout Placement="Bottom">
                      <MenuItem Header="Rename" />
                      <MenuItem Header="Duplicate" />
                      <Separator />
                      <MenuItem Header="Delete" />
                    </MenuFlyout>
                  </Button.Flyout>
                </Button>
                """,
            },
        },
    };

    private static ControlPage CommandBarPage() => new ControlPage
    {
        Name = "CommandBar",
        Summary = "A toolbar of primary and overflow commands. New in Avalonia 12.",
        IsNew = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Primary and secondary commands",
                Description = "Items beyond the primary set move into the overflow menu.",
                PreviewHeight = 140,
                Xaml = """
                <CommandBar xmlns="https://github.com/avaloniaui" Width="440" VerticalAlignment="Top">
                  <CommandBar.PrimaryCommands>
                    <CommandBarButton Label="Add" />
                    <CommandBarButton Label="Edit" />
                    <CommandBarButton Label="Share" />
                    <CommandBarSeparator />
                    <CommandBarToggleButton Label="Pin" />
                  </CommandBar.PrimaryCommands>
                  <CommandBar.SecondaryCommands>
                    <CommandBarButton Label="Settings" />
                    <CommandBarButton Label="Help" />
                  </CommandBar.SecondaryCommands>
                </CommandBar>
                """,
            },
        },
    };

    private static ControlPage ToolTipPage() => new ControlPage
    {
        Name = "ToolTip",
        Summary = "Contextual help shown on hover, attached via ToolTip.Tip.",
        Samples =
        {
            new ControlSample
            {
                Title = "Simple and rich tips",
                Description = "Hover over either button to see its tip.",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Orientation="Horizontal" Spacing="12">
                  <Button Content="Plain tip" ToolTip.Tip="A short explanation." />
                  <Button Content="Rich tip">
                    <ToolTip.Tip>
                      <StackPanel Spacing="4" Width="200">
                        <TextBlock Text="Rich content" FontWeight="SemiBold" />
                        <TextBlock TextWrapping="Wrap" Opacity="0.8"
                                   Text="A tip can hold any control, not only text." />
                      </StackPanel>
                    </ToolTip.Tip>
                  </Button>
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage SeparatorPage() => new ControlPage
    {
        Name = "Separator",
        Summary = "A thin rule that divides groups of items.",
        Samples =
        {
            new ControlSample
            {
                Title = "Between items",
                Xaml = """
                <StackPanel xmlns="https://github.com/avaloniaui" Width="260" Spacing="6">
                  <TextBlock Text="Section one" />
                  <Separator />
                  <TextBlock Text="Section two" />
                  <Separator />
                  <TextBlock Text="Section three" />
                </StackPanel>
                """,
            },
        },
    };

    private static ControlPage NativeMenuBarPage() => new ControlPage
    {
        Name = "NativeMenuBar",
        Summary = "Exports the menu to the OS menu bar, which macOS expects at the top of the screen.",
        Samples =
        {
            new ControlSample
            {
                Title = "Platform menu",
                Description = "NativeMenuBar renders nothing on its own — it is a view onto the menu "
                            + "attached to the window via NativeMenu.Menu. On macOS the OS draws that "
                            + "menu in the screen-top bar and the control stays collapsed; on Windows "
                            + "and Linux it falls back to drawing the same items inline, as below.",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        BorderBrush="{DynamicResource GalleryCardBorderBrush}"
                        BorderThickness="1"
                        CornerRadius="6"
                        Width="440">
                  <DockPanel>
                    <!-- NativeMenuBar shows the menu attached to the window, so the
                         NativeMenu below is set on the Window, not on the bar itself. -->
                    <NativeMenuBar DockPanel.Dock="Top" />

                    <TextBlock Margin="14"
                               TextWrapping="Wrap"
                               Text="On macOS this menu moves to the system bar at the top of the screen and the control above collapses to nothing." />
                  </DockPanel>
                </Border>
                """,
                // NativeMenuBar reads NativeMenu.Menu from the TopLevel, so the sample has to
                // attach the menu to the real window for the control to render anything at
                // all. The previous revision declared the menu inside the snippet, which
                // parsed cleanly and then drew an empty bar.
                Attach = control =>
                {
                    var top = TopLevel.GetTopLevel(control);
                    if (top is null)
                        return null;

                    var previous = NativeMenu.GetMenu(top);
                    NativeMenu.SetMenu(top, BuildDemoMenu());

                    return DisposableAction.Create(() => NativeMenu.SetMenu(top, previous));
                },
                CSharp = """
                // Attach the menu to the window, then let NativeMenuBar export it.
                NativeMenu.SetMenu(this, new NativeMenu
                {
                    Items =
                    {
                        new NativeMenuItem("File")
                        {
                            Menu = new NativeMenu
                            {
                                Items = { new NativeMenuItem("Open…"), new NativeMenuItem("Save") },
                            },
                        },
                    },
                });
                """,
            },
        },
    };
}
