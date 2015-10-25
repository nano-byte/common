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
using System.Linq;
using Gtk;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses simple dialog boxes to inform the user about the progress of tasks.
    /// </summary>
    public class GuiTaskHandler : TaskHandlerBase
    {
        [CanBeNull]
        private readonly Window _owner;

        /// <summary>
        /// Creates a new task handler.
        /// </summary>
        /// <param name="owner">The parent window for any dialogs created by the handler.</param>
        public GuiTaskHandler([CanBeNull] Window owner = null)
        {
            _owner = owner;
        }

        /// <inheritdoc/>
        protected override void LogHandler(LogSeverity severity, string message)
        {
            // TODO: Implement
        }

        /// <inheritdoc/>
        public override void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException("task");
            #endregion

            Log.Debug("Task: " + task.Name);
            Invoke(() =>
            {
                using (var dialog = new TaskRunDialog(task, CancellationTokenSource, _owner))
                {
                    dialog.Execute();
                    if (dialog.Exception != null) dialog.Exception.Rethrow();
                }
            });
        }

        /// <inheritdoc/>
        public override bool Ask(string question)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException("question");
            #endregion

            Log.Debug("Question: " + question);
            switch (Invoke(() => Msg.YesNoCancel(_owner, question, MsgSeverity.Warn)))
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
            if (title == null) throw new ArgumentNullException("title");
            if (message == null) throw new ArgumentNullException("message");
            #endregion

            Invoke(() => Msg.Inform(_owner, title + Environment.NewLine + message, MsgSeverity.Info));
        }

        /// <inheritdoc/>
        public override void Output<T>(string title, IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException("title");
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            string message = StringUtils.Join(Environment.NewLine, data.Select(x => x.ToString()));
            Output(title, message);
        }

        /// <inheritdoc/>
        public virtual ICredentialProvider BuildCredentialProvider()
        {
            return null;
        }

        private void Invoke(System.Action action)
        {
            if (_owner == null) action();
            else Application.Invoke((sender, args) => action());
        }

        private T Invoke<T>(Func<T> action)
        {
            if (_owner == null) return action();
            else
            {
                T result = default(T);
                Application.Invoke((sender, args) => { result = action(); });
                return result;
            }
        }
    }
}
