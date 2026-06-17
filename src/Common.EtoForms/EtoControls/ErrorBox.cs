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

            var layout = new StackLayout
            {
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Spacing = 8,
                Items =
                {
                    new Label {Text = messageLines[0], Font = SystemFonts.Bold()}
                }
            };
            if (messageLines.Count > 1)
                layout.Items.Add(new Label {Text = string.Join(Environment.NewLine, messageLines.Skip(1))});

            if (log is {IsEmpty: false})
                layout.Items.Add(new StackLayoutItem(log.Build(), expand: true));

            using var dialog = new Dialog
            {
                Title = Application.Instance.Name,
                Padding = 10,
                Width = 500,
                Height = log is {IsEmpty: false} ? 400 : -1,
                Content = layout
            };

            var closeButton = new Button {Text = Resources.OK};
            closeButton.Click += delegate { dialog.Close(); };
            dialog.PositiveButtons.Add(closeButton);
            dialog.DefaultButton = closeButton;
            dialog.AbortButton = closeButton;

            if (owner == null) dialog.ShowModal();
            else dialog.ShowModal(owner);
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
