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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Gtk;
using JetBrains.Annotations;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// A dialog with a progress bar that runs and tracks the progress of an <see cref="ITask"/>.
    /// </summary>
    internal partial class TaskRunDialog : Dialog
    {
        private readonly ITask _task;
        private readonly Thread _taskThread;
        private readonly ICredentialProvider _credentialProvider;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Progress<TaskSnapshot> _progress = new Progress<TaskSnapshot>();

        /// <summary>
        /// Creates a new task tracking dialog.
        /// </summary>
        /// <param name="task">The trackable task to execute and display.</param>
        /// <param name="credentialProvider">Object used to retrieve credentials for specific <see cref="Uri"/>s on demand; can be <c>null</c>.</param>
        /// <param name="cancellationTokenSource">Used to signal if the user pressed the Cancel button.</param>
        /// <param name="parent">The parent window for this dialog; can be <c>null</c>.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Auto-generated")]
        public TaskRunDialog([NotNull] ITask task, ICredentialProvider credentialProvider, CancellationTokenSource cancellationTokenSource, [CanBeNull] Widget parent = null)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            Parent = parent;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Build();

            Title = task.Name;
            buttonCancel.Sensitive = task.CanCancel;

            _task = task;
            _taskThread = new Thread(RunTask);
            _cancellationTokenSource = cancellationTokenSource;
            _credentialProvider = credentialProvider;
        }

        private readonly ProgressBar progressBarTask = new ProgressBar();
        private readonly Label labelTask = new Label();
        private readonly Button buttonCancel = new Button
        {
            CanDefault = true,
            CanFocus = true,
            UseStock = true,
            UseUnderline = true,
            Label = "gtk-cancel"
        };

        private void Build()
        {
            var vBox = new VBox
            {
                BorderWidth = 2
            };

            var vBoxInner = new VBox
            {
                Spacing = 10,
                BorderWidth = 10,
            };
            vBox.Add(vBoxInner);
            {
                var child = (Box.BoxChild)vBox[vBoxInner];
                child.Position = 0;
                child.Expand = false;
                child.Fill = false;
            }

            vBoxInner.Add(progressBarTask);
            ((Box.BoxChild)vBoxInner[progressBarTask]).Position = 0;

            vBoxInner.Add(labelTask);
            ((Box.BoxChild)vBoxInner[labelTask]).Position = 1;

            var actionArea = new HButtonBox
            {
                Spacing = 10,
                BorderWidth = 5,
                LayoutStyle = ButtonBoxStyle.End
            };


            AddActionWidget(buttonCancel, -6);
            {
                var child = (ButtonBox.ButtonBoxChild)actionArea[buttonCancel];
                child.Expand = false;
                child.Fill = false;
            }

            WindowPosition = WindowPosition.CenterOnParent;
            Child?.ShowAll();
            DefaultWidth = 400;
            DefaultHeight = 150;

            Show();
        }

        /// <summary>
        /// Displays the dialog and runs the task. Handles cancellation and closing and destroying the dialog.
        /// </summary>
        public void Execute()
        {
            _progress.ProgressChanged += OnProgressChanged;
            _taskThread.Start();

            Run();

            while (_taskThread.IsAlive && Exception == null) // Only close the window if the task is no longer running
            {

                if (_task.CanCancel)
                {
                    buttonCancel.Sensitive = false; // Don't tempt user to press Cancel again
                    _cancellationTokenSource.Cancel();
                }

                Run();
            }

            Destroy();
        }

        private void OnProgressChanged(TaskSnapshot snapshot)
        {
            progressBarTask.Fraction = snapshot.Value;
            labelTask.Text = snapshot.ToString();

            if (snapshot.State == TaskState.Complete)
                OnClose();
        }

        /// <summary>
        /// An exception thrown by <see cref="ITask.Run"/>, if any.
        /// </summary>
        public Exception Exception { get; private set; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are rethrown on the UI thread.")]
        private void RunTask()
        {
            try
            {
                _task.Run(_cancellationTokenSource.Token, _credentialProvider, _progress);
            }
            catch (Exception ex)
            {
                Exception = ex;
                Application.Invoke(delegate { OnClose(); });
            }
        }
    }
}
