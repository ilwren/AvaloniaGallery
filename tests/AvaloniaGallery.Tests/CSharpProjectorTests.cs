using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AvaloniaGallery.Models;
using AvaloniaGallery.Samples;
using AvaloniaGallery.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace AvaloniaGallery.Tests;

/// <summary>
/// The C# tab is generated from the XAML tab rather than hand written, which keeps the two
/// from drifting apart. That only helps if the generated code actually compiles, so every
/// projection is fed through Roslyn here. A sample that produces code a reader could not
/// paste into their own project is a bug in the projector, and this is where it surfaces.
/// </summary>
public class CSharpProjectorTests
{
    /// <summary>
    /// Only the System namespaces are supplied. Everything Avalonia-specific has to come
    /// from the using directives the projector emits, which is what proves a snippet is
    /// self-contained.
    /// </summary>
    private const string AmbientUsings = """
        using System;
        using System.Linq;
        using System.Collections.Generic;
        """;

    private static readonly Lazy<IReadOnlyList<MetadataReference>> References =
        new(BuildReferences);

    private static IReadOnlyList<MetadataReference> BuildReferences()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var references = new List<MetadataReference>();

        // Everything already loaded covers the Avalonia assemblies the gallery uses.
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location))
                continue;

            if (seen.Add(assembly.Location))
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
        }

        // The framework reference assemblies are not necessarily loaded yet.
        var trusted = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty;
        foreach (var path in trusted.Split(Path.PathSeparator))
        {
            if (path.Length > 0 && File.Exists(path) && seen.Add(path))
                references.Add(MetadataReference.CreateFromFile(path));
        }

        return references;
    }

    public static TheoryData<string, string> ProjectedSamples()
    {
        var data = new TheoryData<string, string>();

        foreach (var page in SampleRegistry.AllPages)
        {
            foreach (var sample in page.Samples)
            {
                // Hand written C# is the author's own code, not the projector's output.
                if (sample.CSharp is null)
                    data.Add(page.Name, sample.Title);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(ProjectedSamples))]
    public void Projected_csharp_compiles(string pageName, string sampleTitle)
    {
        var sample = FindSample(pageName, sampleTitle);
        var code = sample.CSharpSource;

        Assert.False(string.IsNullOrWhiteSpace(code),
            $"{pageName} / {sampleTitle} produced no C# projection.");

        var diagnostics = Compile(code!, pageName + sampleTitle);

        Assert.True(diagnostics.Count == 0,
            $"{pageName} / {sampleTitle} produced C# that does not compile:\n" +
            string.Join("\n", diagnostics.Select(d => "  " + d.Id + ": " + d.GetMessage())) +
            "\n--- generated code ---\n" + code);
    }

    /// <summary>
    /// Every sample must offer a C# tab: an empty one would leave the reader with nothing
    /// where the gallery promises an equivalent.
    /// </summary>
    [Fact]
    public void Every_sample_has_csharp_source()
    {
        var missing = SampleRegistry.AllPages
            .SelectMany(page => page.Samples.Select(sample => (page, sample)))
            .Where(pair => string.IsNullOrWhiteSpace(pair.sample.CSharpSource))
            .Select(pair => pair.page.Name + " / " + pair.sample.Title)
            .ToList();

        Assert.True(missing.Count == 0,
            "These samples have no C# equivalent:\n" + string.Join("\n", missing));
    }

    /// <summary>Hand written C# takes priority over the generated projection.</summary>
    [Fact]
    public void Hand_written_csharp_wins_over_projection()
    {
        var sample = new ControlSample
        {
            Title = "Test",
            Xaml = "<Button Content=\"Hi\" />",
            CSharp = "// hand written",
        };

        Assert.Equal("// hand written", sample.CSharpSource);
    }

    /// <summary>The projection is derived from the markup, not a stored copy of it.</summary>
    [Fact]
    public void Projection_follows_the_xaml()
    {
        var sample = new ControlSample
        {
            Title = "Test",
            Xaml = "<Button Content=\"Click me\" IsEnabled=\"False\" />",
        };

        var projected = sample.CSharpSource;

        Assert.NotNull(projected);
        Assert.Contains("new Button", projected);
        Assert.Contains("Content = \"Click me\"", projected);

        // Booleans must be C# literals, not the XAML spelling.
        Assert.Contains("IsEnabled = false", projected);
    }

    /// <summary>Enum values project to real members rather than quoted strings.</summary>
    [Fact]
    public void Enum_values_project_to_enum_members()
    {
        var sample = new ControlSample
        {
            Title = "Test",
            Xaml = "<StackPanel Orientation=\"Horizontal\" />",
        };

        Assert.Contains("Orientation = Orientation.Horizontal", sample.CSharpSource);
    }

    private static ControlSample FindSample(string pageName, string sampleTitle)
    {
        var page = SampleRegistry.AllPages.Single(p => p.Name == pageName);
        return page.Samples.Single(s => s.Title == sampleTitle);
    }

    private static IReadOnlyList<Diagnostic> Compile(string code, string seed)
    {
        // The projector puts its using directives at the top of the snippet; C# requires
        // them before any type declaration, so they are hoisted out of the method body.
        var usings = new List<string>();
        var body = new List<string>();

        foreach (var line in code.Replace("\r\n", "\n").Split('\n'))
        {
            if (line.StartsWith("using ", StringComparison.Ordinal) &&
                line.EndsWith(";", StringComparison.Ordinal))
            {
                usings.Add(line);
            }
            else
            {
                body.Add(line);
            }
        }

        var source =
            AmbientUsings + "\n" +
            string.Join("\n", usings) + "\n" +
            "public class Projected" + Math.Abs(seed.GetHashCode()) +
            " : Avalonia.Controls.UserControl\n{\n    public void Build()\n    {\n" +
            string.Join("\n", body) + "\n" +
            "        this.Content = root;\n    }\n}";

        var compilation = CSharpCompilation.Create(
            "projection_" + Guid.NewGuid().ToString("N"),
            new[] { CSharpSyntaxTree.ParseText(source) },
            References.Value,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();
    }
}
