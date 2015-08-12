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
using Gtk;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses simple GTK# dialog boxes to inform the user about the progress of tasks.
    /// </summary>
    public class DialogTaskHandler : GuiTaskHandlerBase
    {
        [CanBeNull]
        private readonly Window _owner;

        /// <summary>
        /// Creates a new task handler.
        /// </summary>
        /// <param name="owner">The parent window for any dialogs created by the handler.</param>
        public DialogTaskHandler([CanBeNull] Window owner = null)
        {
            _owner = owner;
        }

        /// <inheritdoc/>
        public override void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            Log.Debug("Task: " + task.Name);
            ApplicationUtils.Invoke(() =>
            {
                using (var dialog = new TaskRunDialog(task, CredentialProvider, CancellationTokenSource, _owner))
                {
                    dialog.Execute();
                    if (dialog.Exception != null) throw dialog.Exception.PreserveStack();
                }
            });
        }

        /// <inheritdoc/>
        public override bool Ask(string question)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException(nameof(question));
            #endregion

            Log.Debug("Question: " + question);
            switch (ApplicationUtils.Invoke(() => Msg.YesNoCancel(_owner, question, MsgSeverity.Warn)))
            {
                case ResponseType.Yes:
                    Log.Debug("Answer: Yes");
                    return true;
                case ResponseType.No:
                    Log.Debug("Answer: No");
                    return false;
                case ResponseType.Cancel:
                default:
                    Log.Debug("Answer: Cancel");
                    throw new OperationCanceledException();
            }
        }

        /// <inheritdoc/>
        public override void Output(string title, string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (message == null) throw new ArgumentNullException(nameof(message));
            #endregion

            ApplicationUtils.Invoke(() => Msg.Inform(_owner, title + Environment.NewLine + message, MsgSeverity.Info));
        }

        /// <inheritdoc/>
        public override void Error(Exception exception)
        {
            #region Sanity checks
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            #endregion

            Log.Error(exception);
            Msg.Inform(_owner, exception.Message, MsgSeverity.Error);
        }
    }
}
