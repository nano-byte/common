// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Net;

namespace NanoByte.Common.Controls;

/// <summary>
/// A <see cref="HintTextBox"/> designed specifically for entering URIs.
/// </summary>
/// <remarks>Will turn red for invalid input and green for valid input. Will not allow focus to be lost for invalid input.</remarks>
[Description("""A HintTextBox designed specifically for entering URIs.""")]
public class UriTextBox : HintTextBox
{
    #region Events
    /// <inheritdoc/>
    protected override void OnDragEnter(DragEventArgs drgevent)
    {
        #region Sanity checks
        if (drgevent == null) throw new ArgumentNullException(nameof(drgevent));
        #endregion

        drgevent.Effect = ValidateUri(GetDropText(drgevent))
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        base.OnDragEnter(drgevent);
    }

    /// <inheritdoc/>
    protected override void OnDragDrop(DragEventArgs drgevent)
    {
        Text = GetDropText(drgevent);

        base.OnDragDrop(drgevent);
    }
    #endregion

    #region Properties
    /// <summary>
    /// The <see cref="Uri"/> represented by this text box.
    /// </summary>
    /// <exception cref="UriFormatException">Trying to read while <see cref="TextBox.Text"/> is not a well-formed <see cref="Uri"/>.</exception>
    /// <remarks>It is always safe to set this property. It is safe to read this property after validation has been performed.</remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Uri? Uri { get => string.IsNullOrEmpty(Text) ? null : new Uri(Text, AllowRelative ? UriKind.RelativeOrAbsolute : UriKind.Absolute); set => Text = value?.ToStringRfc() ?? ""; }

    /// <summary>
    /// When set to <c>true</c> only URIs starting with "http:" or "https:" will be considered valid.
    /// </summary>
    [DefaultValue(false), Description("""When set to true only URIs starting with "http:" or "https:" will be considered valid."""), Category("Behavior")]
    public bool HttpOnly { get; set; }

    /// <summary>
    /// When set to <c>true</c> relative URIs are accepted.
    /// </summary>
    [DefaultValue(false), Description("""When set to true relative URIs are accepted."""), Category("Behavior")]
    public bool AllowRelative { get; set; }

    /// <summary>
    /// Indicates whether the currently entered text is a valid URI.
    /// </summary>
    public bool IsValid => ValidateUri(Text);
    #endregion

    #region Constructor
    public UriTextBox()
    {
        // Use event instead of method override to ensure special handling of HintText works
        TextChanged += delegate { ForeColor = ValidateUri(Text) ? Color.Green : Color.Red; };
    }
    #endregion

    //--------------------//

    #region Helpers
    /// <summary>
    /// Checks if a text represents a valid <see cref="Uri"/>.
    /// </summary>
    /// <param name="text">Text to check.</param>
    protected virtual bool ValidateUri(string text)
    {
        // Allow empty input
        if (string.IsNullOrEmpty(text)) return true;

        // Check URI is well-formed
        if (!Uri.TryCreate(text, AllowRelative ? UriKind.RelativeOrAbsolute : UriKind.Absolute, out var uri)) return false;

        // Check URI is HTTP(S) if that was requested
        if (HttpOnly && uri.IsAbsoluteUri) return uri.Scheme == "http" || uri.Scheme == "https";

        return true;
    }

    /// <summary>
    /// Returns the text dropped on a control.
    /// </summary>
    /// <param name="dragEventArgs">Argument of a dragging operation.</param>
    /// <returns>The dropped text.</returns>
    private static string GetDropText(DragEventArgs dragEventArgs)
    {
        if (dragEventArgs.Data!.GetDataPresent(DataFormats.FileDrop))
        {
            var files = dragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            return (files ?? []).FirstOrDefault() ?? "";
        }
        if (dragEventArgs.Data.GetDataPresent(DataFormats.Text))
            return (string)dragEventArgs.Data.GetData(DataFormats.Text)!;

        return "";
    }
    #endregion
}
