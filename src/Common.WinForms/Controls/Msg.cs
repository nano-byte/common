// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using System.Runtime.InteropServices;
using NanoByte.Common.Native;

#if NET45 || NET462 || NET472
using TaskDialog;
#endif

namespace NanoByte.Common.Controls;

/// <summary>
/// Provides easier access to typical <see cref="MessageBox"/> configurations and automatically upgrades to TaskDialogs when available.
/// </summary>
public static class Msg
{
    /// <summary>
    /// Displays a message to the user using a message box or task dialog.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    public static void Inform(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity)
    {
        #region Sanity checks
        if (text == null) throw new ArgumentNullException(nameof(text));
        #endregion

        if (ShowTaskDialog(owner, text, severity) == null)
            ShowMessageBox(owner, text, severity, MessageBoxButtons.OK);
    }

    /// <summary>
    /// Asks the user a OK/Cancel-question using a message box or task dialog.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    /// <param name="okCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.OK"/> option; <c>null</c> for default.</param>
    /// <param name="cancelCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.Cancel"/> option; <c>null</c> for default.</param>
    /// <returns><c>true</c> if <paramref name="okCaption"/> was selected, <c>false</c> if <paramref name="cancelCaption"/> was selected.</returns>
    /// <remarks>If a <see cref="MessageBox"/> is used, <paramref name="okCaption"/> and <paramref name="cancelCaption"/> are not displayed to the user, so don't rely on them!</remarks>
    public static bool OkCancel(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity, [Localizable(true)] string? okCaption = null, [Localizable(true)] string? cancelCaption = null)
    {
        #region Sanity checks
        if (text == null) throw new ArgumentNullException(nameof(text));
        #endregion

        var result = ShowTaskDialog(owner, text, severity, ok: okCaption ?? "OK", cancel: cancelCaption, canCancel: true)
                  ?? ShowMessageBox(owner, text, severity, MessageBoxButtons.OKCancel);
        return result == DialogResult.OK;
    }

    /// <summary>
    /// Asks the user to choose between two options (yes/no) using a message box or task dialog.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    /// <param name="yesCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.Yes"/> option; <c>null</c> for default.</param>
    /// <param name="noCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.No"/> option; <c>null</c> for default.</param>
    /// <returns><c>true</c> if <paramref name="yesCaption"/> was chosen, <c>false</c> if <paramref name="noCaption"/> was chosen.</returns>
    /// <remarks>If a <see cref="MessageBox"/> is used, <paramref name="yesCaption"/> and <paramref name="noCaption"/> are not displayed to the user, so don't rely on them!</remarks>
    public static bool YesNo(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity, [Localizable(true)] string? yesCaption = null, [Localizable(true)] string? noCaption = null)
    {
        #region Sanity checks
        if (text == null) throw new ArgumentNullException(nameof(text));
        #endregion

        var result = ShowTaskDialog(owner, text, severity, yes: yesCaption ?? Resources.Yes, no: noCaption ?? Resources.No)
                  ?? ShowMessageBox(owner, text, severity, MessageBoxButtons.YesNo);
        return result == DialogResult.Yes;
    }

    /// <summary>
    /// Asks the user to choose between three options (yes/no/cancel) using a message box or task dialog.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    /// <param name="yesCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.Yes"/> option; <c>null</c> for default.</param>
    /// <param name="noCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.No"/> option; <c>null</c> for default.</param>
    /// <returns><see cref="DialogResult.Yes"/> if <paramref name="yesCaption"/> was chosen,
    /// <see cref="DialogResult.No"/> if <paramref name="noCaption"/> was chosen,
    /// <see cref="DialogResult.Cancel"/> otherwise.</returns>
    /// <remarks>If a <see cref="MessageBox"/> is used, <paramref name="yesCaption"/> and <paramref name="noCaption"/> are not displayed to the user, so don't rely on them!</remarks>
    public static DialogResult YesNoCancel(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity, [Localizable(true)] string? yesCaption = null, [Localizable(true)] string? noCaption = null)
    {
        #region Sanity checks
        if (text == null) throw new ArgumentNullException(nameof(text));
        #endregion

        return ShowTaskDialog(owner, text, severity, yes: yesCaption ?? Resources.Yes, no: noCaption ?? Resources.No, canCancel: true)
            ?? ShowMessageBox(owner, text, severity, MessageBoxButtons.YesNoCancel);
    }

    /// <summary>Displays a message using a <see cref="MessageBox"/>.</summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    /// <param name="buttons">The buttons the user can click.</param>
    private static DialogResult ShowMessageBox(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity, MessageBoxButtons buttons)
    {
        // Handle RTL systems
        MessageBoxOptions localizedOptions;
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft) localizedOptions = MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;
        else localizedOptions = 0;

