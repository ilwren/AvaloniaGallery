using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Models;
using AvaloniaGallery.Samples;

namespace AvaloniaGallery.Views;

/// <summary>
/// The gallery shell: navigation on the left, the selected control's page on the right.
/// <para>
/// This is a <see cref="UserControl"/> rather than a <see cref="Window"/> so the same view
/// serves every platform — the desktop head wraps it in a window, while single view hosts
/// such as the browser set it as their root content directly.
/// </para>
/// </summary>
public partial class MainView : UserControl
{
    private readonly List<NavEntry> _navEntries = new();
    private bool _suppressNavCallback;

    /// <summary>The page currently shown, so it can be rebuilt when the language changes.</summary>
    private ControlPage? _currentPage;

    public MainView()
    {
        InitializeComponent();

        BuildNavigation();
        BuildLanguageMenu();

        // The item list is set once and never replaced. Rebuilding ItemsSource on every
        // keystroke used to drop the very object the dropdown was about to commit, so
        // clicking a suggestion neither completed the text nor navigated. Filtering runs
        // through ItemFilter instead, which narrows the view without touching the source.
        SearchBox.ItemsSource = SampleRegistry.AllPages.Select(p => p.Name).ToList();
        SearchBox.ItemFilter = MatchesSearch;

        // Category headers are translated, so the list has to be re-lettered in place.
        // Loc.LanguageChanged is static and outlives any view, so the subscription is tied
        // to the time this view is actually in a tree. Subscribing in the constructor and
        // unsubscribing only on detach used to leak: a view that was closed without ever
        // being detached stayed subscribed for the life of the process, and later language
        // switches then reached into its dead visual tree from whatever thread set them.
        AttachedToVisualTree += (_, _) => Loc.LanguageChanged += OnLanguageChanged;
        DetachedFromVisualTree += (_, _) => Loc.LanguageChanged -= OnLanguageChanged;

        ShowHome();
    }

    private void BuildNavigation()
    {
        _navEntries.Clear();

        foreach (var category in SampleRegistry.Categories)
        {
            _navEntries.Add(NavEntry.ForCategory(category));

            foreach (var page in category.Pages)
                _navEntries.Add(NavEntry.ForPage(page));
        }

        NavList.ItemsSource = _navEntries;
        NavList.ItemTemplate = new FuncDataTemplate<NavEntry>((entry, _) => BuildNavRow(entry), true);

        // Headers must not be selectable, otherwise clicking one would steal the highlight
        // from the page the user is reading.
        //
        // The list virtualizes, so containers are recycled between rows: every branch has
        // to set the state explicitly. Only tagging headers leaves the flag behind on a
        // container that later hosts a page, which silently makes that page unclickable.
        NavList.ContainerPrepared += (_, e) =>
        {
            if (e.Container is not ListBoxItem item)
                return;

            var isHeader = _navEntries[e.Index].IsHeader;

            item.IsHitTestVisible = !isHeader;
            item.Focusable = !isHeader;
            item.Classes.Set("navHeader", isHeader);
        };
    }

    private static Control BuildNavRow(NavEntry entry)
    {
        if (entry.Category is { } category)
        {
            var row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Margin = new Avalonia.Thickness(0, 6, 0, 2),
            };

            row.Children.Add(new PathIcon
            {
                Data = Geometry.Parse(category.IconData),
                Width = 13,
                Height = 13,
                Opacity = 0.65,
            });

            var label = new TextBlock
            {
                FontSize = 11,
                FontWeight = FontWeight.SemiBold,
                Opacity = 0.65,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Bound through the localization hub so switching language updates the header
            // without rebuilding the list and losing the current selection.
            label.Bind(TextBlock.TextProperty, new Avalonia.Data.Binding
            {
                Source = LocSource.For(category.NameKey ?? category.Name),
                Path = nameof(LocSource.Value),
            });

            row.Children.Add(label);
            return row;
        }

        var page = entry.Page!;
        var pageRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 7 };

        pageRow.Children.Add(new TextBlock
        {
            Text = page.Name,
            VerticalAlignment = VerticalAlignment.Center,
        });

        if (page.IsNew)
        {
            pageRow.Children.Add(new Border
            {
                Classes = { "badge" },
                Child = new TextBlock { Text = "12" },
            });
        }

