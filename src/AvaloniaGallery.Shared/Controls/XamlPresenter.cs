using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using System.Text;

namespace AvaloniaGallery.Controls;

/// <summary>
/// Minimal, dependency-free syntax highlighter for the source panes.
/// <para>
/// A full tokenizer would be overkill here; this walks the text once and colours tags,
/// attributes, strings, comments and (for C#) keywords. It degrades gracefully: anything
/// it does not recognise is emitted as plain text.
/// </para>
/// </summary>
public sealed class XamlPresenter : TextBlock
{
    public static readonly StyledProperty<string?> SourceProperty =
        AvaloniaProperty.Register<XamlPresenter, string?>(nameof(Source));

    /// <summary>"xaml" or "csharp".</summary>
    public static readonly StyledProperty<string> LanguageProperty =
        AvaloniaProperty.Register<XamlPresenter, string>(nameof(Language), "xaml");

    public string? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    // Colours tuned to read well on both the light and dark code surfaces.
    private static readonly IBrush TagBrush = new SolidColorBrush(Color.Parse("#569CD6"));
    private static readonly IBrush AttrBrush = new SolidColorBrush(Color.Parse("#9CDCFE"));
    private static readonly IBrush ValueBrush = new SolidColorBrush(Color.Parse("#CE9178"));
    private static readonly IBrush CommentBrush = new SolidColorBrush(Color.Parse("#6A9955"));
    private static readonly IBrush KeywordBrush = new SolidColorBrush(Color.Parse("#569CD6"));
    private static readonly IBrush TypeBrush = new SolidColorBrush(Color.Parse("#4EC9B0"));
    private static readonly IBrush PlainBrush = new SolidColorBrush(Color.Parse("#D4D4D4"));

    private static readonly HashSet<string> CsKeywords = new(StringComparer.Ordinal)
    {
        "abstract","as","base","bool","break","byte","case","catch","char","class","const",
        "continue","decimal","default","delegate","do","double","else","enum","event","explicit",
        "extern","false","finally","fixed","float","for","foreach","get","goto","if","implicit",
        "in","int","interface","internal","is","lock","long","namespace","new","null","object",
        "operator","out","override","params","private","protected","public","readonly","ref",
        "return","sealed","set","short","sizeof","stackalloc","static","string","struct","switch",
        "this","throw","true","try","typeof","uint","ulong","unchecked","unsafe","ushort","using",
        "var","virtual","void","volatile","while","record","init","required","nameof","async","await",
    };

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty || change.Property == LanguageProperty)
            Render();
    }

    private void Render()
    {
        Inlines?.Clear();
        var text = Source;
        if (string.IsNullOrEmpty(text))
            return;

        var inlines = Inlines ??= new InlineCollection();

        if (string.Equals(Language, "csharp", StringComparison.OrdinalIgnoreCase))
            HighlightCSharp(text, inlines);
        else
            HighlightXaml(text, inlines);
    }

    private static void Add(InlineCollection target, string text, IBrush brush)
    {
        if (text.Length > 0)
            target.Add(new Run(text) { Foreground = brush });
    }

    private static void HighlightXaml(string text, InlineCollection output)
    {
        var i = 0;
        var buffer = new StringBuilder();

        void FlushPlain()
        {
            Add(output, buffer.ToString(), PlainBrush);
            buffer.Clear();
        }

        while (i < text.Length)
        {
            // Comment
            if (text.AsSpan(i).StartsWith("<!--"))
            {
                FlushPlain();
                var end = text.IndexOf("-->", i, StringComparison.Ordinal);
                if (end < 0) end = text.Length; else end += 3;
                Add(output, text[i..end], CommentBrush);
                i = end;
                continue;
            }

            if (text[i] == '<')
            {
                FlushPlain();

                // Tag opener including optional '/'
                var j = i + 1;
                if (j < text.Length && text[j] == '/') j++;
                while (j < text.Length && (char.IsLetterOrDigit(text[j]) || text[j] is ':' or '.' or '_')) j++;
                Add(output, text[i..j], TagBrush);
                i = j;

                // Attributes until the tag closes
                while (i < text.Length && text[i] != '>')
                {
                    if (text[i] == '"')
                    {
                        var close = text.IndexOf('"', i + 1);
                        if (close < 0) close = text.Length - 1;
                        Add(output, text[i..(close + 1)], ValueBrush);
                        i = close + 1;
                    }
                    else if (char.IsLetter(text[i]))
                    {
                        var k = i;
                        while (k < text.Length && (char.IsLetterOrDigit(text[k]) || text[k] is ':' or '.' or '_')) k++;
                        Add(output, text[i..k], AttrBrush);
                        i = k;
                    }
                    else
                    {
                        Add(output, text[i].ToString(), PlainBrush);
                        i++;
                    }
                }

                if (i < text.Length)
                {
                    Add(output, ">", TagBrush);
                    i++;
                }
                continue;
            }

            buffer.Append(text[i]);
            i++;
        }

        FlushPlain();
    }

    private static void HighlightCSharp(string text, InlineCollection output)
    {
        var i = 0;
        while (i < text.Length)
        {
            // Line comment
            if (i + 1 < text.Length && text[i] == '/' && text[i + 1] == '/')
            {
                var end = text.IndexOf('\n', i);
                if (end < 0) end = text.Length;
                Add(output, text[i..end], CommentBrush);
                i = end;
                continue;
            }

            // String literal
            if (text[i] == '"')
            {
                var k = i + 1;
                while (k < text.Length && (text[k] != '"' || text[k - 1] == '\\')) k++;
                if (k < text.Length) k++;
                Add(output, text[i..k], ValueBrush);
                i = k;
                continue;
            }

            if (char.IsLetter(text[i]) || text[i] == '_')
            {
                var k = i;
                while (k < text.Length && (char.IsLetterOrDigit(text[k]) || text[k] == '_')) k++;
                var word = text[i..k];

                var brush = CsKeywords.Contains(word) ? KeywordBrush
                    : char.IsUpper(word[0]) ? TypeBrush
                    : PlainBrush;

                Add(output, word, brush);
                i = k;
                continue;
            }

            Add(output, text[i].ToString(), PlainBrush);
            i++;
        }
    }
}
