using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaGallery.Models;
using AvaloniaGallery.Samples;
using Xunit;

namespace AvaloniaGallery.Tests;

/// <summary>
/// The samples are XAML strings parsed at runtime, so the C# compiler cannot check them.
/// These tests are what stands in for that: every snippet in the registry is parsed,
/// mounted in a window and laid out, which catches unknown types, bad property names,
/// broken bindings and templates that throw during measure.
/// </summary>
public class SampleIntegrityTests
{
    public static TheoryData<string, string> AllSamples()
    {
        var data = new TheoryData<string, string>();

        foreach (var page in SampleRegistry.AllPages)
            foreach (var sample in page.Samples)
                data.Add(page.Name, sample.Title);

        return data;
    }

    private static ControlSample Resolve(string pageName, string sampleTitle)
    {
        var page = SampleRegistry.FindByName(pageName);
        Assert.NotNull(page);

        var sample = page!.Samples.Single(s => s.Title == sampleTitle);
        return sample;
    }

    /// <summary>Every sample must parse, render and complete a layout pass without throwing.</summary>
    [AvaloniaTheory]
    [MemberData(nameof(AllSamples))]
    public void Sample_renders(string pageName, string sampleTitle)
    {
        var sample = Resolve(pageName, sampleTitle);

        // A handful of samples document something that is not a control at all — TrayIcon
        // is declared on the Application, not in the visual tree. Those supply the preview
        // through a factory, and their Xaml is reference markup rather than the thing being
        // mounted, so the live content is what gets exercised here.
        var control = sample.LiveContentFactory is { } factory
            ? factory()
            : Assert.IsAssignableFrom<Control>(AvaloniaRuntimeXamlLoader.Parse(sample.Xaml));

        if (sample.DataContextFactory?.Invoke() is { } dc)
            control.DataContext = dc;

        var window = new Window
        {
            Content = control,
            Width = 900,
            Height = 700,
        };

        window.Show();

        // Force a full measure/arrange/render cycle: this is where a bad template or a
        // binding to a missing property actually blows up.
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        Assert.True(control.IsMeasureValid, $"{pageName} / {sampleTitle} never completed measure.");
        Assert.True(control.Bounds.Width > 0 || control.Bounds.Height > 0,
            $"{pageName} / {sampleTitle} laid out to a zero size.");

        // The attach hook is what makes samples like NativeMenuBar show anything, so run it
        // against a real window and make sure it tears down cleanly.
        if (sample.Attach is { } attach)
        {
            var subscription = attach(control);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            subscription?.Dispose();
        }

        window.Close();
    }

    /// <summary>
    /// Reference markup still has to be valid XML even when it is not mounted, otherwise the
    /// source pane would be showing something that could never compile.
    /// </summary>
    [AvaloniaTheory]
    [MemberData(nameof(AllSamples))]
    public void Sample_xaml_is_well_formed(string pageName, string sampleTitle)
    {
        var sample = Resolve(pageName, sampleTitle);

        var exception = Record.Exception(() => System.Xml.Linq.XDocument.Parse(sample.Xaml));

        Assert.True(exception is null,
            $"{pageName} / {sampleTitle} is not well formed XML: {exception?.Message}");
    }
}
