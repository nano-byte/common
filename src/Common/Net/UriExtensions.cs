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
using JetBrains.Annotations;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Provides extension methods for <see cref="Uri"/>s.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// An alternate version of <see cref="Uri.ToString"/> that produces results escaped according to RFC 2396.
        /// </summary>
        [PublicAPI, Pure, NotNull]
        public static string ToStringRfc([NotNull] this Uri uri)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException("uri");
            #endregion

            return (uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString).
                // Leave { and } unescaped for templating support
                Replace("%7B", "{").Replace("%7D", "}");
        }

        /// <summary>
        /// Adds a trailing slash to the URI if it does not already have one.
        /// </summary>
        [PublicAPI, Pure, NotNull]
        public static Uri EnsureTrailingSlash([NotNull] this Uri uri)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException("uri");
            #endregion

            string escapedString = uri.ToStringRfc();
            return escapedString.EndsWith("/") ? uri : new Uri(escapedString + "/");
        }

        /// <summary>
        /// Reparses a URI (generated via conversion) to ensure it is a valid absolute URI.
        /// </summary>
        [PublicAPI, Pure, NotNull]
        public static Uri ReparseAsAbsolute([NotNull] this Uri uri)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException("uri");
            #endregion

            return new Uri(uri.OriginalString, UriKind.Absolute);
        }
    }
}