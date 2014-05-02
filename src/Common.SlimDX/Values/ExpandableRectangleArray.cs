/*
 * Copyright 2006-2014 Bastian Eicher
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
using System.Collections.Generic;
using System.Drawing;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Builds a rectangle array that can grow in any direction.
    /// </summary>
    /// <typeparam name="T">The type of elements to store in the array.</typeparam>
    public class ExpandableRectangleArray<T>
        where T : struct
    {
        #region Structures
        private struct Subset
        {
            public readonly Point Start;
            public readonly T[,] Array;

            public Subset(Point start, T[,] array)
            {
                Start = start;
                Array = array;
            }
        }
        #endregion

        #region Variables
        /// <summary>Maintains a list of all <see cref="Subset"/>s inserted so far.</summary>
        private readonly LinkedList<Subset> _subsets = new LinkedList<Subset>();
        #endregion

        #region Properties
        /// <summary>
        /// The total area the rectangle currently encompasses.
        /// </summary>
        public Rectangle TotalArea { get; private set; }
        #endregion

        //--------------------//

        #region Add
        /// <summary>
        /// Insert a new subset array before any existing entries growing the rectangle as necessary.
        /// </summary>
        /// <param name="start">The top-left coordinates of the area to insert the array into.</param>
        /// <param name="array">The array to insert. Do not modify this array after calling this method!</param>
        /// <returns>Values in negative areas are clipped away.</returns>
        public void AddFirst(Point start, T[,] array)
        {
            #region Sanity checks
            if (array == null) throw new ArgumentNullException("array");
            #endregion

            _subsets.AddFirst(new Subset(start, array));
            ExpandArea(new Rectangle(start, new Size(array.GetLength(0), array.GetLength(1))));
        }

        /// <summary>
        /// Insert a new subset array after any existing entries growing the rectangle as necessary.
        /// </summary>
        /// <param name="start">The top-left coordinates of the area to insert the array into.</param>
        /// <param name="array">The array to insert. Do not modify this array after calling this method!</param>
        /// <returns>Values in negative areas are clipped away.</returns>
        public void AddLast(Point start, T[,] array)
        {
            #region Sanity checks
            if (array == null) throw new ArgumentNullException("array");
            #endregion

            _subsets.AddLast(new Subset(start, array));
            ExpandArea(new Rectangle(start, new Size(array.GetLength(0), array.GetLength(1))));
        }

        /// <summary>
        /// Updates <see cref="TotalArea"/> to accomodate a new subset.
        /// </summary>
        private void ExpandArea(Rectangle subsetArea)
        {
            // Clip away negative areas
            if (subsetArea.X < 0)
            {
                subsetArea.Width += subsetArea.X;
                subsetArea.X = 0;
            }
            if (subsetArea.Y < 0)
            {
                subsetArea.Height += subsetArea.Y;
                subsetArea.Y = 0;
            }

            TotalArea = (TotalArea == default(Rectangle)) ? subsetArea : Rectangle.Union(TotalArea, subsetArea);
        }
        #endregion

        #region Get array
        /// <summary>
        /// Returns the smallest possible array that encompasses all inserted subsets. Blanks between subsets are filled with the default value for <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An array containing the copyed data. Size will be determined by <see cref="TotalArea"/>.</returns>
        public T[,] GetArray()
        {
            // Create a result array filled with default values
            var result = new T[TotalArea.Width, TotalArea.Height];

            CopySubsetsToArray(result);
            return result;
        }

        /// <summary>
        /// Returns the smallest possible array that encompasses all inserted subsets and can be backed by a base array.
        /// </summary>
        /// <param name="baseValues">An array to query for values to fill the blanks left between subsets.</param>
        /// <returns>An array containing the copyed data. Size will be trimmed if <paramref name="baseValues"/> is too small.</returns>
        public T[,] GetArray(T[,] baseValues)
        {
            #region Sanity checks
            if (baseValues == null) throw new ArgumentNullException("baseValues");
            #endregion

            // Build the result array and fill it with base values if possible
            int width = Math.Min(TotalArea.Width, baseValues.GetLength(0) - TotalArea.X);
            int height = Math.Min(TotalArea.Height, baseValues.GetLength(1) - TotalArea.Y);
            var result = new T[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    result[x, y] = baseValues[x + TotalArea.X, y + TotalArea.Y];
            }

            CopySubsetsToArray(result);
            return result;
        }

        /// <summary>
        /// Returns the smallest possible array that encompasses all inserted subsets and can be backed by a base array.
        /// </summary>
        /// <param name="baseValues">An array to query for values to fill the blanks left between subsets.</param>
        /// <returns>An array containing the copyed data. Size will be trimmed if <paramref name="baseValues"/> is too small.</returns>
        public T[,] GetArray(Grid<T> baseValues)
        {
            #region Sanity checks
            if (baseValues == null) throw new ArgumentNullException("baseValues");
            #endregion

            return GetArray(baseValues.Data);
        }

        /// <summary>
        /// Copies each subset array to a result array.
        /// </summary>
        private void CopySubsetsToArray(T[,] result)
        {
            foreach (var subset in _subsets)
            {
                var offset = subset.Start - new Size(TotalArea.X, TotalArea.Y);

                // Iterate through intersection of [0,subset.Array) and [-offset,result-offset)
                for (int x = Math.Max(0, -offset.X); x < Math.Min(subset.Array.GetLength(0), result.GetLength(0) - offset.X); x++)
                {
                    for (int y = Math.Max(0, -offset.Y); y < Math.Min(subset.Array.GetLength(1), result.GetLength(1) - offset.Y); y++)
                        result[x + offset.X, y + offset.Y] = subset.Array[x, y];
                }
            }
        }
        #endregion
    }
}
