using AvaloniaGallery.Samples;
using AvaloniaGallery.Services;
using Xunit;

namespace AvaloniaGallery.Tests;

/// <summary>
/// The "Page source" viewer reads the gallery's own C# out of embedded resources. If the
/// embedding or the brace matching breaks, the feature silently shows nothing — so it is
/// checked for every page rather than spot checked.
/// </summary>
public class PageSourceTests
{
    public static TheoryData<string> AllPageNames()
    {
        var data = new TheoryData<string>();

        foreach (var page in SampleRegistry.AllPages)
            data.Add(page.Name);

        return data;
    }

    [Theory]
    [MemberData(nameof(AllPageNames))]
    public void Page_source_can_be_located(string pageName)
    {
        var page = SampleRegistry.FindByName(pageName)!;

        Assert.False(string.IsNullOrEmpty(page.SourceFile),
            $"{pageName} has no SourceFile, so its source cannot be shown.");

        var source = PageSourceProvider.GetPageSource(page.Name, page.SourceFile);

        Assert.False(string.IsNullOrWhiteSpace(source),
            $"No source could be extracted for {pageName} from {page.SourceFile}.");

        // The slice must actually be this page's declaration, not a neighbouring one.
        Assert.Contains($"Name = \"{pageName}\"", source!, StringComparison.Ordinal);
    }

    [Theory]
    [MemberData(nameof(AllPageNames))]
    public void Page_source_is_a_balanced_block(string pageName)
    {
        var page = SampleRegistry.FindByName(pageName)!;
        var source = PageSourceProvider.GetPageSource(page.Name, page.SourceFile)!;

        // A truncated slice is the most likely failure mode of the brace matcher, and it
        // would show the reader a snippet that stops mid-declaration.
        Assert.EndsWith(";", source.TrimEnd(), StringComparison.Ordinal);
        Assert.StartsWith("private static ControlPage", source.TrimStart(), StringComparison.Ordinal);
    }

    [Fact]
    public void Missing_pages_return_null_rather_than_throwing()
    {
        Assert.Null(PageSourceProvider.GetPageSource("NoSuchControl", "BasicInputSamples.cs"));
        Assert.Null(PageSourceProvider.GetPageSource("Button", "NoSuchFile.cs"));
    }
}
