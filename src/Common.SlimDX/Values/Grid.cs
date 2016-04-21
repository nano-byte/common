/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// A 2D grid of values that can be stored in PNG files.
    /// </summary>
    /// <typeparam name="T">The type of values stored in the grid.</typeparam>
    public abstract class Grid<T>
        where T : struct
    {
        /// <summary>
        /// The internal array containing the values.
        /// </summary>
        [NotNull]
        protected internal readonly T[,] Data;

        /// <summary>
        /// The width of the grid (number of values along the X axis).
        /// </summary>
        public int Width { get { return Data.GetLength(0); } }

        /// <summary>
        /// The height of the grid (number of values along the Y axis).
        /// </summary>
        public int Height { get { return Data.GetLength(1); } }

        /// <summary>
        /// Creates a new empty grid.
        /// </summary>
        /// <param name="width">The width of the grid (number of values along the X axis).</param>
        /// <param name="height">The height of the grid (number of values along the Y axis).</param>
        protected Grid(int width, int height)
        {
            Data = new T[width, height];
        }

        /// <summary>
        /// Creates a new grid based on an existing array.
        /// </summary>
        /// <param name="data">Used as the internal array (no defensive copy). Do not modify once passing in!</param>
        protected Grid(T[,] data)
        {
            Data = data;
        }

        /// <summary>
        /// Gets or sets a value in the grid.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"><paramref name="x"/> or <paramref name="y"/> are out of bounds.</exception>
        public virtual T this[int x, int y] { get { return Data[x, y]; } set { Data[x, y] = value; } }

        /// <summary>
        /// Reads a value in the grid and automatically clamps out of bound values of <paramref name="x"/> or <paramref name="y"/>.
        /// </summary>
        public T ClampedRead(int x, int y)
        {
            return Data[x.Clamp(0, Data.GetUpperBound(0)), y.Clamp(0, Data.GetUpperBound(1))];
        }

        /// <summary>
        /// Saves the grid to a PNG file.
        /// </summary>
        public void Save([NotNull, Localizable(false)] string path)
        {
            using (var bitmap = GenerateBitmap())
                bitmap.Save(path, ImageFormat.Png);
        }

        /// <summary>
        /// Saves the grid to a PNG stream.
        /// </summary>
        public void Save([NotNull] Stream stream)
        {
            // NOTE: Use intermediate RAM buffer because writing a PNG directly to a ZIP won't work
            using (var bitmap = GenerateBitmap())
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.CopyToEx(stream);
            }
        }

        /// <summary>
        /// Generates a bitmap representation of the grid.
        /// </summary>
        public abstract Bitmap GenerateBitmap();
    }
}
