// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Abstract base class for <see cref="ITask"/> implementations.
/// </summary>
[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposing WaitHandle is not necessary since the SafeWaitHandle is never touched")]
public abstract class TaskBase : MarshalByRefObject, ITask
{
    /// <inheritdoc/>
    public abstract string Name { get; }

    /// <inheritdoc/>
    public object? Tag { get; set; }

    /// <inheritdoc/>
    public virtual bool CanCancel => true;

    /// <summary>
    /// Indicates whether this task should prevent the system from entering idle mode.
    /// </summary>
    protected virtual bool PreventIdle => true;

    /// <summary>Signaled when the user wants to cancel the task execution.</summary>
    protected CancellationToken CancellationToken;

    /// <summary>Used to report back the task's progress.</summary>
    private IProgress<TaskSnapshot>? _progress;

    /// <summary>Used to retrieve credentials for specific <see cref="Uri"/>s on demand; can be <c>null</c>.</summary>
    protected ICredentialProvider? CredentialProvider;

    /// <inheritdoc/>
    public void Run(CancellationToken cancellationToken = default, ICredentialProvider? credentialProvider = null, IProgress<TaskSnapshot>? progress = null)
    {
        cancellationToken.ThrowIfCancellationRequested();
        CancellationToken = cancellationToken;
        _progress = progress;
        CredentialProvider = credentialProvider;

        State = TaskState.Started;

        try
        {
            using (PreventIdle ? OSUtils.PreventIdle(Name) : null)
                Execute();
        }
        #region Error handling
        catch (OperationCanceledException)
        {
            State = TaskState.Canceled;
            throw;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            State = TaskState.IOError;
            throw;
        }
        catch (WebException)
        {
            State = TaskState.WebError;
            throw;
        }
        #endregion

        State = TaskState.Complete;

        cancellationToken.ThrowIfCancellationRequested();
    }

    #region Progress
    /// <summary>The current State of the task.</summary>
    protected internal TaskState State { get; protected set => value.To(ref field, OnProgressChanged); }

    /// <summary>
    /// <c>true</c> if <see cref="UnitsProcessed"/> and <see cref="UnitsTotal"/> are measured in bytes;
    /// <c>false</c> if they are measured in generic units.
    /// </summary>
    protected abstract bool UnitsByte { get; }

    /// <summary>The number of units that have been processed so far.</summary>
    protected long UnitsProcessed { get; set => value.To(ref field, OnProgressChangedThrottled); }

    private long _unitsTotal = -1;

    /// <summary>The total number of units that are to be processed; -1 for unknown.</summary>
    protected long UnitsTotal { get => _unitsTotal; set => value.To(ref _unitsTotal, OnProgressChanged); }

    /// <summary>
    /// Informs the caller of the current progress, if a callback was registered.
    /// </summary>
    private void OnProgressChanged() => _progress?.Report(new(State, UnitsByte, UnitsProcessed, _unitsTotal));

    private DateTime _lastProgress;
    private static readonly TimeSpan _progressRate = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Informs the caller of the current progress, if a callback was registered. Limits the rate of progress updates.
    /// </summary>
    private void OnProgressChangedThrottled()
    {
        if (_progress == null) return;

        var now = DateTime.Now;
        if ((now - _lastProgress) < _progressRate) return;

        _progress.Report(new(State, UnitsByte, UnitsProcessed, _unitsTotal));
        _lastProgress = now;
    }
    #endregion

    /// <summary>
    /// The actual code to be executed.
    /// </summary>
    /// <remarks>
    /// <see cref="State"/> is automatically set
    /// to <see cref="TaskState.Started"/> before calling this method,
    /// to <see cref="TaskState.Complete"/> after a successful exit and
    /// to an appropriate error state in case on an exception.
    /// You can set additional <see cref="TaskState"/>s during execution.
    /// </remarks>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="IOException">The task ended with <see cref="TaskState.IOError"/>.</exception>
    /// <exception cref="WebException">The task ended with <see cref="TaskState.WebError"/>.</exception>
    protected abstract void Execute();
}
