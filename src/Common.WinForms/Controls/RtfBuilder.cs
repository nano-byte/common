// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Text;

namespace NanoByte.Common.Controls;

/// <seealso cref="RtfBuilder.AppendPar"/>
public enum RtfColor
{
    Black = 0,
    Blue = 1,
    Green = 2,
    Yellow = 3,
    Orange = 4,
    Red = 5
}

/// <summary>
/// Builds an RTF-formatted string with paragraphs.
/// </summary>
public sealed class RtfBuilder
{
    private readonly StringBuilder _builder = new();

    /// <summary>
    /// Indicates whether the builder is currently empty (contains no paragraphs).
    /// </summary>
    public bool IsEmpty => _builder.Length == 0;

    /// <summary>
    /// Appends a new paragraph.
    /// </summary>
    /// <param name="text">The text in the paragraph.</param>
    /// <param name="color">The color of the text.</param>
    public void AppendPar(string text, RtfColor color)
    {
        string escapedText = (text ?? throw new ArgumentNullException(nameof(text)))
              .Replace(@"\", @"\\")
              .Replace(Environment.NewLine, "\\par\n")
              .Replace("\n", "\\par\n");
        _builder.AppendLine($"\\cf{(int)color + 1} {escapedText}\\par\\par\n");
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{{\\rtf1\r\n{{\\colortbl ;\\red0\\green0\\blue0;\\red0\\green0\\blue255;\\red0\\green255\\blue0;\\red255\\green255\\blue0;\\red255\\green106\\blue0;\\red255\\green0\\blue0;}}\r\n{_builder}}}";
}
