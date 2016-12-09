/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using JetBrains.Annotations;
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
    [PublicAPI]
    public interface ITaskHandler : IDisposable
    {
        /// <summary>
        /// Used to signal when the user wishes to cancel the entire current process (and any <see cref="ITask"/>s it includes).
        /// </summary>
        /// <remarks>Once this has been signalled this <see cref="ITaskHandler"/> cannot be reused, since any subsequently started <see cref="ITask"/>s will be cancelled immediatley.</remarks>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Used to ask the user or a keyring for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s; can be <c>null</c>.
        /// </summary>
        [CanBeNull]
        ICredentialProvider CredentialProvider { get; }

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
        void RunTask([NotNull] ITask task);

        /// <summary>
        /// The detail level of messages displayed to the user.
        /// </summary>
        Verbosity Verbosity { get; set; }

        /// <summary>
        /// Asks the user a Yes/No/Cancel question.
        /// </summary>
        /// <param name="question">The question and comprehensive information to help the user make an informed decision.</param>
        /// <returns><c>true</c> if the user answered with 'Yes'; <c>false</c> if the user answered with 'No'.</returns>
        /// <exception cref="OperationCanceledException">The user selected 'Cancel'.</exception>
        bool Ask([NotNull, Localizable(true)] string question);

        /// <summary>
        /// Asks the user a Yes/No/Cancel question or uses a default answer in <see cref="Tasks.Verbosity.Batch"/> mode.
        /// </summary>
        /// <param name="question">The question and comprehensive information to help the user make an informed decision.</param>
        /// <param name="defaultAnswer">The answer to automatically use when <see cref="Verbosity"/> is <see cref="Tasks.Verbosity.Batch"/> or lower.</param>
        /// <param name="alternateMessage">A message to output with <see cref="Log.Warn(string)"/> when the <paramref name="defaultAnswer"/> is used instead of asking the user.</param>
        /// <returns><c>true</c> if the user answered with 'Yes'; <c>false</c> if the user answered with 'No'.</returns>
        /// <exception cref="OperationCanceledException">The user selected 'Cancel'.</exception>
        bool Ask([NotNull, Localizable(true)] string question, bool defaultAnswer, [CanBeNull, Localizable(true)] string alternateMessage = null);

        /// <summary>
        /// Displays multi-line text to the user.
        /// </summary>
        /// <param name="title">A title for the message. Will only be displayed in GUIs, not on the console. Must not contain critical information!</param>
        /// <param name="message">The string to display. Trailing linebreaks are appropriately handled or ignored.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        void Output([NotNull, Localizable(true)] string title, [NotNull, Localizable(true)] string message);

        /// <summary>
        /// Displays multi-line text to the user unless <see cref="Verbosity"/> is <see cref="Tasks.Verbosity.Batch"/>.
        /// </summary>
        /// <param name="title">A title for the message. Will only be displayed in GUIs, not on the console. Must not contain critical information!</param>
        /// <param name="message">The string to display.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        void OutputLow([NotNull, Localizable(true)] string title, [NotNull, Localizable(true)] string message);

        /// <summary>
        /// Displays tabular data to the user.
        /// </summary>
        /// <param name="title">A title for the data. Will only be displayed in GUIs, not on the console. Must not contain critical information!</param>
        /// <param name="data">The data to display.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        void Output<T>([NotNull, Localizable(true)] string title, [NotNull, ItemNotNull] IEnumerable<T> data);

        /// <summary>
        /// Displays tabular data to the user unless <see cref="Verbosity"/> is <see cref="Tasks.Verbosity.Batch"/>.
        /// </summary>
        /// <param name="title">A title for the message. Will only be displayed in GUIs, not on the console. Must not contain critical information!</param>
        /// <param name="data">The data to display.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        void OutputLow<T>([NotNull, Localizable(true)] string title, [NotNull, ItemNotNull] IEnumerable<T> data);

        /// <summary>
        /// Displays an error message to the user.
        /// </summary>
        /// <param name="exception">The exception representing the error that occurred.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")]
        void Error([NotNull] Exception exception);
    }
}
