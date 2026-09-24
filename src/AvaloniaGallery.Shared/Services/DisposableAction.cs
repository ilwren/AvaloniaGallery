namespace AvaloniaGallery.Services;

/// <summary>
/// Runs an action on dispose.
/// <para>
/// Avalonia has its own <c>Disposable.Create</c>, but it is internal to the framework, and
/// pulling in System.Reactive for one delegate would be a heavy dependency for a gallery.
/// </para>
/// </summary>
public sealed class DisposableAction : IDisposable
{
    private Action? _action;

    private DisposableAction(Action action) => _action = action;

    public static IDisposable Create(Action action) => new DisposableAction(action);

    public void Dispose()
    {
        // Interlocked so a double dispose cannot run the action twice.
        var action = Interlocked.Exchange(ref _action, null);
        action?.Invoke();
    }
}
