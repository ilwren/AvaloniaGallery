using System.Reflection;

namespace AvaloniaGallery.Services;

/// <summary>
/// Serves the gallery's own C# for a given page.
/// <para>
/// The <c>*Samples.cs</c> files are embedded into the assembly at build time (see the
/// <c>EmbeddedResource</c> item in the csproj), so this works identically on desktop and
/// in the browser, where there is no filesystem to read from. Each page's source is
/// extracted by locating its factory method and slicing out the balanced brace block.
/// </para>
/// </summary>
public static class PageSourceProvider
{
    private static readonly Dictionary<string, string> Cache = new(StringComparer.Ordinal);
    private static readonly object Gate = new();

    /// <summary>
    /// Returns the C# that declares <paramref name="pageName"/>'s page, or null when the
    /// source could not be located.
    /// </summary>
    public static string? GetPageSource(string pageName, string categoryFile)
    {
        var key = categoryFile + "::" + pageName;

        lock (Gate)
        {
            if (Cache.TryGetValue(key, out var cached))
                return cached;

            var file = ReadEmbedded(categoryFile);
            if (file is null)
                return null;

            var extracted = Extract(file, pageName);
            if (extracted is not null)
                Cache[key] = extracted;

            return extracted;
        }
    }

    /// <summary>Reads one embedded <c>*Samples.cs</c> file.</summary>
    private static string? ReadEmbedded(string fileName)
    {
        var assembly = typeof(PageSourceProvider).Assembly;

        // LogicalName in the csproj is "src/<file>.cs".
        var name = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("/" + fileName, StringComparison.OrdinalIgnoreCase)
                              || n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));

        if (name is null)
            return null;

        using var stream = assembly.GetManifestResourceStream(name);
        if (stream is null)
            return null;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Slices out <c>private static ControlPage XxxPage() => new ControlPage { … };</c>
    /// by brace matching, which survives the nested braces of collection initialisers and
    /// raw string literals far better than a regex would.
    /// </summary>
    private static string? Extract(string source, string pageName)
    {
        // Find the factory whose body declares Name = "<pageName>".
        var marker = "Name = \"" + pageName + "\"";
        var markerIndex = source.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0)
            return null;

        // Walk back to the start of the enclosing method declaration.
        var methodStart = source.LastIndexOf("private static ControlPage", markerIndex, StringComparison.Ordinal);
        if (methodStart < 0)
            return null;

        // Include the XML doc comment directly above the method when there is one.
        var docStart = methodStart;
        var lineStart = source.LastIndexOf('\n', methodStart == 0 ? 0 : methodStart - 1);
        while (lineStart > 0)
        {
            var prevLineStart = source.LastIndexOf('\n', lineStart - 1);
            var line = source.Substring(prevLineStart + 1, lineStart - prevLineStart - 1).Trim();

            if (!line.StartsWith("///", StringComparison.Ordinal))
                break;

            docStart = prevLineStart + 1;
            lineStart = prevLineStart;
        }

        // Scan forward for the statement terminating semicolon at brace depth zero,
        // skipping anything inside a string so that braces in XAML do not confuse us.
        var depth = 0;
        var i = markerIndex;
        var inRawString = false;
        var inString = false;

        for (; i < source.Length; i++)
        {
            var c = source[i];

            if (inRawString)
            {
                if (c == '"' && i + 2 < source.Length && source[i + 1] == '"' && source[i + 2] == '"')
                {
                    inRawString = false;
                    i += 2;
                }
                continue;
            }

            if (inString)
            {
                if (c == '\\') { i++; continue; }
                if (c == '"') inString = false;
                continue;
            }

            if (c == '"')
            {
                if (i + 2 < source.Length && source[i + 1] == '"' && source[i + 2] == '"')
                {
                    inRawString = true;
                    i += 2;
                }
                else
                {
                    inString = true;
                }
                continue;
            }

            if (c == '{') depth++;
            else if (c == '}') depth--;
            else if (c == ';' && depth <= 0) break;
        }

        if (i >= source.Length)
            return null;

        var block = source.Substring(docStart, i - docStart + 1);
        return Dedent(block);
    }

    /// <summary>Strips the common leading indentation so the snippet reads flush left.</summary>
    private static string Dedent(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n');

        var indent = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Length - l.TrimStart(' ').Length)
            .DefaultIfEmpty(0)
            .Min();

        if (indent == 0)
            return string.Join("\n", lines).Trim('\n');

        return string.Join("\n", lines.Select(l => l.Length >= indent ? l[indent..] : l.TrimStart(' ')))
            .Trim('\n');
    }
}
