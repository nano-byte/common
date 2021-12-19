// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Represents an operation that can be cancelled and have its progress tracked.
/// </summary>
/// <seealso cref="ITaskHandler"/>
public interface ITask
{
    /// <summary>
    /// Runs the task and blocks until it is complete.
    /// </summary>
    /// <param name="cancellationToken">Used to receive a signal (e.g. from another thread) when the user wishes to cancel the task.</param>
    /// <param name="credentialProvider">Object used to retrieve credentials for specific <see cref="Uri"/>s on demand; can be <c>null</c>.</param>
    /// <param name="progress">Used to report back the task's progress (e.g. to another thread).</param>
    /// <exception cref="OperationCanceledException">The task was canceled from another thread.</exception>
    /// <exception cref="IOException">The task ended with <see cref="TaskState.IOError"/>.</exception>
    /// <exception cref="WebException">The task ended with <see cref="TaskState.WebError"/>.</exception>
    /// <seealso cref="ITaskHandler.RunTask"/>
    void Run(CancellationToken cancellationToken = default, ICredentialProvider? credentialProvider = null, IProgress<TaskSnapshot>? progress = null);

    /// <summary>
    /// A name describing the task in human-readable form.
    /// </summary>
    [Description("A name describing the task in human-readable form.")]
    [Localizable(true)]
    string Name { get; }

    /// <summary>
    /// An object used to associate the task with a specific process; can be <c>null</c>.
    /// </summary>
    object? Tag { get; set; }

    /// <summary>
    /// Indicates whether this task can be canceled once it has been started.
    /// </summary>
    [Description("Indicates whether this task can be canceled once it has been started.")]
    bool CanCancel { get; }
}