// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.Threading;

/// <summary>
/// Waits for a <see cref="WaitHandle"/> to become available or the <see cref="CancellationToken"/> to be triggered.
/// </summary>
public sealed class WaitTask : TaskBase
{
    /// <summary>The <see cref="WaitHandle"/> to wait for; <c>null</c> to wait for <see cref="CancellationToken"/>.</summary>
    private readonly WaitHandle? _waitHandle;

    /// <summary>The number of milliseconds to wait before raising <see cref="TimeoutException"/>; <see cref="Timeout.Infinite"/> to wait indefinitely</summary>
    private readonly int _millisecondsTimeout;

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    protected override bool PreventIdle => false;

    /// <inheritdoc/>
    protected override bool UnitsByte => false;

    /// <summary>
    /// Creates a new waiting task.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="waitHandle">The <see cref="WaitHandle"/> to wait for; <c>null</c> to wait for <see cref="CancellationToken"/>.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait before raising <see cref="TimeoutException"/>; <see cref="Timeout.Infinite"/> to wait indefinitely.</param>
    public WaitTask([Localizable(true)] string name, WaitHandle? waitHandle = null, int millisecondsTimeout = Timeout.Infinite)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _waitHandle = waitHandle;
        _millisecondsTimeout = millisecondsTimeout;
    }

    /// <inheritdoc/>
    protected override void Execute()
    {
        if (_waitHandle == null) CancellationToken.WaitHandle.WaitOne(_millisecondsTimeout);
        else _waitHandle.WaitOne(CancellationToken, _millisecondsTimeout);
    }
}
