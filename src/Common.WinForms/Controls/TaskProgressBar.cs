// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;
using NanoByte.Common.Tasks;
using static NanoByte.Common.Tasks.TaskState;
using static NanoByte.Common.Native.WindowsTaskbar.ProgressBarState;

#if !NET20
using System.Collections.Concurrent;
#endif

namespace NanoByte.Common.Controls;

/// <summary>
/// A progress bar that takes <see cref="TaskSnapshot"/> inputs.
/// </summary>
public sealed class TaskProgressBar : ProgressBar, IProgress<TaskSnapshot>
{
    public TaskProgressBar()
    {
        CreateHandle();
    }

    /// <inheritdoc/>
    public void Report(TaskSnapshot value)
    {
        try
        {
            // Ensure execution on GUI thread
            if (InvokeRequired)
            {
                BeginInvoke(Report, value);
                return;
            }

            Value = value.Value switch
            {
                _ when value.State == Complete => 100,
                <= 0 => 0,
                >= 1 => 100,
                _ => (int)(value.Value * 100)
            };

            var state = value.State switch
            {
                Started or Header => Indeterminate,
                Data when value.UnitsTotal == -1 => Indeterminate,
                Data => Normal,
                Complete => NoProgress,
                WebError or IOError => Error,
                _ => NoProgress,
            };
            UpdateTaskbar(state);
            Style = state == Indeterminate
                ? ProgressBarStyle.Marquee
                : ProgressBarStyle.Continuous;
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or Win32Exception)
        {
            Log.Debug("Failed to update progress bar", ex);
        }
        #endregion
    }

#if NET20
    private void UpdateTaskbar(WindowsTaskbar.ProgressBarState state)
    {}
#else
    private IntPtr? _formHandle;
    private static readonly ConcurrentDictionary<IntPtr, TaskProgressBar> _taskbarOwners = [];

    private void UpdateTaskbar(WindowsTaskbar.ProgressBarState state)
    {
        _formHandle ??= FindForm()?.Handle;
        if (_formHandle is not {} formHandle) return;

        // Ensure only one progress bar at a time controls a form's taskbar entry
        if (_taskbarOwners.GetOrAdd(formHandle, this) != this) return;

        WindowsTaskbar.SetProgressState(formHandle, state);
        WindowsTaskbar.SetProgressValue(formHandle, Value, 100);

        if (state is NoProgress or Error)
            _taskbarOwners.TryRemove(formHandle, out _);
    }
#endif
}
