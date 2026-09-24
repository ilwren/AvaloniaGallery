using AvaloniaGallery.Samples;
using Xunit;

namespace AvaloniaGallery.Tests;

/// <summary>Structural checks on the catalogue itself.</summary>
public class RegistryTests
{
    [Fact]
    public void Every_page_has_at_least_one_sample()
    {
        var empty = SampleRegistry.AllPages
            .Where(p => p.Samples.Count == 0)
            .Select(p => p.Name)
            .ToList();

        Assert.True(empty.Count == 0, "Pages with no samples: " + string.Join(", ", empty));
    }

    [Fact]
    public void Page_names_are_unique()
    {
        var dupes = SampleRegistry.AllPages
            .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.True(dupes.Count == 0, "Duplicate page names: " + string.Join(", ", dupes));
    }

    [Fact]
    public void Every_page_belongs_to_a_category()
    {
        Assert.All(SampleRegistry.AllPages, p => Assert.False(string.IsNullOrWhiteSpace(p.CategoryName)));
    }

    [Fact]
    public void Every_sample_declares_xaml()
    {
        foreach (var page in SampleRegistry.AllPages)
            foreach (var sample in page.Samples)
                Assert.False(string.IsNullOrWhiteSpace(sample.Xaml),
                    $"{page.Name} / {sample.Title} has no XAML.");
    }

    [Fact]
    public void Search_finds_a_known_control()
    {
        var hits = SampleRegistry.Search("button").ToList();
        Assert.Contains(hits, p => p.Name == "Button");
    }

    [Fact]
    public void Search_is_case_insensitive()
    {
        Assert.Contains(SampleRegistry.Search("SLIDER"), p => p.Name == "Slider");
    }

    [Fact]
    public void Search_with_empty_term_returns_nothing()
    {
        Assert.Empty(SampleRegistry.Search(""));
        Assert.Empty(SampleRegistry.Search(null));
    }
}
