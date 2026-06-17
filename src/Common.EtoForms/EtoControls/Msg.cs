// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Provides easier access to typical <see cref="MessageBox"/> configurations.
/// </summary>
public static class Msg
{
    /// <summary>
    /// Displays a message to the user using a message box.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    public static void Inform(Control? owner, [Localizable(true)] string text, MsgSeverity severity)
        => Show(owner, text, MessageBoxButtons.OK, severity);

    /// <summary>
    /// Asks the user a OK/Cancel-question using a message box.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    /// <returns><c>true</c> if the user selected OK; <c>false</c> if the user selected Cancel.</returns>
    public static bool OkCancel(Control? owner, [Localizable(true)] string text, MsgSeverity severity)
        => Show(owner, text, MessageBoxButtons.OKCancel, severity) == DialogResult.Ok;

    /// <summary>
    /// Asks the user to choose between two options (yes/no) using a message box.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    /// <returns><c>true</c> if the user chose 'Yes', <c>false</c> if the user chose 'No'.</returns>
    public static bool YesNo(Control? owner, [Localizable(true)] string text, MsgSeverity severity)
        => Show(owner, text, MessageBoxButtons.YesNo, severity) == DialogResult.Yes;

    /// <summary>
    /// Asks the user to choose between three options (yes/no/cancel) using a message box.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="text">The message to be displayed.</param>
    /// <param name="severity">How severe/important the message is.</param>
    /// <returns><see cref="DialogResult.Yes"/>, <see cref="DialogResult.No"/> or <see cref="DialogResult.Cancel"/>.</returns>
    public static DialogResult YesNoCancel(Control? owner, [Localizable(true)] string text, MsgSeverity severity)
        => Show(owner, text, MessageBoxButtons.YesNoCancel, severity);

    private static DialogResult Show(Control? owner, [Localizable(true)] string text, MessageBoxButtons buttons, MsgSeverity severity)
    {
        #region Sanity checks
        if (text == null) throw new ArgumentNullException(nameof(text));
        #endregion

        var type = severity switch
        {
            MsgSeverity.Warn => MessageBoxType.Warning,
            MsgSeverity.Error => MessageBoxType.Error,
            _ => MessageBoxType.Information
        };

        return owner == null
            ? MessageBox.Show(text, buttons, type)
            : MessageBox.Show(owner, text, buttons, type);
    }
}
