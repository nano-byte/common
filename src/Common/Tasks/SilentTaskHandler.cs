/*
 * Copyright 2006-2017 Bastian Eicher
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
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Executes tasks silently and suppresses any questions.
    /// </summary>
    public class SilentTaskHandler : TaskHandlerBase
    {
        /// <inheritdoc/>
        protected override void LogHandler(LogSeverity severity, string message)
        {}

        /// <inheritdoc/>
        protected override ICredentialProvider BuildCredentialProvider() => null;

        /// <inheritdoc/>
        public override void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            Log.Debug("Task: " + task.Name);
            task.Run(CancellationToken);
        }

        /// <summary>
        /// Always returns <see cref="Tasks.Verbosity.Batch"/>.
        /// </summary>
        public override Verbosity Verbosity { get => Verbosity.Batch; set {} }

        /// <summary>
        /// Always returns <c>false</c>.
        /// </summary>
        protected override bool Ask(string question, MsgSeverity severity)
        {
            Log.Debug($"Question: {question}\nAutomatic answer: No");
            return false;
        }

        /// <inheritdoc/>
        public override void Output(string title, string message) => Log.Info($"{title}\n{message}");

        /// <inheritdoc/>
        public override void Error(Exception exception) => Log.Error(exception);
    }
}
