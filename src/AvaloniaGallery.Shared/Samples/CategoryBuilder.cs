using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

internal static class CategoryBuilder
{
    /// <summary>
    /// Adds pages to a category and stamps each one with the category name and source file
    /// so the search index, breadcrumb and "Page source" viewer have them without a
    /// back-reference.
    /// </summary>
    public static ControlCategory With(this ControlCategory category, params ControlPage[] pages)
    {
        foreach (var page in pages)
        {
            page.CategoryName = category.Name;
            page.SourceFile = category.SourceFile;
            category.Pages.Add(page);
        }

        return category;
    }
}

/// <summary>Path geometries for the navigation glyphs, kept in one place.</summary>
internal static class Icons
{
    public const string Cursor = "M4 2l10 6-4.2 1.2L12 14l-2 .8-2.4-4.8L4 13z";
    public const string Collections = "M2 3h5v5H2zm7 0h5v5H9zM2 10h5v5H2zm7 0h5v5H9z";
    public const string Text = "M3 3h10v2H9v9H7V5H3z";
    public const string Layout = "M2 2h12v3H2zm0 5h5v7H2zm7 0h5v7H9z";
    public const string DateTime = "M3 3h10a1 1 0 0 1 1 1v9a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1zm0 3v7h10V6z";
    public const string Media = "M2 3h12v10H2zm4 2.5v5l4-2.5z";
    public const string Menus = "M2 3h12v2H2zm0 4h12v2H2zm0 4h8v2H2z";
    public const string Dialogs = "M2 3h12v8H8l-3 3v-3H2z";
    public const string Navigation = "M8 1l7 14H1z";
    public const string Shapes = "M8 1l6 12H2zM2 1h4v4H2z";
    public const string Status = "M2 7h12v2H2zm1-4h10v2H3zm0 8h10v2H3z";
    public const string Color = "M8 1a7 7 0 1 0 0 14c1 0 1.5-.6 1.5-1.3 0-.8-.7-1.2-.7-1.9 0-.5.4-.8 1-.8H11a4 4 0 0 0 4-4C15 3.6 11.9 1 8 1z";
    public const string System = "M6 1h4l.4 2.3 1.9 1.1 2.2-.9 2 3.4-1.8 1.5v2.2l1.8 1.5-2 3.4-2.2-.9-1.9 1.1L10 18H6l-.4-2.3-1.9-1.1-2.2.9-2-3.4L1.3 10V7.8L-.5 6.3l2-3.4 2.2.9 1.9-1.1z";
}
