/*
 * Copyright 2006-2014 Bastian Eicher
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
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Controls;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses simple dialog boxes to inform the user about the progress of tasks.
    /// </summary>
    public class GuiTaskHandler : MarshalNoTimeout, ITaskHandler
    {
        [CanBeNull]
        private readonly IWin32Window _owner;

        /// <summary>
        /// Starts <see cref="Log"/> recording.
        /// </summary>
        /// <param name="owner">The parent window for any dialogs created by the handler; can be <see langword="null"/>.</param>
        public GuiTaskHandler([CanBeNull] IWin32Window owner = null)
        {
            _owner = owner;

            Log.Handler += LogHandler;
        }

        /// <summary>
        /// Records <see cref="Log"/> messages in <see cref="ErrorLog"/> based on their <see cref="LogSeverity"/> and the current <see cref="Verbosity"/> level.
        /// </summary>
        /// <param name="severity">The type/severity of the entry.</param>
        /// <param name="message">The message text of the entry.</param>
        protected virtual void LogHandler(LogSeverity severity, string message)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    if (Verbosity >= Verbosity.Debug) _errorLog.AppendPar(message, RtfColor.Blue);
                    break;
                case LogSeverity.Info:
                    if (Verbosity >= Verbosity.Verbose) _errorLog.AppendPar(message, RtfColor.Green);
                    break;
                case LogSeverity.Warn:
                    _errorLog.AppendPar(message, RtfColor.Orange);
                    break;
                case LogSeverity.Error:
                    _errorLog.AppendPar(message, RtfColor.Red);
                    break;
            }
        }

        private readonly RtfBuilder _errorLog = new RtfBuilder();

        /// <summary>
        /// Collects <see cref="Log"/> entries with color-coded formatting.
        /// </summary>
        public RtfBuilder ErrorLog { get { return _errorLog; } }

        /// <summary>
        /// Used to signal the <see cref="CancellationToken"/>.
        /// </summary>
        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get { return CancellationTokenSource.Token; } }

        /// <inheritdoc/>
        public virtual void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException("task");
            #endregion

            Log.Debug("Task: " + task.Name);
            using (var dialog = new TaskRunDialog(task, CancellationTokenSource))
            {
                dialog.ShowDialog(_owner);
                if (dialog.Exception != null) dialog.Exception.Rethrow();
            }
        }

        /// <inheritdoc/>
        public Verbosity Verbosity { get; set; }

        /// <inheritdoc/>
        public virtual bool Ask(string question)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException("question");
            #endregion

            Log.Debug("Question: " + question);
            switch (Msg.YesNoCancel(_owner, question, MsgSeverity.Warn))
            {
                case DialogResult.Yes:
                    Log.Debug("Answer: Yes");
                    return true;
                case DialogResult.No:
                    Log.Debug("Answer: No");
                    return false;
                case DialogResult.Cancel:
                default:
                    Log.Debug("Answer: Cancel");
                    throw new OperationCanceledException();
            }
        }

        /// <inheritdoc/>
        public virtual void Output(string title, string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException("title");
            if (message == null) throw new ArgumentNullException("message");
            #endregion

            OutputBox.Show(title, message, _owner);
        }

        /// <inheritdoc/>
        public virtual void Output<T>(string title, IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException("title");
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            OutputGridBox.Show(title, data, _owner);
        }

        #region Dispose
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Log.Handler -= LogHandler;
                CancellationTokenSource.Dispose();
            }
        }
        #endregion
    }
}
