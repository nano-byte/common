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
using JetBrains.Annotations;
using NanoByte.Common.Values.Design;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Stores the file type describing the kind of data a property stores.
    /// Controls the behaviour of <see cref="CodeEditor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [PublicAPI]
    public sealed class FileTypeAttribute : Attribute
    {
        private readonly string _fileType;

        /// <summary>
        /// The name of the file type (e.g. XML, JavaScript, Lua).
        /// </summary>
        public string FileType { get { return _fileType; } }

        /// <summary>
        /// Creates a new file type attribute.
        /// </summary>
        /// <param name="fileType">The name of the file type (e.g. XML, JavaScript, Lua).</param>
        public FileTypeAttribute([NotNull] string fileType)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(fileType)) throw new ArgumentNullException("fileType");
            #endregion

            _fileType = fileType;
        }
    }
}
