// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Builds a formatted string with colored paragraphs for display in a <see cref="RichTextArea"/>.
/// </summary>
public sealed class FormattedTextBuilder
{
    private readonly List<(LogSeverity Severity, string Message)> _entries = [];

    /// <summary>
    /// Indicates whether no entries have been appended yet.
    /// </summary>
    public bool IsEmpty => _entries.Count == 0;

    /// <summary>
    /// Appends a log entry as a new line.
    /// </summary>
    /// <param name="severity">The type/severity of the entry. Determines the text color.</param>
    /// <param name="message">The message of the entry.</param>
    public void AppendLine(LogSeverity severity, string message)
    {
        #region Sanity checks
        if (message == null) throw new ArgumentNullException(nameof(message));
        #endregion

        _entries.Add((severity, message));
    }

    /// <summary>
    /// Builds a read-only <see cref="RichTextArea"/> displaying the aggregated entries with colors based on their severity.
    /// </summary>
    public RichTextArea Build()
    {
        var textArea = new RichTextArea {ReadOnly = true};

        int position = 0;
        foreach ((var severity, string message) in _entries)
        {
            textArea.Append(message + "\n", scrollToCursor: false);
            if (GetColor(severity) is {} color)
                textArea.Buffer.SetForeground(new Range<int>(position, position + message.Length - 1), color);
            position += message.Length + 1;
        }

        return textArea;
    }

    /// <summary>
    /// Determines the color to use for a log entry based on the <see cref="LogSeverity"/>.
    /// </summary>
    /// <returns>The color to use, or <c>null</c> to use the default text color.</returns>
    private static Color? GetColor(LogSeverity severity)
        => severity switch
        {
            LogSeverity.Debug => Colors.Blue,
            LogSeverity.Info => Colors.Green,
            LogSeverity.Warn => Colors.Orange,
            LogSeverity.Error => Colors.Red,
            _ => null
        };

    /// <summary>
    /// Returns all aggregated entries as a single string.
    /// </summary>
    public override string ToString() => string.Join(Environment.NewLine, _entries.Select(x => x.Message));
}
