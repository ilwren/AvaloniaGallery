using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Models;
using AvaloniaGallery.Services;

namespace AvaloniaGallery.Samples;

/// <summary>Window chrome, dialogs and platform integration.</summary>
internal static class SystemSamples
{
    public static ControlCategory Build() => new ControlCategory
    {
        Name = "System",
        IconData = Icons.System,
        NameKey = "Cat.System",
        SourceFile = "SystemSamples.cs",
    }
    .With(WindowPage(), WindowEffectsPage(), TrayIconPage(), NativeControlHostPage(),
          ManagedFileChooserPage(), AboutAvaloniaDialogPage(), OpenGlControlPage());

    private static ControlPage WindowPage() => new ControlPage
    {
        Name = "Window",
        Summary = "A top level window, with control over chrome, sizing and transparency.",
        Samples =
        {
            new ControlSample
            {
                Title = "Window properties",
                Description = "Set in markup on the Window root, or from code before it is shown.",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        Background="{DynamicResource GalleryCheckerBrush}"
                        CornerRadius="6" Padding="16" Width="400">
                  <StackPanel Spacing="6">
                    <TextBlock Text="A Window cannot be nested inside a page," TextWrapping="Wrap" />
                    <TextBlock Opacity="0.75" TextWrapping="Wrap"
                               Text="so the markup on the right shows how you would declare one instead." />
                  </StackPanel>
                </Border>
                """,
                CSharp = """
                var window = new Window
                {
                    Title = "My window",
                    Width = 900,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,

                    // Borderless with a client side drop shadow
                    SystemDecorations = SystemDecorations.Full,
                    TransparencyLevelHint = new[] { WindowTransparencyLevel.AcrylicBlur },
                };

                window.Show();
                """,
            },
        },
    };

    private static ControlPage TrayIconPage() => new ControlPage
    {
        // Browsers and phones have no notification area; the gallery swaps in an
        // explanatory card there instead of a button that could not work.
        Requires = PlatformRequirement.TrayArea,
        Name = "TrayIcon",
        Summary = "Puts an icon with a menu in the system notification area.",
        Samples =
        {
            new ControlSample
            {
                Title = "A working tray icon",
                Description = "Press the button to register a real tray icon with the OS, then look "
                            + "in your notification area: it has a live menu (show / hide / quit) and "
                            + "left clicking it reactivates the window. TrayIcon belongs to the "
                            + "Application rather than the visual tree, which is why it is declared "
                            + "in App.axaml as shown in the source.",
                // TrayIcon cannot be parsed into a page — it is not a Control and has no
                // place in the visual tree — so the preview is the interactive harness that
                // drives it, while the source pane still shows the real declarative markup.
                LiveContentFactory = TrayIconDemo.Create,
                Xaml = """
                <Application xmlns="https://github.com/avaloniaui"
                             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                             x:Class="MyApp.App">
                  <TrayIcon.Icons>
                    <TrayIcons>
                      <TrayIcon Icon="/Assets/icon.ico" ToolTipText="My app">
                        <TrayIcon.Menu>
                          <NativeMenu>
                            <NativeMenuItem Header="Show" Click="OnShow" />
                            <NativeMenuItem Header="Hide" Click="OnHide" />
                            <NativeMenuItemSeparator />
                            <NativeMenuItem Header="Quit" Click="OnQuit" />
                          </NativeMenu>
                        </TrayIcon.Menu>
                      </TrayIcon>
                    </TrayIcons>
                  </TrayIcon.Icons>
                </Application>
                """,
                CSharp = """
                // Equivalent in code. TrayIcon.SetIcons attaches the collection to the
                // Application; assigning an empty TrayIcons removes the icon again.
                var icon = new TrayIcon
                {
                    Icon = new WindowIcon(iconStream),
                    ToolTipText = "My app",
                    IsVisible = true,
                    Menu = new NativeMenu
                    {
                        Items =
                        {
                            new NativeMenuItem("Show") { Command = showCommand },
                            new NativeMenuItemSeparator(),
                            new NativeMenuItem("Quit") { Command = quitCommand },
                        },
                    },
                };

                icon.Clicked += (_, _) => ShowMainWindow();

                TrayIcon.SetIcons(Application.Current!, new TrayIcons { icon });

                // Remember to clear it on shutdown, or the icon can outlive the process
                // in some Linux notification areas.
                TrayIcon.SetIcons(Application.Current!, new TrayIcons());
                """,
            },
        },
    };

    private static ControlPage NativeControlHostPage() => new ControlPage
    {
        Name = "NativeControlHost",
        Summary = "Embeds a platform native handle, such as an HWND or NSView, inside the tree.",
        Samples =
        {
            new ControlSample
            {
                Title = "Hosting a native handle",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        Background="{DynamicResource GalleryCheckerBrush}"
                        CornerRadius="6" Padding="16" Width="400">
                  <TextBlock TextWrapping="Wrap"
                             Text="Subclass NativeControlHost and override CreateNativeControlCore to return a platform handle." />
                </Border>
                """,
                CSharp = """
                public class MyNativeHost : NativeControlHost
                {
                    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
                    {
                        // Create the HWND / NSView / X11 window here.
                        return base.CreateNativeControlCore(parent);
                    }

                    protected override void DestroyNativeControlCore(IPlatformHandle control)
                        => base.DestroyNativeControlCore(control);
                }
                """,
            },
        },
    };

