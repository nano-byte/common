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
    public sealed class TaskProgressBar : ProgressBar, IProgress<TaskSnapshot>
    {
        /// <summary>
        /// Determines the handle of the <see cref="Form"/> containing this control.
        /// </summary>
        /// <returns>The handle of the parent <see cref="Form"/> or <see cref="IntPtr.Zero"/> if there is no parent.</returns>
        private IntPtr ParentHandle
        {
            get
            {
                var parent = FindForm();
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

                case TaskState.Started or TaskState.Header:
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

                case TaskState.IOError or TaskState.WebError:
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

            int currentValue = value.Value switch
            {
                <= 0 => 0,
                >= 1 => 100,
                _ => (int)(value.Value * 100)
            };

            // When the State is complete the bar should always be full
            if (value.State == TaskState.Complete) currentValue = 100;

            Value = currentValue;
            IntPtr formHandle = ParentHandle;
            if (UseTaskbar && formHandle != IntPtr.Zero) WindowsTaskbar.SetProgressValue(formHandle, currentValue, 100);
        }
    }
}
