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
        // Ensure execution on GUI thread
        if (InvokeRequired)
        {
            BeginInvoke(new Action<TaskSnapshot>(Report), value);
            return;
        }

        Text = value.ToString();

        ForeColor = value.State switch
        {
            TaskState.Complete => Color.Green,
            TaskState.WebError or TaskState.IOError => Color.Red,
            _ => SystemColors.ControlText
        };
    }
}