    private static ControlPage ManagedFileChooserPage() => new ControlPage
    {
        Name = "ManagedFileChooser",
        Summary = "Avalonia's own file dialog, used where the platform has no native one.",
        Assembly = "Avalonia.Dialogs",
        Samples =
        {
            new ControlSample
            {
                Title = "Opting into the managed dialog",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        Background="{DynamicResource GalleryCheckerBrush}"
                        CornerRadius="6" Padding="16" Width="400">
                  <TextBlock TextWrapping="Wrap"
                             Text="Enable the managed chooser during AppBuilder setup, then use the normal StorageProvider API." />
                </Border>
                """,
                CSharp = """
                AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .UseManagedSystemDialogs();   // Avalonia.Dialogs

                // Afterwards the standard storage API routes through it:
                var files = await TopLevel.GetTopLevel(this)!.StorageProvider
                    .OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Open" });
                """,
            },
        },
    };

    private static ControlPage AboutAvaloniaDialogPage() => new ControlPage
    {
        Name = "AboutAvaloniaDialog",
        Summary = "The ready made about box that ships with Avalonia.Dialogs.",
        Assembly = "Avalonia.Dialogs",
        Samples =
        {
            new ControlSample
            {
                Title = "Showing the dialog",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        Background="{DynamicResource GalleryCheckerBrush}"
                        CornerRadius="6" Padding="16" Width="400">
                  <TextBlock TextWrapping="Wrap"
                             Text="A Window subclass, so it is shown as a dialog rather than embedded in a page." />
                </Border>
                """,
                CSharp = """
                await new AboutAvaloniaDialog().ShowDialog(parentWindow);
                """,
            },
        },
    };

    private static ControlPage OpenGlControlPage() => new ControlPage
    {
        Name = "OpenGlControlBase",
        Summary = "Base class for drawing with raw OpenGL inside the Avalonia tree.",
        Assembly = "Avalonia.OpenGL",
        IsInfrastructure = true,
        Samples =
        {
            new ControlSample
            {
                Title = "Custom GL rendering",
                Xaml = """
                <Border xmlns="https://github.com/avaloniaui"
                        Background="{DynamicResource GalleryCheckerBrush}"
                        CornerRadius="6" Padding="16" Width="400">
                  <TextBlock TextWrapping="Wrap"
                             Text="Derive from OpenGlControlBase and override the init, render and deinit hooks." />
                </Border>
                """,
                CSharp = """
                public class GlDemo : OpenGlControlBase
                {
                    protected override void OnOpenGlInit(GlInterface gl) { /* compile shaders */ }

                    protected override void OnOpenGlRender(GlInterface gl, int fb)
                    {
                        gl.ClearColor(0.1f, 0.1f, 0.15f, 1f);
                        gl.Clear((int)GlConsts.GL_COLOR_BUFFER_BIT);
                    }

                    protected override void OnOpenGlDeinit(GlInterface gl) { /* free resources */ }
                }
                """,
            },
        },
    };

    private static ControlPage WindowEffectsPage() => new ControlPage
    {
        // The material is a property of the host window, which only a desktop lifetime has.
        Requires = PlatformRequirement.Desktop,
        Name = "Window materials",
        Summary = "Mica, Acrylic and blur-behind backdrops for the window itself.",
        DocsUrl = "https://docs.avaloniaui.net/docs/guides/platforms/how-to-use-transparency-and-blur",
        Samples =
        {
            new ControlSample
            {
                Title = "Mica, Acrylic and blur",
                Description = "Press a button to apply the material to this window for real. "
                            + "TransparencyLevelHint is a ranked preference list, not a command: "
                            + "the platform picks the first level it can honour, so the readout "
                            + "shows what ActualTransparencyLevel reports back. Mica needs "
                            + "Windows 11, Acrylic needs a compositor that supports it, and on "
                            + "anything else the request degrades to Blur and then to None.",
                // The effect applies to the hosting Window, so there is nothing to parse
                // into the preview: the harness drives the real window instead.
                LiveContentFactory = WindowEffectsDemo.Create,
                Xaml = """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        x:Class="MyApp.MainWindow"
                        Background="Transparent"
                        TransparencyLevelHint="Mica,AcrylicBlur,Blur,None">

                  <!-- ExperimentalAcrylicBorder paints an acrylic material inside the
                       window, which is how you get the effect on platforms whose window
                       manager will not do it for the whole window. -->
                  <Panel>
                    <ExperimentalAcrylicBorder IsHitTestVisible="False">
                      <ExperimentalAcrylicBorder.Material>
                        <ExperimentalAcrylicMaterial BackgroundSource="Digger"
                                                     TintColor="Black"
                                                     TintOpacity="1"
                                                     MaterialOpacity="0.65" />
                      </ExperimentalAcrylicBorder.Material>
                    </ExperimentalAcrylicBorder>

                    <TextBlock Margin="24" Text="Content sits on top of the material." />
                  </Panel>
                </Window>
                """,
                CSharp = """
                // The hint is a ranked list. Asking for Mica alone on a machine that cannot
                // do Mica falls straight back to opaque, so list the softer materials after
                // it and let the platform choose.
                window.TransparencyLevelHint = new[]
                {
                    WindowTransparencyLevel.Mica,
                    WindowTransparencyLevel.AcrylicBlur,
                    WindowTransparencyLevel.Blur,
                    WindowTransparencyLevel.None,
                };

                // A material only shows through if the window is not painting over it.
                window.Background = Brushes.Transparent;

                // What you asked for and what you got are different questions.
                Console.WriteLine(window.ActualTransparencyLevel);
                """,
            },
        },
    };
}
