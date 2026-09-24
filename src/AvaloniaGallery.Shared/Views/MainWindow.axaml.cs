using Avalonia.Controls;
using AvaloniaGallery.Models;

namespace AvaloniaGallery.Views;

/// <summary>
/// Desktop shell. All behaviour lives in <see cref="MainView"/> so that the browser and
/// any future mobile head show exactly the same gallery.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    /// <summary>Used by the screenshot harness to drive navigation from outside.</summary>
    internal void Navigate(ControlPage page) => Root.NavigateTo(page);
}
