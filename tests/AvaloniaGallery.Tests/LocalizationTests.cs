using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using AvaloniaGallery.Localization;
using AvaloniaGallery.Samples;
using Xunit;

namespace AvaloniaGallery.Tests;

/// <summary>
/// A half translated UI is worse than an untranslated one, so the tables are checked for
/// completeness rather than trusted.
/// </summary>
public class LocalizationTests
{
    [Fact]
    public void Every_language_translates_every_key()
    {
        var english = Loc.Keys;

        foreach (var language in Loc.SupportedLanguages)
        {
            var table = typeof(Loc)
                .GetMethod("TableFor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object[] { language.Code }) as IReadOnlyDictionary<string, string>;

            Assert.NotNull(table);

            var missing = english.Where(k => !table!.ContainsKey(k)).ToList();

            Assert.True(missing.Count == 0,
                $"{language.EnglishName} is missing: {string.Join(", ", missing)}");
        }
    }

    [Fact]
    public void Switching_language_changes_the_text()
    {
        var original = Loc.Language;
        try
        {
            Loc.Language = Loc.SupportedLanguages.Single(l => l.Code == "en");
            var english = Loc.Get("App.Search");

            Loc.Language = Loc.SupportedLanguages.Single(l => l.Code == "zh-Hans");
            var chinese = Loc.Get("App.Search");

            Assert.NotEqual(english, chinese);
        }
        finally
        {
            Loc.Language = original;
        }
    }

    /// <summary>A live binding must follow the language, not freeze at its first value.</summary>
    [Fact]
    public void LocSource_updates_when_language_changes()
    {
        var original = Loc.Language;
        try
        {
            Loc.Language = Loc.SupportedLanguages.Single(l => l.Code == "en");

            var source = LocSource.For("App.Title");
            var before = source.Value;

            var raised = false;
            source.PropertyChanged += (_, _) => raised = true;

            Loc.Language = Loc.SupportedLanguages.Single(l => l.Code == "zh-Hans");

            Assert.True(raised, "LocSource did not raise PropertyChanged on a language switch.");
            Assert.NotEqual(before, source.Value);
        }
        finally
        {
            Loc.Language = original;
        }
    }

    [Fact]
    public void Every_category_has_a_translation_key()
    {
        foreach (var category in SampleRegistry.Categories)
        {
            Assert.False(string.IsNullOrEmpty(category.NameKey),
                $"Category '{category.Name}' has no NameKey, so it cannot be translated.");

            // Get falls back to the key itself when a string is missing, which would render
            // "Cat.Foo" in the navigation pane.
            Assert.NotEqual(category.NameKey, Loc.Get(category.NameKey!));
        }
    }

    [Fact]
    public void Unknown_keys_fall_back_to_the_key()
    {
        Assert.Equal("Nope.Missing", Loc.Get("Nope.Missing"));
    }

    /// <summary>
    /// Avalonia holds a binding's source with a weak reference. A LocSource created per
    /// label therefore becomes collectable the moment the markup extension returns, and a
    /// collection before the next language switch leaves that label stuck in the old
    /// language — which is exactly what happened to the navigation headers. Sharing one
    /// source per key keeps them rooted; this test fails if that sharing is removed.
    /// </summary>
    [Fact]
    public void Loc_sources_survive_garbage_collection()
    {
        var original = Loc.Language;

        try
        {
            Loc.Language = Loc.SupportedLanguages.First(l => l.Code == "en");

            var source = LocSource.For("Cat.Text");
            var seen = 0;
            source.PropertyChanged += OnChanged;

            void OnChanged(object? sender, PropertyChangedEventArgs e) => seen++;

            // Nothing but the shared cache is keeping the source alive at this point.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Loc.Language = Loc.SupportedLanguages.First(l => l.Code == "zh-Hans");

            Assert.True(seen > 0,
                "LocSource stopped raising PropertyChanged after a collection, so bound " +
                "labels would keep showing the previous language.");
            Assert.Equal("\u6587\u672c", source.Value);

            source.PropertyChanged -= OnChanged;
        }
        finally
        {
            Loc.Language = original;
        }
    }

    /// <summary>The same key must hand back the same rooted instance.</summary>
    [Fact]
    public void Loc_sources_are_shared_per_key()
    {
        Assert.Same(LocSource.For("Cat.Text"), LocSource.For("Cat.Text"));
        Assert.NotSame(LocSource.For("Cat.Text"), LocSource.For("Cat.Layout"));
    }

    /// <summary>
    /// The thread culture must agree with the selected language at all times.
    /// <para>
    /// A field initializer picked the default language without running the property setter,
    /// so the culture was left at whatever the OS supplied. On a Chinese Windows the gallery
    /// then reported English while CurrentCulture was still zh-CN, and every culture-driven
    /// control — Calendar, DatePicker, TimePicker — rendered Chinese names under an English
    /// UI. Applying the culture from a static constructor closes that gap.
    /// </para>
    /// </summary>
    [Fact]
    public void Culture_always_matches_the_selected_language()
    {
        var expected = Loc.Language.Code.Split('-')[0];

        Assert.Equal(expected, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        Assert.Equal(expected, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

        var original = Loc.Language;
        try
        {
            Loc.Language = Loc.SupportedLanguages.Single(l => l.Code == "zh-Hans");
            Assert.Equal("zh", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        }
        finally
        {
            Loc.Language = original;
        }

        Assert.Equal(expected, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
    }
}
