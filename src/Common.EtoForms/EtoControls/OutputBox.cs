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
    /// <param name="title">The text to display above the <paramref name="message"/>.</param>
    /// <param name="message">The selectable multi-line text to display to the user.</param>
    public static void Show(Control? owner, [Localizable(true)] string title, [Localizable(true)] string message)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (message == null) throw new ArgumentNullException(nameof(message));
        #endregion

        var textArea = new TextArea
        {
            ReadOnly = true,
            Wrap = true,
            Font = BoxLayout.BodyFont,
            Text = message.ToNativeNewLine().TrimEnd(Environment.NewLine.ToCharArray())
        };
        textArea.Load += delegate { textArea.CaretIndex = 0; }; // Scroll to top

        using var dialog = new Dialog
        {
            Title = BoxLayout.AppName,
            Padding = 10,
            Resizable = true,
            Content = new StackLayout
            {
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Spacing = 8,
                Items =
                {
                    BoxLayout.HeaderRow("Info", title, out _),
                    new StackLayoutItem(textArea, expand: true)
                }
            }
        };
        dialog.AddCloseButton();
        dialog.SetInitialClientSize(BoxLayout.TextSize(columns: 80, lines: 20));

        dialog.ShowBox(owner);
    }
}
