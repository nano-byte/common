// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Text;
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
        HandleCreated += delegate { WindowsTaskbar.PreventPinning(Handle); };
    }

    /// <summary>
    /// Displays an error box with a message and details.
    /// </summary>
    /// <param name="owner">The parent window for the dialog; can be <c>null</c>.</param>
    /// <param name="exception">The error message to display.</param>
    /// <param name="logRtf">The details formatted as RTF.</param>
    /// <returns>The text the user entered if he pressed OK; otherwise <c>null</c>.</returns>
    public static void Show(IWin32Window? owner, Exception exception, RtfBuilder logRtf)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        if (logRtf == null) throw new ArgumentNullException(nameof(logRtf));
        #endregion

        Log.Error(exception);

        using var errorBox = new ErrorBox
        {
            Text = Application.ProductName,
            labelMessage = {Text = exception.Message},
            labelInnerMessage = {Text = GetInnerMessage(exception)},
            textLog = {Rtf = logRtf.ToString()},
            ShowInTaskbar = owner == null
        };
        // ReSharper disable once AccessToDisposedClosure
        errorBox.Shown += delegate { errorBox.SetForegroundWindow(); };
        errorBox.ShowDialog(owner);
    }

    private static string GetInnerMessage(Exception exception)
    {
        var builder = new StringBuilder();
        while (exception.InnerException != null && exception.InnerException.Message != exception.Message)
        {
            exception = exception.InnerException;
            builder.AppendLine(exception.Message);
        }
        return builder.ToString();
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