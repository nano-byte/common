// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Concurrent;
using NanoByte.Common.Native;
using NanoByte.Common.Tasks;
using static NanoByte.Common.Tasks.TaskState;
using static NanoByte.Common.Native.WindowsTaskbar.ProgressBarState;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// A progress bar that takes <see cref="TaskSnapshot"/> inputs.
/// </summary>
public sealed class TaskProgressBar : ProgressBar, IProgress<TaskSnapshot>
{
    /// <inheritdoc/>
    public void Report(TaskSnapshot value)
        // Ensure execution on the UI thread
        => Application.Instance.AsyncInvoke(() =>
        {
            try
            {
                bool indeterminate = value.State switch
                {
                    Started or Header => true,
                    Data when value.UnitsTotal == -1 => true,
                    _ => false
                };

                Indeterminate = indeterminate;
                if (!indeterminate)
                {
                    Value = value.Value switch
                    {
                        _ when value.State == Complete => MaxValue,
                        <= 0 => MinValue,
                        >= 1 => MaxValue,
                        _ => (int)(value.Value * MaxValue)
                    };
                }

                UpdateTaskbar(value.State, indeterminate);
            }
            #region Error handling
            catch (Exception ex) when (ex is InvalidOperationException or ObjectDisposedException)
            {
                Log.Debug("Failed to update progress bar", ex);
            }
            #endregion
        });

    private IntPtr _windowHandle;
    private static readonly ConcurrentDictionary<IntPtr, TaskProgressBar> _taskbarOwners = [];

    private void UpdateTaskbar(TaskState taskState, bool indeterminate)
    {
        if (!WindowsUtils.IsWindows7) return;

        var state = taskState switch
        {
            _ when indeterminate => WindowsTaskbar.ProgressBarState.Indeterminate,
            Data => Normal,
            WebError or IOError => Error,
            _ => NoProgress
        };

        if (_windowHandle == IntPtr.Zero) _windowHandle = ParentWindow?.NativeHandle ?? IntPtr.Zero;
        if (_windowHandle == IntPtr.Zero) return;

        // Ensure only one progress bar at a time controls a window's taskbar entry
        if (_taskbarOwners.GetOrAdd(_windowHandle, this) != this) return;

        WindowsTaskbar.SetProgressState(_windowHandle, state);
        WindowsTaskbar.SetProgressValue(_windowHandle, Value - MinValue, MaxValue - MinValue);

        if (state is NoProgress or Error)
            _taskbarOwners.TryRemove(_windowHandle, out _);
    }
}
