// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Info;
using NanoByte.Common.Streams;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Shared building blocks for simple modal boxes like <see cref="ErrorBox"/> and <see cref="OutputBox"/>.
/// </summary>
internal static class BoxLayout
{
    /// <summary>
    /// The name of the application, for use as a window title.
    /// </summary>
    public static string? AppName
        => string.IsNullOrEmpty(Application.Instance?.Name)
            ? AppInfo.Current.ProductName ?? AppInfo.Current.Name
            : Application.Instance.Name;

    /// <summary>
    /// The font to use for regular text.
    /// </summary>
    public static Font BodyFont => SystemFonts.Message();

    /// <summary>
    /// Creates a label for regular, wrapped text.
    /// </summary>
    public static Label Text([Localizable(true)] string text)
        => new Label
        {
            Text = text,
            Font = BodyFont,
            Wrap = WrapMode.Word
        }.ReflowOnResize();

    /// <summary>
    /// Creates a row with a large icon next to a heading.
    /// </summary>
    /// <param name="iconName">The name of the embedded PNG image to use as the icon, without the file extension.</param>
    /// <param name="heading">The text of the heading.</param>
    /// <param name="headingLabel">Returns the label displaying the heading.</param>
    public static TableLayout HeaderRow([Localizable(false)] string iconName, [Localizable(true)] string heading, out Label headingLabel)
    {
        headingLabel = Heading(heading);
        return new TableLayout
        {
            Spacing = new(10, 0),
            Rows =
            {
                new TableRow(
                    new TableCell(Icon(iconName)),
                    new TableCell(headingLabel, scaleWidth: true)
                )
            }
        };
    }

    private static Label Heading([Localizable(true)] string text)
        => new Label
        {
            Text = text,
            Font = SystemFonts.Message(BodyFont.Size * 4 / 3),
            Wrap = WrapMode.Word
        }.ReflowOnResize();

    private static Control Icon([Localizable(false)] string name)
    {
        try
        {
            using var stream = typeof(BoxLayout).GetEmbeddedStream(name + ".png");
            return new ImageView {Image = new Bitmap(stream)};
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug($"Failed to load {name} icon", ex);
            return new Panel();
        }
        #endregion
    }

    /// <summary>
    /// Calculates the size needed to display a block of <see cref="BodyFont"/> text.
    /// </summary>
    /// <param name="columns">The number of average characters per line.</param>
    /// <param name="lines">The number of lines.</param>
    /// <remarks>Use this instead of fixed pixel sizes, so that windows scale with the system font.</remarks>
    public static Size TextSize(int columns, int lines)
    {
        const string sample = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var font = BodyFont;
        return new(
            (int)Math.Ceiling(font.MeasureString(sample).Width / sample.Length * columns),
            (int)Math.Ceiling(font.LineHeight * lines));
    }

    extension(Dialog dialog)
    {
        /// <summary>
        /// Adds an OK button that closes the dialog.
        /// </summary>
        public void AddCloseButton()
        {
            var closeButton = new Button {Text = Resources.OK};
            closeButton.Click += delegate { dialog.Close(); };
            dialog.PositiveButtons.Add(closeButton);
            dialog.DefaultButton = closeButton;
            dialog.AbortButton = closeButton;
        }
    }

    extension(Label label)
    {
        /// <summary>
        /// Adds a context menu for copying a label's text to the clipboard.
        /// </summary>
        public Label WithCopyMenu()
        {
            var copyItem = new ButtonMenuItem {Text = Resources.CopyToClipboard};
            copyItem.Click += delegate
            {
                using var clipboard = new Clipboard();
                clipboard.Text = label.Text;
            };
            label.ContextMenu = new ContextMenu(copyItem);
            return label;
        }
    }

    extension(Dialog dialog)
    {
        /// <summary>
        /// Shows a dialog modally, centered on the screen if there is no <paramref name="owner"/>.
        /// </summary>
        public void ShowBox(Control? owner)
        {
            dialog.ShowInTaskbar = owner == null;
            dialog.CenterOnScreenIfOwnerless();
            dialog.PreventPinning();

            if (owner == null) dialog.ShowModal();
            else dialog.ShowModal(owner);
        }
    }
}
