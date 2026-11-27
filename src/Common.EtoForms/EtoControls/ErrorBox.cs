// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// A dialog displaying an error message and details.
/// </summary>
public static class ErrorBox
{
    /// <summary>
    /// Displays an error box with a message and details.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="exception">The error to display.</param>
    /// <param name="log">Optional aggregated log messages.</param>
    public static void Show(Control? owner, Exception exception, FormattedTextBuilder? log = null)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        Log.Debug("Showing error box", exception);

        string message = exception.GetMessageWithInner();
        try
        {
            var messageLines = message.SplitMultilineText().ToList();
            bool hasLog = log is {IsEmpty: false};

            using var dialog = new Dialog
            {
                Title = BoxLayout.AppName,
                Padding = 10,
                Resizable = true,
                Closeable = false
            };

            var layout = new StackLayout
            {
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Spacing = 8,
                Items = {BoxLayout.HeaderRow("Error", messageLines[0], out var headingLabel)}
            };
            headingLabel.WithCopyMenu();
            if (messageLines.Count > 1)
                layout.Items.Add(BoxLayout.Text(string.Join(Environment.NewLine, messageLines.Skip(1))).WithCopyMenu());
            if (hasLog) layout.Items.Add(new StackLayoutItem(log!.Build(), expand: true));
            dialog.Content = layout;
            dialog.AddCloseButton();

            if (hasLog) dialog.SetInitialClientSize(BoxLayout.TextSize(columns: 90, lines: 22));
            else dialog.FitContentHeight(BoxLayout.TextSize(columns: 80, lines: 0).Width);

            dialog.ShowBox(owner);
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Error("Failed to show error box", ex);

            // Use simple message box as last ditch effort to still show the original error message to the user
            Msg.Inform(owner, message, MsgSeverity.Error);
        }
        #endregion
    }
}
