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
using System.Windows.Forms;
using NanoByte.Common.Controls;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses simple dialog boxes to inform the user about the progress of tasks.
    /// </summary>
    public class GuiTaskHandler : MarshalNoTimeout, ITaskHandler
    {
        private readonly IWin32Window _owner;

        /// <summary>
        /// Creates a new task handler.
        /// </summary>
        /// <param name="owner">The parent window for any dialogs created by the handler.</param>
        public GuiTaskHandler(IWin32Window owner = null)
        {
            _owner = owner;
        }

        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get { return CancellationTokenSource.Token; } }

        /// <inheritdoc/>
        public virtual void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException("task");
            #endregion

            using (var dialog = new TaskRunDialog(task, CancellationTokenSource))
            {
                dialog.ShowDialog(_owner);
                if (dialog.Exception != null) dialog.Exception.Rethrow();
            }
        }

        /// <summary>
        /// Always returns 1. This ensures that information hidden by the GUI is at least retrievable from the log files.
        /// </summary>
        public virtual int Verbosity { get { return 1; } set { } }

        /// <inheritdoc/>
        public bool Batch { get; set; }

        /// <inheritdoc/>
        public virtual bool AskQuestion(string question, string batchInformation = null)
        {
            if (Batch)
            {
                if (!string.IsNullOrEmpty(batchInformation)) Log.Warn(batchInformation);
                return false;
            }

            return Msg.YesNo(_owner, question, MsgSeverity.Warn);
        }

        /// <inheritdoc/>
        public virtual void Output(string title, string information)
        {
            OutputBox.Show(title, information);
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
            if (disposing) CancellationTokenSource.Dispose();
        }
        #endregion
    }
}
