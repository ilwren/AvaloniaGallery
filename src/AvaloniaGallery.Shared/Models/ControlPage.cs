using AvaloniaGallery.Services;

namespace AvaloniaGallery.Models;

/// <summary>Top level grouping shown in the navigation pane.</summary>
public sealed class ControlCategory
{
    public required string Name { get; init; }

    /// <summary>Resource key used to translate <see cref="Name"/> in the UI.</summary>
    public string? NameKey { get; init; }

    /// <summary>Path geometry used for the category glyph in the nav pane.</summary>
    public required string IconData { get; init; }

    /// <summary>File the pages were declared in, used by the "Page source" viewer.</summary>
    public string SourceFile { get; init; } = string.Empty;

    public List<ControlPage> Pages { get; } = new();
}

/// <summary>
/// Documentation + samples for a single Avalonia control.
/// </summary>
public sealed class ControlPage
{
    public required string Name { get; init; }

    /// <summary>One line summary rendered under the page title.</summary>
    public required string Summary { get; init; }

    /// <summary>Namespace the type lives in, e.g. <c>Avalonia.Controls</c>.</summary>
    public string Namespace { get; init; } = "Avalonia.Controls";

    /// <summary>Assembly / NuGet package that provides the control.</summary>
    public string Assembly { get; init; } = "Avalonia.Controls";

    /// <summary>Set for controls introduced in Avalonia 12.</summary>
    public bool IsNew { get; init; }

    /// <summary>Abstract base classes and presenters are flagged so users know they are infrastructure.</summary>
    public bool IsInfrastructure { get; init; }

    /// <summary>Link to the matching page on docs.avaloniaui.net, when one exists.</summary>
    public string? DocsUrl { get; init; }

    /// <summary>
    /// What the control needs from the host. Pages that cannot run on the current platform
    /// still appear in the navigation, but render an explanatory card in place of the live
    /// preview rather than throwing.
    /// </summary>
    public PlatformRequirement Requires { get; init; } = PlatformRequirement.Any;

    public List<ControlSample> Samples { get; } = new();

    /// <summary>Category is assigned by the registry when the page is added.</summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Source file the page was declared in; set by the registry.</summary>
    public string SourceFile { get; set; } = string.Empty;

    /// <summary>Used by the search box.</summary>
    public string SearchText => $"{Name} {Summary} {CategoryName}";
}
