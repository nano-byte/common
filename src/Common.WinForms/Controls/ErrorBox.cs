// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;

namespace NanoByte.Common.Controls;

/// <summary>
/// A dialog displaying an error message and details.
/// </summary>
public sealed partial class ErrorBox : Form
{
    private ErrorBox()
    {
        InitializeComponent();
        Font = DefaultFonts.Modern;
        HandleCreated += delegate { WindowsTaskbar.PreventPinning(Handle); };
    }

    /// <summary>
    /// Displays an error box with a message and details.
    /// </summary>
    /// <param name="owner">The parent window for the dialog; can be <c>null</c>.</param>
    /// <param name="exception">The error message to display.</param>
    /// <param name="log">Optional log messages.</param>
    /// <returns>The text the user entered if he pressed OK; otherwise <c>null</c>.</returns>
    public static void Show(IWin32Window? owner, Exception exception, RtfBuilder? log = null)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        Log.Debug("Showing error box", exception);

        string[] messageLines = exception.GetMessageWithInner().SplitMultilineText();
        using var errorBox = new ErrorBox
        {
            Text = Application.ProductName,
            labelMessage = {Text = messageLines[0]},
            labelDetails = {Text = StringUtils.Join(Environment.NewLine, messageLines.Skip(1))},
            textLog =
            {
                Rtf = log?.ToString(),
                Visible = log is {IsEmpty: false}
            },
            ShowInTaskbar = owner == null
        };

        // ReSharper disable once AccessToDisposedClosure
        errorBox.Shown += delegate { errorBox.SetForegroundWindow(); };
        errorBox.ShowDialog(owner);
    }

    private void label_Click(object? sender, EventArgs e)
    {
        if (sender is Control control)
        {
            new ContextMenuStrip
            {
                Items =
                {
                    {Resources.CopyToClipboard, null, delegate { Clipboard.SetText(control.Text); }}
                }
            }.Show(control, new Point());
        }
    }
}
