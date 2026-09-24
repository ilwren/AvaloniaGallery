using Avalonia.Controls;

namespace AvaloniaGallery.Models;

/// <summary>
/// A single runnable example shown inside a <see cref="ControlPage"/>.
/// <para>
/// <see cref="Xaml"/> is the one and only source of truth: it is parsed at runtime to
/// produce the live control *and* shown verbatim in the "Source" pane, so the snippet a
/// user copies is always exactly what they just saw running.
/// </para>
/// </summary>
public sealed class ControlSample
{
    public required string Title { get; init; }

    /// <summary>Short explanation of what the sample demonstrates.</summary>
    public string? Description { get; init; }

    /// <summary>The XAML that is both rendered and displayed.</summary>
    public required string Xaml { get; init; }

    /// <summary>
    /// Hand written C# for the "C#" tab, used when the sample wants to show something the
    /// markup cannot express (event wiring, commands, the surrounding setup).
    /// <para>
    /// Leave it null and <see cref="CSharpSource"/> falls back to a projection of
    /// <see cref="Xaml"/>, so every sample has a C# tab without a second copy to maintain.
    /// </para>
    /// </summary>
    public string? CSharp { get; init; }

    /// <summary>
    /// What the "C#" tab actually shows: the hand written snippet when there is one, and
    /// otherwise the projection generated from this sample's own XAML.
    /// </summary>
    public string? CSharpSource => CSharp ?? Services.CSharpProjector.Project(Xaml);

    /// <summary>
    /// Optional data context factory. Samples that bind to a collection use this so the
    /// markup can stay idiomatic (<c>ItemsSource="{Binding Items}"</c>).
    /// </summary>
    public Func<object?>? DataContextFactory { get; init; }

    /// <summary>When true the example is hosted in a fixed-height box (maps, canvases…).</summary>
    public double? PreviewHeight { get; init; }

    /// <summary>
    /// Runs once the parsed control is attached to a window, and is disposed when it leaves.
    /// <para>
    /// A few controls only do anything when something outside the snippet is configured —
    /// <see cref="NativeMenuBar"/>, for instance, renders the menu attached to the
    /// <see cref="TopLevel"/> rather than one declared inside itself. Rather than showing a
    /// dead control, those samples use this hook to wire up the surrounding state for real
    /// and undo it on the way out.
    /// </para>
    /// </summary>
    public Func<Control, IDisposable?>? Attach { get; init; }

    /// <summary>
    /// Builds the preview in code instead of parsing <see cref="Xaml"/>.
    /// <para>
    /// Reserved for controls that cannot live in the visual tree at all — a
    /// <see cref="TrayIcon"/> belongs to the <see cref="Avalonia.Application"/>, not to a
    /// page. Those samples still show real, declarative XAML in the source pane (the markup
    /// you would actually write in App.axaml); this factory supplies the interactive harness
    /// that drives it, which is something markup alone cannot express.
    /// </para>
    /// </summary>
    public Func<Control>? LiveContentFactory { get; init; }
}
