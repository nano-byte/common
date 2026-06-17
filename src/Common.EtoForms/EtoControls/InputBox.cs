// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Shows a simple dialog asking the user to input some text.
/// </summary>
public static class InputBox
{
    /// <summary>
    /// Displays an input box asking the user to input some text.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="title">The window title to use.</param>
    /// <param name="prompt">The prompt to display.</param>
    /// <param name="defaultText">The default text to show pre-entered in the input field.</param>
    /// <param name="password">Shall the input characters be hidden as a password?</param>
    /// <returns>The text the user entered if they pressed OK; otherwise <c>null</c>.</returns>
    public static string? Show(Control? owner, [Localizable(true)] string title, [Localizable(true)] string prompt, [Localizable(true)] string? defaultText = null, bool password = false)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
        if (string.IsNullOrEmpty(prompt)) throw new ArgumentNullException(nameof(prompt));
        #endregion

        Control input;
        Func<string?> getText;
        if (password)
        {
            var box = new PasswordBox {Text = defaultText};
            input = box;
            getText = () => box.Text;
        }
        else
        {
            var box = new TextBox {Text = defaultText};
            input = box;
            getText = () => box.Text;
        }

        using var dialog = new Dialog<string?>
        {
            Title = title,
            Padding = 10,
            Width = 400,
            Content = new StackLayout
            {
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Spacing = 8,
                Items = {new Label {Text = prompt.Replace("\n", Environment.NewLine)}, input}
            }
        };

        var okButton = new Button {Text = Resources.OK};
        okButton.Click += delegate
        {
            dialog.Result = getText();
            dialog.Close();
        };
        var cancelButton = new Button {Text = Resources.Cancel};
        cancelButton.Click += delegate
        {
            dialog.Result = null;
            dialog.Close();
        };

        dialog.PositiveButtons.Add(okButton);
        dialog.NegativeButtons.Add(cancelButton);
        dialog.DefaultButton = okButton;
        dialog.AbortButton = cancelButton;

        return owner == null ? dialog.ShowModal() : dialog.ShowModal(owner);
    }
}