        return pageRow;
    }

    private void BuildLanguageMenu()
    {
        // The flyout lives in Button.Flyout rather than the visual tree, so the XAML name
        // generator does not produce a field for it — reach it through the button instead.
        if (LanguageButton.Flyout is not MenuFlyout flyout)
            return;

        foreach (var language in Loc.SupportedLanguages)
        {
            var item = new MenuItem { Header = language.NativeName };
            item.Click += (_, _) => Loc.Language = language;
            flyout.Items.Add(item);
        }
    }

    private void OnLanguageChanged()
    {
        // Loc.Language is static and can be set from any thread, but this handler touches
        // the visual tree, which is UI-thread only. Marshal when we are called from
        // elsewhere; running inline on the UI thread keeps the update synchronous so a
        // caller that switches language sees the result immediately.
        // Each AvaloniaObject captures the dispatcher it was created on, which in a test
        // run is not necessarily Dispatcher.UIThread. Marshal to this view's own dispatcher.
        if (!CheckAccess())
        {
            Dispatcher.Post(OnLanguageChanged);
            return;
        }

        // Re-letter whatever is on screen. {loc:Loc} bindings update themselves, but
        // anything that reads the culture when it is constructed does not: a Calendar or
        // DatePicker built while the language was Chinese keeps rendering Chinese month and
        // meridiem names under an English UI until the page is rebuilt. Rebuilding only the
        // home page, as this used to, left every control page stale.
        if (DetailHost.Content is HomeView)
            ShowHome();
        else if (_currentPage is { } page)
            DetailHost.Content = new ControlPageView(page);
    }

    private void ShowHome()
    {
        _suppressNavCallback = true;
        NavList.SelectedItem = null;
        _suppressNavCallback = false;

        _currentPage = null;
        DetailHost.Content = new HomeView(Navigate);
    }

    /// <summary>Navigates to a page from outside the view (used by tests and tooling).</summary>
    internal void NavigateTo(ControlPage page) => Navigate(page);

    /// <summary>Navigates to a page and keeps the navigation list's highlight in step.</summary>
    private void Navigate(ControlPage page)
    {
        var entry = _navEntries.FirstOrDefault(e => ReferenceEquals(e.Page, page));

        if (entry is not null && !ReferenceEquals(NavList.SelectedItem, entry))
        {
            _suppressNavCallback = true;
            NavList.SelectedItem = entry;
            NavList.ScrollIntoView(entry);
            _suppressNavCallback = false;
        }

        _currentPage = page;
        DetailHost.Content = new ControlPageView(page);
    }

    /// <summary>
    /// Applies a window material and clears the shell chrome so it is actually visible.
    /// </summary>
    /// <remarks>
    /// Setting <see cref="TopLevel.TransparencyLevelHint"/> on its own looks like it does
    /// nothing, which is the trap this method exists to close: the material is composited
    /// behind the window surface, so an opaque window background or an opaque root control
    /// hides it completely. The gallery painted both. Toggling the "backdrop" class on the
    /// window and on this view swaps those to transparent, and swaps them back for
    /// <see cref="WindowTransparencyLevel.None"/>.
    /// </remarks>
    /// <returns>The level the platform actually granted, which is often not the one asked for.</returns>
    public WindowTransparencyLevel ApplyWindowMaterial(IReadOnlyList<WindowTransparencyLevel> hint)
    {
        if (TopLevel.GetTopLevel(this) is not Window window)
            return WindowTransparencyLevel.None;

        var wantsMaterial = hint.Count > 0 && hint[0] != WindowTransparencyLevel.None;

        window.TransparencyLevelHint = hint;

        Classes.Set("backdrop", wantsMaterial);
        window.Classes.Set("backdrop", wantsMaterial);

        return window.ActualTransparencyLevel;
    }

    private void OnNavSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressNavCallback)
            return;

        if (NavList.SelectedItem is NavEntry { Page: { } page })
        {
            _currentPage = page;
            DetailHost.Content = new ControlPageView(page);
        }
    }

    private void OnHomeClick(object? sender, RoutedEventArgs e) => ShowHome();

    private void OnThemeClick(object? sender, RoutedEventArgs e)
    {
        // Follow whatever is actually being shown, so the first click always flips
        // away from the current appearance even when the app is on "Default".
        var isDark = ActualThemeVariant == ThemeVariant.Dark;

        // Set the variant on the root so it applies to the whole app, including popups
        // and flyouts that are not children of this view.
        if (Avalonia.Application.Current is { } app)
            app.RequestedThemeVariant = isDark ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    /// <summary>
    /// Matches a suggestion against what the user typed. Searching the summary and category
    /// as well as the name is why this is a custom predicate rather than a built-in
    /// FilterMode: typing "tray" should find TrayIcon, and "tab" should find TabControl.
    /// </summary>
    private static bool MatchesSearch(string? search, object? item)
    {
        if (string.IsNullOrWhiteSpace(search))
            return true;

        if (item is not string name)
            return false;

        if (name.Contains(search, StringComparison.OrdinalIgnoreCase))
            return true;

        // Fall back to the registry's richer search so summaries count too.
        return SampleRegistry.Search(search).Any(p =>
            string.Equals(p.Name, name, StringComparison.Ordinal));
    }

    private void OnSearchSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SearchBox.SelectedItem is string name &&
            SampleRegistry.FindByName(name) is { } page)
        {
            Navigate(page);

            // Committing a suggestion should leave the box showing that page and nothing
            // pending, otherwise the next keystroke reopens a stale dropdown.
            SearchBox.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, false);
        }
    }
}
