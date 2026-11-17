// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Combines a <see cref="TaskProgressBar"/> and a <see cref="TaskLabel"/>.
/// </summary>
public sealed class TaskControl : Panel, IProgress<TaskSnapshot>
{
    private readonly Label _operationLabel = new() {Wrap = WrapMode.None};
    private readonly TaskProgressBar _progressBar = new();
    private readonly TaskLabel _progressLabel = new();

    /// <summary>
    /// The name of the task being tracked.
    /// </summary>
    [Description("The name of the task being tracked.")]
    public string TaskName
    {
        get;
        set
        {
            field = value ?? "";
            _operationLabel.ToolTip = field;
            UpdateOperationLabel();
        }
    } = "";

    /// <summary>
    /// Creates a new task tracking control.
    /// </summary>
    public TaskControl()
    {
        // Fixed minimal size, so long task names are cut off instead of widening the surrounding layout
        _operationLabel.Size = new(1, (int)Math.Ceiling(_operationLabel.Font.LineHeight) + 2);
        _operationLabel.LoadComplete += delegate { UpdateOperationLabel(); };
        _operationLabel.SizeChanged += delegate { UpdateOperationLabelDeferred(); };

        Content = new TableLayout
        {
            Spacing = new(0, 3),
            Rows = {_operationLabel, _progressBar, _progressLabel, null}
        };
    }

    private bool _updatePending;

    /// <summary>
    /// Calls <see cref="UpdateOperationLabel"/> once the current layout pass is complete.
    /// </summary>
    private void UpdateOperationLabelDeferred()
    {
        if (_updatePending) return;
        _updatePending = true;
        Application.Instance.AsyncInvoke(() =>
        {
            _updatePending = false;
            UpdateOperationLabel();
        });
    }

    /// <summary>
    /// Displays <see cref="TaskName"/>, shortened with "…" if it does not fit the available width.
    /// </summary>
    private void UpdateOperationLabel()
    {
        string text = TaskName;
        float width = _operationLabel.Width - 2; // Small margin to account for differences between text measuring and rendering
        var font = _operationLabel.Font;

        if (_operationLabel.Loaded && width > 0 && text.Length != 0 && font.MeasureString(text).Width > width)
        {
            // Binary search for the longest prefix that fits together with the ellipsis
            int min = 0, max = text.Length;
            while (min < max)
            {
                int mid = (min + max + 1) / 2;
                if (font.MeasureString(text.Substring(0, mid) + "…").Width <= width) min = mid;
                else max = mid - 1;
            }
            text = text.Substring(0, min) + "…";
        }

        if (_operationLabel.Text != text) _operationLabel.Text = text;
    }

    /// <inheritdoc/>
    public void Report(TaskSnapshot value)
    {
        _progressBar.Report(value);
        _progressLabel.Report(value);
    }
}
