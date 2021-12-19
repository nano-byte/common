// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing.Drawing2D;

namespace NanoByte.Common.Drawing
{
    /// <summary>
    /// Provides extension methods for <see cref="Image"/>s.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Scales an image by a linear <paramref name="factor"/>.
        /// </summary>
        public static Image Scale(this Image image, float factor)
        {
            #region Sanity checks
            if (image == null) throw new ArgumentNullException(nameof(image));
            #endregion

            var scaledSize = image.Size.Multiply(factor);
            var scaledImage = new Bitmap(scaledSize.Width, scaledSize.Height);

            using var graphics = Graphics.FromImage(scaledImage);
            graphics.InterpolationMode = factor >= 2
                ? InterpolationMode.NearestNeighbor // looks better for large factors
                : InterpolationMode.HighQualityBicubic; // looks better for small factors
            graphics.DrawImage(image, new Rectangle(new Point(), scaledSize));

            return scaledImage;
        }
    }
}
