// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Net;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// A <see cref="TextBox"/> designed specifically for entering URIs.
/// </summary>
/// <remarks>Will turn red for invalid input and green for valid input. Accepts dropped files and URIs.</remarks>
public class UriTextBox : TextBox
{
    /// <summary>
    /// Creates a new URI text box.
    /// </summary>
    public UriTextBox()
    {
        AllowDrop = true;
        TextChanged += delegate { TextColor = IsValid ? Colors.Green : Colors.Red; };
    }

    /// <summary>
    /// The <see cref="Uri"/> represented by this text box.
    /// </summary>
    /// <exception cref="UriFormatException">Trying to read while <see cref="TextControl.Text"/> is not a well-formed <see cref="Uri"/>.</exception>
    /// <remarks>It is always safe to set this property. It is safe to read this property if <see cref="IsValid"/> is <c>true</c>.</remarks>
    public Uri? Uri
    {
        get => string.IsNullOrEmpty(Text) ? null : new Uri(Text, AllowRelative ? UriKind.RelativeOrAbsolute : UriKind.Absolute);
        set => Text = value?.ToStringRfc() ?? "";
    }

    /// <summary>
    /// When set to <c>true</c> only URIs starting with "http:" or "https:" will be considered valid.
    /// </summary>
    [DefaultValue(false), Description("""When set to true only URIs starting with "http:" or "https:" will be considered valid.""")]
    public bool HttpOnly { get; set; }

    /// <summary>
    /// When set to <c>true</c> relative URIs are accepted.
    /// </summary>
    [DefaultValue(false), Description("""When set to true relative URIs are accepted.""")]
    public bool AllowRelative { get; set; }

    /// <summary>
    /// Indicates whether the currently entered text is a valid URI.
    /// </summary>
    public bool IsValid => ValidateUri(Text);

    /// <summary>
    /// Checks if a text represents a valid <see cref="Uri"/>.
    /// </summary>
    /// <param name="text">Text to check.</param>
    protected virtual bool ValidateUri(string? text)
    {
        // Allow empty input
        if (string.IsNullOrEmpty(text)) return true;

        // Check URI is well-formed
        if (!Uri.TryCreate(text, AllowRelative ? UriKind.RelativeOrAbsolute : UriKind.Absolute, out var uri)) return false;

        // Check URI is HTTP(S) if that was requested
        if (HttpOnly && uri.IsAbsoluteUri) return uri.Scheme == "http" || uri.Scheme == "https";

        return true;
    }

    /// <inheritdoc/>
    protected override void OnDragEnter(DragEventArgs e)
    {
        #region Sanity checks
        if (e == null) throw new ArgumentNullException(nameof(e));
        #endregion

        SetDragEffects(e);
        base.OnDragEnter(e);
    }

    /// <inheritdoc/>
    protected override void OnDragOver(DragEventArgs e)
    {
        #region Sanity checks
        if (e == null) throw new ArgumentNullException(nameof(e));
        #endregion

        SetDragEffects(e);
        base.OnDragOver(e);
    }

    /// <inheritdoc/>
    protected override void OnDragDrop(DragEventArgs e)
    {
        #region Sanity checks
        if (e == null) throw new ArgumentNullException(nameof(e));
        #endregion

        Text = GetDropText(e);
        base.OnDragDrop(e);
    }

    private void SetDragEffects(DragEventArgs e)
        => e.Effects = ValidateUri(GetDropText(e)) ? DragEffects.Copy : DragEffects.None;

    /// <summary>
    /// Returns the text dropped on a control. Uses the local path for files.
    /// </summary>
    private static string GetDropText(DragEventArgs e)
    {
        if (e.Data.ContainsUris && e.Data.Uris is [var uri, ..])
            return uri.IsFile ? uri.LocalPath : uri.OriginalString;
        if (e.Data.ContainsText)
            return e.Data.Text ?? "";

        return "";
    }
}
