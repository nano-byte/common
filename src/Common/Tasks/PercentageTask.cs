// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// A callback to be called by a workload to report its progress in percent.
/// </summary>
/// <param name="percent">The workload's progress in percent.</param>
[CLSCompliant(false)]
public delegate void PercentProgressCallback(int percent);

/// <summary>
/// A task that executes an <see cref="Action{T}"/> and reports progress in percent.
/// </summary>
/// <param name="name">A name describing the task in human-readable form.</param>
/// <param name="work">The code to be executed by the task. Is given a callback to report progress in percent. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
/// <param name="cancellationCallback">An optional callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</param>
[CLSCompliant(false)]
public sealed class PercentageTask([Localizable(true)] string name, Action<PercentProgressCallback> work, Action? cancellationCallback = null) : TaskBase
{
    /// <inheritdoc/>
    public override string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <inheritdoc/>
    public override bool CanCancel => cancellationCallback != null;

    /// <inheritdoc/>
    protected override bool UnitsByte => false;

    /// <inheritdoc/>
    protected override void Execute()
    {
        CancellationTokenRegistration? RegisterCancellationCallBack()
            => cancellationCallback?.To(CancellationToken.Register);

        UnitsTotal = 100;
        State = TaskState.Data;

        using (RegisterCancellationCallBack())
            work(percent => UnitsProcessed = percent);
    }
}
