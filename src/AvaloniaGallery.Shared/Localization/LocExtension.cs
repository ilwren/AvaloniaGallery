using System.Linq;
using Avalonia.Data;

namespace AvaloniaGallery.Localization;

/// <summary>
/// Markup extension that yields a live translated string: <c>{loc:Loc App.Title}</c>.
/// <para>
/// Returning an observable rather than a plain string is what lets the language picker
/// work without rebuilding any views — every binding created from this extension pushes a
/// new value when <see cref="Loc.LanguageChanged"/> fires.
/// </para>
/// </summary>
public sealed class LocExtension : Avalonia.Markup.Xaml.MarkupExtension
{
    public LocExtension()
    {
    }

    public LocExtension(string key) => Key = key;

    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new Binding { Source = LocSource.For(Key), Path = nameof(LocSource.Value) };
}

/// <summary>
/// Backing object for one localized binding. It re-reads the table and raises
/// <see cref="PropertyChanged"/> whenever the language changes.
/// <para>
/// Instances are shared per key by <see cref="LocSource.For"/>. Avalonia holds a binding's
/// source weakly, so a per-label source would be collected while its label is still on
/// screen and the text would freeze in the old language. Sharing gives every binding a
/// rooted source, and the number of objects is bounded by the number of distinct keys
/// rather than by the number of labels ever created.
/// </para>
/// </summary>
public sealed class LocSource : System.ComponentModel.INotifyPropertyChanged
{
    private static readonly Dictionary<string, LocSource> Cache = new(StringComparer.Ordinal);
    private static readonly object CacheGate = new();

    private readonly string _key;

    // Private on purpose: a directly constructed source would not be rooted anywhere, and
    // Avalonia's weak binding reference would let it be collected while its label lives on.
    private LocSource(string key) => _key = key;

    /// <summary>Returns the shared source for <paramref name="key"/>, creating it once.</summary>
    public static LocSource For(string key)
    {
        lock (CacheGate)
        {
            if (!Cache.TryGetValue(key, out var source))
            {
                source = new LocSource(key);
                Cache[key] = source;
            }

            return source;
        }
    }

    public string Value => Loc.Get(_key);

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    static LocSource() => Loc.LanguageChanged += RefreshAll;

    private static void RefreshAll()
    {
        LocSource[] sources;
        lock (CacheGate)
            sources = Cache.Values.ToArray();

        foreach (var source in sources)
            source.Refresh();
    }

    internal void Refresh() =>
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Value)));
}
