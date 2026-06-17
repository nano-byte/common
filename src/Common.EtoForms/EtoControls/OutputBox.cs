// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// A simple dialog displaying selectable multi-line text.
/// </summary>
public static class OutputBox
{
    /// <summary>
    /// Displays an output box with some text.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="title">The window title to use.</param>
    /// <param name="message">The selectable multi-line text to display to the user.</param>
    public static void Show(Control? owner, [Localizable(true)] string title, [Localizable(true)] string message)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (message == null) throw new ArgumentNullException(nameof(message));
        #endregion

        using var dialog = new Dialog
        {
            Title = title,
            Padding = 10,
            Width = 600,
            Height = 400,
            Content = new TextArea
            {
                ReadOnly = true,
                Text = message.Replace("\n", Environment.NewLine)
            }
        };

        var closeButton = new Button {Text = Resources.OK};
        closeButton.Click += delegate { dialog.Close(); };
        dialog.PositiveButtons.Add(closeButton);
        dialog.DefaultButton = closeButton;
        dialog.AbortButton = closeButton;

        if (owner == null) dialog.ShowModal();
        else dialog.ShowModal(owner);
    }
}
