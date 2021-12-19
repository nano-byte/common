// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;
using NanoByte.Common.Tasks;

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
        // Ensure execution on GUI thread
        if (InvokeRequired)
        {
            BeginInvoke(new Action<TaskSnapshot>(Report), value);
            return;
        }

        Value = value.Value switch
        {
            _ when value.State == TaskState.Complete => 100,
            <= 0 => 0,
            >= 1 => 100,
            _ => (int)(value.Value * 100)
        };

        switch (value.State)
        {
            case TaskState.Ready:
                Style = ProgressBarStyle.Continuous;
                UpdateTaskbar(WindowsTaskbar.ProgressBarState.NoProgress);
                break;

            case TaskState.Started or TaskState.Header:
                Style = ProgressBarStyle.Marquee;
                UpdateTaskbar(WindowsTaskbar.ProgressBarState.Indeterminate);
                break;

            case TaskState.Data when value.UnitsTotal == -1:
                Style = ProgressBarStyle.Marquee;
                UpdateTaskbar(WindowsTaskbar.ProgressBarState.Indeterminate);
                break;

            case TaskState.Data:
                Style = ProgressBarStyle.Continuous;
                UpdateTaskbar(WindowsTaskbar.ProgressBarState.Normal);
                break;

            case TaskState.IOError or TaskState.WebError:
                Style = ProgressBarStyle.Continuous;
                UpdateTaskbar(WindowsTaskbar.ProgressBarState.Error);
                break;

            case TaskState.Complete:
                Style = ProgressBarStyle.Continuous;
                UpdateTaskbar(WindowsTaskbar.ProgressBarState.NoProgress);
                break;
        }
    }

#if NET20
    private void UpdateTaskbar(WindowsTaskbar.ProgressBarState state)
    {}
#else
    private IntPtr? _formHandle;
    private static readonly ConcurrentDictionary<IntPtr, TaskProgressBar> _taskbarOwners = new();

    private void UpdateTaskbar(WindowsTaskbar.ProgressBarState state)
    {
        _formHandle ??= FindForm()?.Handle;
        if (!_formHandle.HasValue) return;
        var formHandle = _formHandle.Value;

        // Ensure only one progress bar at a time controls a form's taskbar entry
        if (_taskbarOwners.GetOrAdd(formHandle, this) != this) return;

        WindowsTaskbar.SetProgressState(formHandle, state);
        WindowsTaskbar.SetProgressValue(formHandle, Value, 100);

        if (state is (WindowsTaskbar.ProgressBarState.NoProgress or WindowsTaskbar.ProgressBarState.Error))
            _taskbarOwners.TryRemove(formHandle, out _);
    }
#endif
}
