// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// Shows a simple dialog asking the user to input some text.
/// </summary>
public sealed partial class InputBox : Form
{
    private InputBox()
    {
        InitializeComponent();
        buttonOK.Text = Resources.OK;
        buttonCancel.Text = Resources.Cancel;
    }

    /// <summary>
    /// Displays an input box asking the the user to input some text.
    /// </summary>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="title">The window title to use.</param>
    /// <param name="prompt">The prompt to display.</param>
    /// <param name="defaultText">The default text to show pre-entered in the input field.</param>
    /// <param name="password">Shall the input characters be hidden as a password?</param>
    /// <returns>The text the user entered if she pressed OK; otherwise <c>null</c>.</returns>
    public static string? Show(IWin32Window? owner, [Localizable(true)] string title, [Localizable(true)] string prompt, [Localizable(true)] string? defaultText = null, bool password = false)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
        if (string.IsNullOrEmpty(prompt)) throw new ArgumentNullException(nameof(prompt));
        #endregion

        using var inputBox = new InputBox
        {
            Text = title,
            labelPrompt = {Text = prompt.Replace("\n", Environment.NewLine)},
            textInput = {Text = defaultText, UseSystemPasswordChar = password},
            ShowInTaskbar = (owner == null)
        };
        return (inputBox.ShowDialog(owner) == DialogResult.OK) ? inputBox.textInput.Text : null;
    }

    private void InputBox_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data!.GetDataPresent(DataFormats.FileDrop))
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            textInput.Text = (files ?? new string[0]).FirstOrDefault();
        }
        else if (e.Data.GetDataPresent(DataFormats.Text))
            textInput.Text = (string)e.Data.GetData(DataFormats.Text);
    }

    private void InputBox_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = (e.Data!.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop))
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }
}