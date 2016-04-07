/*
 * Copyright 2006-2016 Bastian Eicher
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

#if NET45
namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Provides extension methods for <see cref="IProgress{T}"/>s.
    /// </summary>
    public static class ProgressExtensions
    {
        /// <summary>
        /// Converts a NanoByte.Common progress handler to a regular .NET progress handler.
        /// </summary>
        public static System.IProgress<T> ToSystem<T>(IProgress<T> progress)
        {
            return new System.Progress<T>(progress.Report);
        }

        /// <summary>
        /// Converts a regular .NET progress handler to a NanoByte.Common progress handler.
        /// </summary>
        public static IProgress<T> ToNanoByte<T>(System.IProgress<T> progress)
        {
            return new Progress<T>(progress.Report);
        }
    }
}
#endif