        // Select icon based on message severity
        var icon = severity switch
        {
            MsgSeverity.Warn => MessageBoxIcon.Warning,
            MsgSeverity.Error => MessageBoxIcon.Error,
            _ => MessageBoxIcon.Information
        };

        // Default to the negative option ("No" or "Cancel") for warnings and errors
        var defaultButton = (severity >= MsgSeverity.Warn && buttons != MessageBoxButtons.OK)
            ? MessageBoxDefaultButton.Button2
            : MessageBoxDefaultButton.Button1;

        // Display MessageDialog
        return MessageBox.Show(owner, text, Application.ProductName, buttons, icon, defaultButton, localizedOptions);
    }

    private static DialogResult? ShowTaskDialog(IWin32Window? owner, string text, MsgSeverity severity, string? yes = null, string? no = null, string? ok = null, string? cancel = null, bool canCancel = false)
    {
        if (!WindowsUtils.IsWindowsVista) return null;

        var icon = severity switch
        {
            MsgSeverity.Warn => TaskDialogIcon.Warning,
            MsgSeverity.Error => TaskDialogIcon.Error,
            _ => TaskDialogIcon.Information
        };

        string[] splitText = text.Replace("\r\n", "\n").Split(['\n'], 2);
        string heading = splitText[0];
        string? details = (splitText.Length == 2) ? splitText[1] : null;

        var buttons = new List<TaskDialogButton>();

        void TryAddButton(DialogResult result, string? caption)
        {
#if NET
            if (string.IsNullOrEmpty(caption)) return;
            string[] captionSplit = caption.Replace("\r\n", "\n").Split(['\n'], 2);
            buttons.Add(new TaskDialogCommandLinkButton(captionSplit[0], (captionSplit.Length == 2) ? captionSplit[1] : null) {Tag = result});
#else
            if (!string.IsNullOrEmpty(caption))
                buttons.Add(new TaskDialogButton((int)result, caption.Replace("\r\n", "\n")));
#endif
        }

        TryAddButton(DialogResult.Yes, yes);
        TryAddButton(DialogResult.No, no);
        TryAddButton(DialogResult.OK, ok);

#if NET
        if (canCancel)
        {
            if (string.IsNullOrEmpty(cancel)) buttons.Add(TaskDialogButton.Cancel);
            else TryAddButton(DialogResult.Cancel, cancel);
        }

        var taskDialog = new TaskDialogPage
        {
            Caption = Application.ProductName,
            Icon = icon,
            Heading = heading,
            Text = details,
            AllowCancel = canCancel,
            RightToLeftLayout = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
        };
        taskDialog.Buttons.AddRange(buttons);
        if (severity >= MsgSeverity.Warn)
        {
            taskDialog.DefaultButton = buttons.FirstOrDefault(x => x.Tag is DialogResult.No)
                                    ?? buttons.FirstOrDefault(x => x.Tag is DialogResult.Cancel or null); // Standard Cancel button has no tag
        }

        try
        {
            var pushedButton = (owner == null)
                ? TaskDialog.ShowDialog(taskDialog)
                : TaskDialog.ShowDialog(owner, taskDialog);
            return pushedButton.Tag is DialogResult tag ? tag : DialogResult.Cancel;
        }
        #region Error handling
        catch (Exception ex) when (ex is COMException or BadImageFormatException or EntryPointNotFoundException or ArgumentException or InvalidOperationException)
        {
            Log.Info("Failed to show TaskDialog, falling back to MessageBox", ex);
            return null;
        }
        #endregion
#else
        var taskDialog = new TaskDialog.TaskDialog
        {
            PositionRelativeToWindow = true,
            WindowTitle = Application.ProductName,
            MainIcon = icon,
            MainInstruction = heading,
            Content = details,
            RightToLeftLayout = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
        };

        if (canCancel)
        {
            taskDialog.AllowDialogCancellation = true;
            if (string.IsNullOrEmpty(cancel))
                taskDialog.CommonButtons = TaskDialogCommonButtons.Cancel;
            else
                TryAddButton(DialogResult.Cancel, cancel);
        }
        if (buttons.Count != 0)
        {
            taskDialog.UseCommandLinks = true;
            taskDialog.Buttons = buttons.ToArray();
        }
        if (severity >= MsgSeverity.Warn && (canCancel || !string.IsNullOrEmpty(no)))
            taskDialog.DefaultButton = (int)(string.IsNullOrEmpty(no) ? DialogResult.Cancel : DialogResult.No);

        try
        {
            int result = owner == null ? taskDialog.Show() : taskDialog.Show(owner);
            return (DialogResult)result;
        }
        #region Error handling
        catch (Exception ex) when (ex is COMException or BadImageFormatException or EntryPointNotFoundException or ArgumentException or InvalidOperationException)
        {
            Log.Info("Failed to show TaskDialog, falling back to MessageBox", ex);
            return null;
        }
        #endregion
#endif
    }
}
