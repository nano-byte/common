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
using System.ComponentModel;
using System.IO;
using System.Net;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Represents an operation that can be cancelled and have its progress tracked.
    /// </summary>
    /// <remarks>
    /// Unlike System.Threading.Tasks.Task, these tasks do not provide any asynchrony by themselves.
    /// They execute all their code on the same thread they are started on and rely on <seealso cref="ITaskHandler"/>s for scheduling on background threads.
    /// </remarks>
    /// <seealso cref="ITaskHandler"/>
    public interface ITask
    {
        /// <summary>
        /// Runs the task and blocks until it is complete.
        /// </summary>
        /// <param name="cancellationToken">Used to receive a signal (e.g. from another thread) when the user wishes to cancel the task execution.</param>
        /// <param name="progress">Used to report back the task's progress (e.g. to another thread).</param>
        /// <param name="credentialProvider">Object used to retrieve credentials for specific <see cref="Uri"/>s on demand; can be <see langword="null"/>.</param>
        /// <exception cref="OperationCanceledException">The task was canceled from another thread.</exception>
        /// <exception cref="IOException">The task ended with <see cref="TaskState.IOError"/>.</exception>
        /// <exception cref="WebException">The task ended with <see cref="TaskState.WebError"/>.</exception>
        /// <seealso cref="ITaskHandler.RunTask"/>
        void Run(CancellationToken cancellationToken = default(CancellationToken), [CanBeNull] IProgress<TaskSnapshot> progress = null, [CanBeNull] ICredentialProvider credentialProvider = null);

        /// <summary>
        /// A name describing the task in human-readable form.
        /// </summary>
        [Description("A name describing the task in human-readable form.")]
        [NotNull, Localizable(true)]
        string Name { get; }

        /// <summary>
        /// An object used to associate the task with a specific process; can be <see langword="null"/>.
        /// </summary>
        [CanBeNull]
        object Tag { get; set; }

        /// <summary>
        /// Indicates whether this task can be canceled once it has been started.
        /// </summary>
        [Description("Indicates whether this task can be canceled once it has been started.")]
        bool CanCancel { get; }
    }
}
