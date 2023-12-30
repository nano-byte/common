// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Drawing;

/// <summary>
/// Wraps an <see cref="Image"/> and provides and caches scaled versions of it.
/// </summary>
/// <param name="image">The base image.</param>
public sealed class ScalableImage(Image image) : IDisposable
{
    private readonly Dictionary<SizeF, Image> _scaledImages = [];

    /// <summary>
    /// Returns a copy of the base image scaled by the specified <paramref name="factor"/>.
    /// </summary>
    public Image Get(SizeF factor)
        => factor == new SizeF(1, 1)
            ? image
            : _scaledImages.GetOrAdd(factor, () => image.Scale(factor));

    /// <summary>
    /// Disposes the base image and any scaled images returned by <see cref="Get"/>.
    /// </summary>
    public void Dispose()
    {
        image.Dispose();
        foreach (var img in _scaledImages.Values)
            img.Dispose();
    }
}
