// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using NanoByte.Common.Collections;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Used to execute and track <see cref="ITask"/>s and ask the user questions. Specific implementations provide different kinds of user interfaces.
    /// </summary>
    /// <remarks>
    /// The methods may be called from a background thread. Implementations need to apply appropriate thread-synchronization to update UI elements.
    /// Implementations should derive from <see cref="MarshalNoTimeout"/>.
    /// </remarks>
    public interface ITaskHandler : IDisposable
    {
        /// <summary>
        /// Used to signal when the user wishes to cancel the entire current process (and any <see cref="ITask"/>s it includes).
        /// </summary>
        /// <remarks>Once this has been signalled this <see cref="ITaskHandler"/> cannot be reused, since any subsequently started <see cref="ITask"/>s will be cancelled immediately.</remarks>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Used to ask the user or a keyring for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s; can be <c>null</c>.
        /// </summary>
        ICredentialProvider? CredentialProvider { get; }

        /// <summary>
        /// Runs an <see cref="ITask"/> and tracks its progress. Returns once the task has been completed. The task may be executed on a different thread.
        /// </summary>
        /// <param name="task">The task to be run. (<see cref="ITask.Run"/> or equivalent is called on it.)</param>
        /// <exception cref="OperationCanceledException">The user canceled the task.</exception>
        /// <exception cref="IOException">The task ended with <see cref="TaskState.IOError"/>.</exception>
        /// <exception cref="WebException">The task ended with <see cref="TaskState.WebError"/>.</exception>
        /// <remarks>
        /// This may be called multiple times concurrently but concurrent calls must not depend on each other.
        /// The specific implementation of this method determines whether the tasks actually run concurrently or in sequence.
        /// </remarks>
        void RunTask(ITask task);

        /// <summary>
        /// The detail level of messages displayed to the user.
        /// </summary>
        Verbosity Verbosity { get; set; }

        /// <summary>
        /// Asks the user a Yes/No/Cancel question.
        /// </summary>
        /// <param name="question">The question and comprehensive information to help the user make an informed decision.</param>
        /// <param name="defaultAnswer">The default answer to preselect. May be chosen automatically if the user cannot be asked. <c>null</c> if the user must make the choice themselves.</param>
        /// <param name="alternateMessage">A message to output with <see cref="Log.Warn(string)"/> when the <paramref name="defaultAnswer"/> is used instead of asking the user.</param>
        /// <returns><c>true</c> if the user answered with 'Yes'; <c>false</c> if the user answered with 'No'.</returns>
        /// <exception cref="OperationCanceledException">Throw if the user answered with 'Cancel'.</exception>
        bool Ask([Localizable(true)] string question, bool? defaultAnswer = null, [Localizable(true)] string? alternateMessage = null);

        /// <summary>
        /// Displays multi-line text to the user.
        /// </summary>
        /// <param name="title">A title for the message.</param>
        /// <param name="message">The string to display. Trailing linebreaks are appropriately handled or ignored.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        void Output([Localizable(true)] string title, [Localizable(true)] string message);

        /// <summary>
        /// Displays tabular data to the user.
        /// </summary>
        /// <param name="title">A title for the data.</param>
        /// <param name="data">The data to display.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        void Output<T>([Localizable(true)] string title, IEnumerable<T> data);

        /// <summary>
        /// Displays tree-like data to the user.
        /// </summary>
        /// <param name="title">A title for the data.INamed</param>
        /// <param name="data">The data to display.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        void Output<T>([Localizable(true)] string title, NamedCollection<T> data)
            where T : INamed;

        /// <summary>
        /// Displays an error message to the user.
        /// </summary>
        /// <param name="exception">The exception representing the error that occurred.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")]
        void Error(Exception exception);
    }
}
