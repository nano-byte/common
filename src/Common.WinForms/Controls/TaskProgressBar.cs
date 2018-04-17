// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Windows.Forms;
using NanoByte.Common.Native;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A progress bar that takes <see cref="TaskSnapshot"/> inputs.
    /// </summary>
    public sealed class TaskProgressBar : ProgressBar, Tasks.IProgress<TaskSnapshot>
    {
        /// <summary>
        /// Determines the handle of the <see cref="Form"/> containing this control.
        /// </summary>
        /// <returns>The handle of the parent <see cref="Form"/> or <see cref="IntPtr.Zero"/> if there is no parent.</returns>
        private IntPtr ParentHandle
        {
            get
            {
                Form parent = FindForm();
                return parent?.Handle ?? IntPtr.Zero;
            }
        }

        /// <summary>
        /// Show the progress in the Windows taskbar.
        /// </summary>
        /// <remarks>Use only once per window. Only works on Windows 7 or newer.</remarks>
        [Description("Show the progress in the Windows taskbar."), DefaultValue(false)]
        public bool UseTaskbar { set; get; }

        public TaskProgressBar() => CreateHandle();

        /// <inheritdoc/>
        public void Report(TaskSnapshot value)
        {
            // Ensure execution on GUI thread
            if (InvokeRequired)
            {
                BeginInvoke(new Action<TaskSnapshot>(Report), value);
                return;
            }

            switch (value.State)
            {
                case TaskState.Ready:
                    // When the State is complete the bar should always be empty
                    Style = ProgressBarStyle.Continuous;
                    Value = 0;

                    if (UseTaskbar && ParentHandle != IntPtr.Zero) WindowsTaskbar.SetProgressState(ParentHandle, WindowsTaskbar.ProgressBarState.NoProgress);
                    break;

                case TaskState.Started:
                case TaskState.Header:
                    Style = ProgressBarStyle.Marquee;
                    if (UseTaskbar && ParentHandle != IntPtr.Zero) WindowsTaskbar.SetProgressState(ParentHandle, WindowsTaskbar.ProgressBarState.Indeterminate);
                    break;

                case TaskState.Data:
                    if (value.UnitsTotal == -1)
                    {
                        Style = ProgressBarStyle.Marquee;
                        if (UseTaskbar && ParentHandle != IntPtr.Zero) WindowsTaskbar.SetProgressState(ParentHandle, WindowsTaskbar.ProgressBarState.Indeterminate);
                    }
                    else
                    {
                        Style = ProgressBarStyle.Continuous;
                        if (UseTaskbar && ParentHandle != IntPtr.Zero) WindowsTaskbar.SetProgressState(ParentHandle, WindowsTaskbar.ProgressBarState.Normal);
                    }
                    break;

                case TaskState.IOError:
                case TaskState.WebError:
                    Style = ProgressBarStyle.Continuous;
                    if (UseTaskbar && ParentHandle != IntPtr.Zero) WindowsTaskbar.SetProgressState(ParentHandle, WindowsTaskbar.ProgressBarState.Error);
                    break;

                case TaskState.Complete:
                    // When the State is complete the bar should always be full
                    Style = ProgressBarStyle.Continuous;
                    Value = 100;

                    if (UseTaskbar && ParentHandle != IntPtr.Zero) WindowsTaskbar.SetProgressState(ParentHandle, WindowsTaskbar.ProgressBarState.NoProgress);
                    break;
            }

            var currentValue = (int)(value.Value * 100);
            if (currentValue < 0) currentValue = 0;
            else if (currentValue > 100) currentValue = 100;

            // When the State is complete the bar should always be full
            if (value.State == TaskState.Complete) currentValue = 100;

            Value = currentValue;
            IntPtr formHandle = ParentHandle;
            if (UseTaskbar && formHandle != IntPtr.Zero) WindowsTaskbar.SetProgressValue(formHandle, currentValue, 100);
        }
    }
}
