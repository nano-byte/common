// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// A task that executes an <see cref="Action"/>. Only completion is reported, no intermediate progress.
/// </summary>
public sealed class ActionTask : TaskBase
{
    /// <inheritdoc/>
    public override string Name { get; }

    /// <summary>The code to be executed by the task. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</summary>
    private readonly Action _work;

    /// <summary>An optional callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</summary>
    private readonly Action? _cancellationCallback;

    /// <inheritdoc/>
    public override bool CanCancel => _cancellationCallback != null;

    /// <inheritdoc/>
    protected override bool UnitsByte => false;

    /// <summary>
    /// Creates a new that executes an <see cref="Action"/>.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="work">The code to be executed by the task. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
    /// <param name="cancellationCallback">An optional callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</param>
    public ActionTask([Localizable(true)] string name, Action work, Action? cancellationCallback = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _work = work ?? throw new ArgumentNullException(nameof(work));
        _cancellationCallback = cancellationCallback;
    }

    /// <inheritdoc/>
    protected override void Execute()
    {
        if (_cancellationCallback == null) _work();
        else
        {
            using (CancellationToken.Register(_cancellationCallback))
                _work();
        }
    }
}
