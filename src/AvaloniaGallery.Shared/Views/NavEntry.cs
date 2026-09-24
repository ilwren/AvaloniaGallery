using AvaloniaGallery.Models;

namespace AvaloniaGallery.Views;

/// <summary>
/// One row in the navigation list.
/// <para>
/// Categories and pages share a single flat list so that one <see cref="Avalonia.Controls.ListBox"/>
/// owns the selection for the whole tree — that is what gives the current page a persistent
/// highlight, which a nested stack of buttons could not do.
/// </para>
/// </summary>
public sealed class NavEntry
{
    private NavEntry(ControlCategory? category, ControlPage? page)
    {
        Category = category;
        Page = page;
    }

    public ControlCategory? Category { get; }

    public ControlPage? Page { get; }

    /// <summary>Category rows are headers: visible, but not selectable.</summary>
    public bool IsHeader => Category is not null;

    public static NavEntry ForCategory(ControlCategory category) => new(category, null);

    public static NavEntry ForPage(ControlPage page) => new(null, page);
}
