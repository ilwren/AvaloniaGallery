using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Samples;
using AvaloniaGallery.Views;
using Xunit;

namespace AvaloniaGallery.Tests;

/// <summary>
/// Covers the shell itself: the navigation pane has to show which page is open, which the
/// original build of the gallery got wrong (clicking a page left no highlight behind).
/// </summary>
public class NavigationTests
{
    private static (Window Window, MainView View, ListBox Nav) Shell()
    {
        var view = new MainView();
        var window = new Window { Content = view, Width = 1280, Height = 860 };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        var nav = view.GetVisualDescendants().OfType<ListBox>().First(l => l.Name == "NavList");
        return (window, view, nav);
    }

    [AvaloniaFact]
    public void Navigating_selects_the_matching_nav_entry()
    {
        var (window, view, nav) = Shell();

        var page = SampleRegistry.FindByName("Slider")!;
        view.NavigateTo(page);

        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        var selected = Assert.IsType<NavEntry>(nav.SelectedItem);
        Assert.Same(page, selected.Page);

        window.Close();
    }

    [AvaloniaFact]
    public void Home_clears_the_selection()
    {
        var (window, view, nav) = Shell();

        view.NavigateTo(SampleRegistry.FindByName("Button")!);
        Dispatcher.UIThread.RunJobs();
        Assert.NotNull(nav.SelectedItem);

        // The home button is the only way back, and it must not leave a page highlighted.
        var home = view.GetVisualDescendants().OfType<Button>().First(b => b.Name == "HomeButton");
        home.Command?.Execute(null);
        RaiseClick(home);

        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        Assert.Null(nav.SelectedItem);
        window.Close();
    }

    [AvaloniaFact]
    public void Category_headers_are_not_selectable()
    {
        var (window, _, nav) = Shell();

        var headers = nav.GetRealizedContainers()
            .OfType<ListBoxItem>()
            .Where(c => c.DataContext is NavEntry { IsHeader: true })
            .ToList();

        Assert.NotEmpty(headers);
        Assert.All(headers, h => Assert.False(h.IsHitTestVisible,
            "A category header is clickable, so it could steal the selection from a page."));

        window.Close();
    }

    /// <summary>
    /// The list virtualizes, so a container that once held a category header gets reused for
    /// a page. If the header state is not cleared on reuse, that page becomes unclickable —
    /// a bug that only shows up after scrolling.
    /// </summary>
    [AvaloniaFact]
    public void Recycled_containers_do_not_keep_header_state()
    {
        var (window, _, nav) = Shell();

        // Scroll to the end and back to force the containers to be reused.
        nav.ScrollIntoView(SampleRegistry.AllPages.Count);
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        nav.ScrollIntoView(0);
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        foreach (var container in nav.GetRealizedContainers().OfType<ListBoxItem>())
        {
            if (container.DataContext is not NavEntry entry)
                continue;

            Assert.Equal(!entry.IsHeader, container.IsHitTestVisible);
            Assert.Equal(entry.IsHeader, container.Classes.Contains("navHeader"));
        }

        window.Close();
    }

    [AvaloniaFact]
    public void Nav_list_contains_every_page_and_category()
    {
        var (window, _, nav) = Shell();

        var entries = Assert.IsAssignableFrom<IEnumerable<NavEntry>>(nav.ItemsSource).ToList();

        Assert.Equal(SampleRegistry.Categories.Count, entries.Count(e => e.IsHeader));
        Assert.Equal(SampleRegistry.AllPages.Count, entries.Count(e => e.Page is not null));

        window.Close();
    }

    private static void RaiseClick(Button button) =>
        button.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));

    /// <summary>
    /// Committing a suggestion must fill the box in and navigate.
    /// <para>
    /// This used to fail: the search handler rebuilt ItemsSource on every keystroke, so the
    /// item object the dropdown was about to commit no longer existed by the time the
    /// selection landed. The text stayed as the typed prefix and nothing navigated.
    /// Filtering now goes through ItemFilter, which narrows the view without replacing the
    /// source collection.
    /// </para>
    /// </summary>
    [AvaloniaFact]
    public void Choosing_a_suggestion_fills_in_and_navigates()
    {
        var (window, view, _) = Shell();

        var box = view.GetVisualDescendants().OfType<AutoCompleteBox>()
            .First(b => b.Name == "SearchBox");

        // Type a prefix, the way a user would.
        box.Text = "Slid";
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        // Commit a suggestion, which is what clicking a dropdown row does.
        box.SelectedItem = "Slider";
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        Assert.Equal("Slider", box.Text);

        var host = view.GetVisualDescendants().OfType<ContentControl>()
            .First(c => c.Name == "DetailHost");

        Assert.IsType<ControlPageView>(host.Content);
    }

    /// <summary>
    /// The suggestion list must survive typing. Rebuilding it per keystroke is what broke
    /// committing a selection, so the source collection is pinned here.
    /// </summary>
    [AvaloniaFact]
    public void Typing_does_not_replace_the_suggestion_source()
    {
        var (window, view, _) = Shell();

        var box = view.GetVisualDescendants().OfType<AutoCompleteBox>()
            .First(b => b.Name == "SearchBox");

        var before = box.ItemsSource;

        box.Text = "Sli";
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        Assert.Same(before, box.ItemsSource);

        // And the filter still narrows what the user would see.
        Assert.NotNull(box.ItemFilter);
        Assert.True(box.ItemFilter!("Slid", "Slider"));
        Assert.False(box.ItemFilter!("Slid", "Button"));
    }

    /// <summary>
    /// Switching language must rebuild the page on screen, not just the home page.
    /// <para>
    /// {loc:Loc} bindings re-letter themselves, but anything that reads the culture when it
    /// is constructed does not: a Calendar built while the language was Chinese keeps
    /// rendering Chinese month names under an English UI. Rebuilding only HomeView, as this
    /// used to, left every control page stale.
    /// </para>
    /// </summary>
    [AvaloniaFact]
    public void Switching_language_rebuilds_the_open_page()
    {
        var (window, view, _) = Shell();
        var original = Loc.Language;

        try
        {
            view.NavigateTo(SampleRegistry.FindByName("Calendar")!);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var host = view.GetVisualDescendants().OfType<ContentControl>()
                .First(c => c.Name == "DetailHost");

            var before = host.Content;
            Assert.IsType<ControlPageView>(before);

            Loc.Language = Loc.SupportedLanguages.Single(l => l.Code == "zh-Hans");
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.IsType<ControlPageView>(host.Content);
            Assert.NotSame(before, host.Content);
        }
        finally
        {
            Loc.Language = original;
            window.Close();
        }
    }
}
