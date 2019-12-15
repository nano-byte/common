// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NanoByte.Common.Controls;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses simple WinForms dialog boxes to inform the user about the progress of tasks.
    /// </summary>
    public class DialogTaskHandler : GuiTaskHandlerBase
    {
        private readonly Control _owner;

        /// <summary>
        /// Creates a new task handler.
        /// </summary>
        /// <param name="owner">The parent window for any dialogs created by the handler.</param>
        public DialogTaskHandler(Control owner)
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
            Exception? ex = null;
            _owner.Invoke(() =>
            {
                using var dialog = new TaskRunDialog(task, CredentialProvider, CancellationTokenSource);
                dialog.ShowDialog(_owner);
                ex = dialog.Exception;
            });
            if (ex != null) throw ex.PreserveStack();
        }

        /// <inheritdoc/>
        protected override bool Ask(string question, MsgSeverity severity)
        {
            Log.Debug("Question: " + question);
            switch (_owner.Invoke(() => Msg.YesNoCancel(_owner, question, severity)))
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
        public override void Output(string title, string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (message == null) throw new ArgumentNullException(nameof(message));
            #endregion

            _owner.Invoke(() => OutputBox.Show(_owner, title, message.TrimEnd(Environment.NewLine.ToCharArray())));
        }

        /// <inheritdoc/>
        public override void Output<T>(string title, IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            _owner.Invoke(() => OutputGridBox.Show(_owner, title, data));
        }

        public override void Error(Exception exception)
        {
            #region Sanity checks
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            #endregion

            _owner.Invoke(() => ErrorBox.Show(_owner, exception, LogRtf));
        }
    }
}
