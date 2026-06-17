// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;
using static NanoByte.Common.Tasks.TaskState;

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
            }
            #region Error handling
            catch (Exception ex) when (ex is InvalidOperationException or ObjectDisposedException)
            {
                Log.Debug("Failed to update progress bar", ex);
            }
            #endregion
        });
}
