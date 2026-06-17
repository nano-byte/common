// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// A progress label that takes <see cref="TaskSnapshot"/> inputs.
/// </summary>
public sealed class TaskLabel : Label, IProgress<TaskSnapshot>
{
    /// <inheritdoc/>
    public void Report(TaskSnapshot value)
        // Ensure execution on the UI thread
        => Application.Instance.AsyncInvoke(() =>
        {
            try
            {
                Text = value.ToString();

                TextColor = value.State switch
                {
                    TaskState.Complete => Colors.Green,
                    TaskState.WebError or TaskState.IOError => Colors.Red,
                    _ => SystemColors.ControlText
                };
            }
            #region Error handling
            catch (Exception ex) when (ex is InvalidOperationException or ObjectDisposedException)
            {
                Log.Debug("Failed to update progress label", ex);
            }
            #endregion
        });
}
