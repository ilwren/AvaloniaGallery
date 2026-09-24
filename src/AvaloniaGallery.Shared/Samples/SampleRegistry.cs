using AvaloniaGallery.Models;

namespace AvaloniaGallery.Samples;

/// <summary>
/// The single catalogue of everything the gallery shows.
/// <para>
/// Adding a control is deliberately a one line change: write a
/// <see cref="ControlPage"/> in the relevant <c>*Samples</c> class and it appears in the
/// navigation pane, the search index and the home page counts automatically.
/// </para>
/// </summary>
public static class SampleRegistry
{
    private static readonly Lazy<IReadOnlyList<ControlCategory>> LazyCategories =
        new(BuildCategories);

    // Derived from Categories rather than assigned as a side effect of building them:
    // touching AllPages first (as the tests and the search box both do) has to work
    // without anyone having read Categories beforehand.
    private static readonly Lazy<IReadOnlyList<ControlPage>> LazyPages =
        new(() => LazyCategories.Value.SelectMany(c => c.Pages).ToList());

    public static IReadOnlyList<ControlCategory> Categories => LazyCategories.Value;

    /// <summary>Flattened view of every page, used by search and by the counters.</summary>
    public static IReadOnlyList<ControlPage> AllPages => LazyPages.Value;

    private static IReadOnlyList<ControlCategory> BuildCategories() => new List<ControlCategory>
    {
        BasicInputSamples.Build(),
        TextSamples.Build(),
        CollectionSamples.Build(),
        LayoutSamples.Build(),
        NavigationSamples.Build(),
        MenuSamples.Build(),
        DateTimeSamples.Build(),
        GraphicsSamples.Build(),
        ColorSamples.Build(),
        StatusSamples.Build(),
        SystemSamples.Build(),
    };

    public static int TotalControls => AllPages.Count;

    public static int TotalSamples => AllPages.Sum(p => p.Samples.Count);

    public static int NewInTwelve => AllPages.Count(p => p.IsNew);

    /// <summary>Case insensitive substring match across name, summary and category.</summary>
    public static IEnumerable<ControlPage> Search(string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Array.Empty<ControlPage>();

        return AllPages
            .Where(p => p.SearchText.Contains(term, StringComparison.OrdinalIgnoreCase))
            // Prefer names that start with the term, then alphabetical.
            .OrderByDescending(p => p.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase))
            .ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase);
    }

    public static ControlPage? FindByName(string name) =>
        AllPages.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
}
