// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Info;
using NanoByte.Common.Streams;

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

            var messageLayout = new StackLayout
            {
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Spacing = 8,
                Items =
                {
                    new Label {Text = messageLines[0], Font = SystemFonts.Bold(), Wrap = WrapMode.Word}.ReflowOnResize()
                }
            };
            if (messageLines.Count > 1)
                messageLayout.Items.Add(new Label {Text = string.Join(Environment.NewLine, messageLines.Skip(1)), Wrap = WrapMode.Word}.ReflowOnResize());

            var layout = new StackLayout
            {
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Spacing = 8,
                Items =
                {
                    new TableLayout
                    {
                        Spacing = new(12, 0),
                        Rows = {new TableRow(new TableCell(ErrorIcon()), new TableCell(messageLayout, scaleWidth: true))}
                    }
                }
            };
            if (log is {IsEmpty: false})
                layout.Items.Add(new StackLayoutItem(log.Build(), expand: true));

            using var dialog = new Dialog
            {
                Title = AppName,
                Padding = 10,
                Content = layout,
                ShowInTaskbar = owner == null
            };
            if (log is {IsEmpty: false})
            {
                dialog.Resizable = true;
                dialog.Size = new(550, 400);
            }
            else dialog.FitContentHeight(450);

            var closeButton = new Button {Text = Resources.OK};
            closeButton.Click += delegate { dialog.Close(); };
            dialog.PositiveButtons.Add(closeButton);
            dialog.DefaultButton = closeButton;
            dialog.AbortButton = closeButton;
            dialog.CenterOnScreenIfOwnerless();
            dialog.PreventPinning();

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

    /// <summary>
    /// The name of the application, for use as a window title.
    /// </summary>
    private static string? AppName
        => string.IsNullOrEmpty(Application.Instance.Name)
            ? AppInfo.Current.ProductName ?? AppInfo.Current.Name
            : Application.Instance.Name;

    private static Control ErrorIcon()
    {
        try
        {
            using var stream = typeof(ErrorBox).GetEmbeddedStream("Error.png");
            return new ImageView {Image = new Bitmap(stream), Size = new(48, 48)};
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Failed to load error icon", ex);
            return new Panel();
        }
        #endregion
    }
}
