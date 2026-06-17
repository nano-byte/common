// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Combines a <see cref="TaskProgressBar"/> and a <see cref="TaskLabel"/>.
/// </summary>
public sealed class TaskControl : Panel, IProgress<TaskSnapshot>
{
    private readonly Label _operationLabel = new();
    private readonly TaskProgressBar _progressBar = new();
    private readonly TaskLabel _progressLabel = new();

    /// <summary>
    /// The name of the task being tracked.
    /// </summary>
    [Description("The name of the task being tracked.")]
    public string TaskName
    {
        get => _operationLabel.Text;
        set => _operationLabel.Text = value;
    }

    /// <summary>
    /// Creates a new task tracking control.
    /// </summary>
    public TaskControl()
    {
        Content = new StackLayout
        {
            Orientation = Orientation.Vertical,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Spacing = 3,
            Items = {_operationLabel, _progressBar, _progressLabel}
        };
    }

    /// <inheritdoc/>
    public void Report(TaskSnapshot value)
    {
        _progressBar.Report(value);
        _progressLabel.Report(value);
    }
}
