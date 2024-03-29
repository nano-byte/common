// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.Controls;

/// <summary>
/// Combines a <see cref="TaskProgressBar"/> and a <see cref="TaskLabel"/>.
/// </summary>
public sealed partial class TaskControl : UserControl, IProgress<TaskSnapshot>
{
    /// <summary>
    /// The name of the task being tracked.
    /// </summary>
    [Description("The name of the task being tracked.")]
    [DefaultValue("")]
    public string TaskName
    {
        get => labelOperation.Text;
        set => labelOperation.Text = value;
    }

    /// <summary>
    /// Creates a new task tracking control.
    /// </summary>
    public TaskControl()
    {
        InitializeComponent();
        CreateHandle();
    }

    /// <inheritdoc/>
    public void Report(TaskSnapshot value)
    {
        progressBar.Report(value);
        progressLabel.Report(value);
    }
}
