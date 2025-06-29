// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

#if !NET20 && !NET40
using System.Threading.Tasks;
using NanoByte.Common.Threading;
#endif

namespace NanoByte.Common.Tasks;

/// <summary>
/// A task that executes an <see cref="Action"/> that can be canceled. Only completion is reported, no intermediate progress.
/// </summary>
/// <param name="name">A name describing the task in human-readable form.</param>
/// <param name="work">The code to be executed by the task. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
public sealed class ActionTask([Localizable(true)] string name, Action<CancellationToken> work) : TaskBase
{
    /// <summary>
    /// A task that executes an <see cref="Action"/> that cannot be canceled. Only completion is reported, no intermediate progress.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="work">The code to be executed by the task. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
    public ActionTask([Localizable(true)] string name, Action work)
        : this(name, _ => work())
    {
        CanCancel = false;
    }

#if !NET20 && !NET40
    /// <summary>
    /// A task that executes an async <see cref="Task"/> that can be canceled. Only completion is reported, no intermediate progress.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="work">The code to be executed and awaited. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
    public ActionTask([Localizable(true)] string name, Func<CancellationToken, Task> work)
        : this(name, cancellationToken => ThreadUtils.RunTask(() => work(cancellationToken)))
    {}

    /// <summary>
    /// A task that executes an async <see cref="Task"/> that cannot be canceled. Only completion is reported, no intermediate progress.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="work">The code to be executed and awaited. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
    public ActionTask([Localizable(true)] string name, Func<Task> work)
        : this(name, _ => work())
    {
        CanCancel = false;
    }
#endif

    /// <inheritdoc/>
    public override string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <inheritdoc/>
    public override bool CanCancel { get; } = true;

    /// <inheritdoc/>
    protected override bool UnitsByte => false;

    /// <inheritdoc/>
    protected override void Execute() => work(CancellationToken);
}
