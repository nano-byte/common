// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.Controls;

/// <summary>
/// A progress label that takes <see cref="TaskSnapshot"/> inputs.
/// </summary>
public sealed class TaskLabel : Label, IProgress<TaskSnapshot>
{
    public TaskLabel()
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

            Text = value.ToString();
            UseMnemonic = false;

            ForeColor = value.State switch
            {
                TaskState.Complete => Color.Green,
                TaskState.WebError or TaskState.IOError => Color.Red,
                _ => SystemColors.ControlText
            };
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or Win32Exception)
        {
            Log.Debug("Failed to update progress label", ex);
        }
        #endregion
    }
}
