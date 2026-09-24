# Avalonia Gallery

*[简体中文](README.zh-CN.md)*

A WinUI 3 Gallery style browser for every control Avalonia ships free and open source.
Avalonia 12.1.2, .NET 10, desktop (Windows / macOS / Linux).

Each sample is XAML parsed at runtime, so the source you copy is the source that rendered
the control next to it. Every sample also has a C# tab.

> **Built by AI.** All code, samples, tests and documentation in this repository were written
> by an AI agent. It has had no human code review.

![Home](docs/screenshot.png)

![Control page](docs/screenshot-page.png)

## Build

Requires the **.NET 10 SDK**. It must be 10: the Avalonia 12 source generators are built
against Roslyn 4.14, and on the .NET 8 SDK they fail to load silently — the build then dies
with a wall of `CS0103` instead of a useful error.

```bash
dotnet run --project src/AvaloniaGallery.Desktop   # run
dotnet build                                       # build all
dotnet test                                        # 522 tests
```

No arguments needed: there is a single solution at the root.

`.github/workflows/release.yml` takes a version number and publishes self-contained
single-file builds for `win-x64`, `win-arm64`, `osx-x64`, `osx-arm64`, `linux-x64` and
`linux-arm64`. macOS output is a signed-ready `.app` bundle with an icon.

## Controls

90 controls in 11 categories, 110 runnable examples.

**Basic input** (13)
Button · RepeatButton · ToggleButton · CheckBox · RadioButton · ToggleSwitch · SplitButton ·
ToggleSplitButton · DropDownButton · HyperlinkButton · ButtonSpinner · Slider · ComboBox

**Text** (7)
TextBlock · SelectableTextBlock · TextBox · MaskedTextBox · AutoCompleteBox · NumericUpDown · Label

**Collections** (9)
ItemsControl · ListBox · TableView · DataGrid · TreeView · TabControl · TabStrip · Carousel ·
VirtualizingStackPanel

**Layout** (16)
StackPanel · Grid · DockPanel · WrapPanel · UniformGrid · Canvas · RelativePanel · Border ·
ScrollViewer · Viewbox · GroupBox · Expander · SplitView · GridSplitter · LayoutTransformControl ·
ThemeVariantScope

**Navigation** (9)
ContentControl · UserControl · TransitioningContentControl · ContentPage · NavigationPage ·
TabbedPage · DrawerPage · CarouselPage · Popup

**Menus and toolbars** (8)
Menu · ContextMenu · Flyout · MenuFlyout · CommandBar · ToolTip · Separator · NativeMenuBar

**Date and time** (4)
Calendar · CalendarDatePicker · DatePicker · TimePicker

**Graphics and media** (6)
Shapes · Arc and Sector · Image · PathIcon · Brushes · ExperimentalAcrylicBorder

**Color** (5)
ColorPicker · ColorView · ColorSpectrum · ColorSlider · ColorPreviewer

**Status and info** (6)
ProgressBar · PipsPager · RefreshContainer · WindowNotificationManager · NotificationCard ·
DataValidationErrors

**System** (7)
Window · Window materials · TrayIcon · NativeControlHost · ManagedFileChooser ·
AboutAvaloniaDialog · OpenGlControlBase

**New in Avalonia 12:** TableView · GroupBox · ContentPage · NavigationPage · TabbedPage ·
DrawerPage · CarouselPage · CommandBar · PipsPager

Desktop-only pages (TrayIcon, Window materials, NativeMenuBar) register a real tray icon and
apply real Mica / Acrylic / blur to the live window, and report what the platform actually
granted rather than pretending it worked.

## Not included

The ~50 abstract base classes, presenters and layers (`TemplatedControl`, `ContentPresenter`,
`OverlayLayer`, …) that are infrastructure rather than things you place in a UI.

`TreeDataGrid` is absent: the original stays MIT, but active development moved to a commercial
fork under Accelerate, and `TableView` is the open-source answer for tabular data in 12.x.

Everything shown is MIT licensed and free for commercial use. The Avalonia logo used as the
application icon belongs to the Avalonia project.
