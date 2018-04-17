// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// A 2D grid of <see cref="byte"/> values that can be stored in grayscale PNG files.
    /// </summary>
    public class ByteGrid : Grid<byte>
    {
        /// <inheritdoc/>
        public ByteGrid(int width, int height)
            : base(width, height)
        {}

        /// <inheritdoc/>
        public ByteGrid(byte[,] data)
            : base(data)
        {}

        /// <summary>
        /// Performs bilinear interpolation to get values lying between discrete grid points.
        /// </summary>
        public float SampledRead(float x, float y)
        {
            int xFloor = (int)Math.Floor(x);
            float xFactor = x - xFloor;

            int yFloor = (int)Math.Floor(y);
            float yFactor = y - yFloor;

            float x1 = ClampedRead(xFloor, yFloor);
            float x2 = ClampedRead(xFloor + 1, yFloor);
            float xLinear1 = (1 - xFactor) * x1 + xFactor * x2;
            float xLinear2 = (1 - xFactor) * ClampedRead(xFloor, yFloor + 1) + xFactor * ClampedRead(xFloor + 1, yFloor + 1);

            return (1 - yFactor) * xLinear1 + yFactor * xLinear2;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified coordinates are within the bounds of the grid.
        /// </summary>
        public bool IsInRange(float x, float y) => x >= 0 && x < Width && y >= 0 && y < Height;

        /// <summary>
        /// Loads a grid from a PNG stream.
        /// </summary>
        public static ByteGrid Load([NotNull] Stream stream)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            try
            {
                using (var bitmap = (Bitmap)Image.FromStream(stream))
                {
                    var data = new byte[bitmap.Width, bitmap.Height];
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                            data[x, y] = bitmap.GetPixel(x, y).R;
                    }
                    return new ByteGrid(data);
                }
            }
            #region Error handling
            catch (ArgumentException ex)
            {
                throw new IOException(Resources.NotAnImage, ex);
            }
            #endregion
        }

        /// <inheritdoc/>
        public override unsafe Bitmap GenerateBitmap()
        {
            var bitmap = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

            // NOTE: Must use Palette property setter, otherwise changes do not persist
            var palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            bitmap.Palette = palette;

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            try
            {
                var pointer = (byte*)bitmapData.Scan0;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                        *(pointer + x) = Data[x, y];
                    pointer += bitmapData.Stride;
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            return bitmap;
        }
    }
}
