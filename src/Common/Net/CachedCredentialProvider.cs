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
using System.Net;
using JetBrains.Annotations;
using NanoByte.Common.Collections;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Caching decorator for <see cref="ICredentialProvider"/>s.
    /// </summary>
    public class CachedCredentialProvider : MarshalNoTimeout, ICredentialProvider
    {
        [NotNull]
        private readonly ICredentialProvider _inner;

        /// <inheritdoc/>
        public bool Interactive => _inner.Interactive;

        private readonly TransparentCache<Uri, NetworkCredential> _cache;

        /// <summary>
        /// Creates a new caching dectorator.
        /// </summary>
        /// <param name="inner">The inner <see cref="ICredentialProvider"/> to wrap.</param>
        public CachedCredentialProvider([NotNull] ICredentialProvider inner)
        {
            #region Sanity checks
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            #endregion

            _inner = inner;
            _cache = new TransparentCache<Uri, NetworkCredential>(uri => inner.GetCredential(uri, null));
        }

        /// <inheritdoc/>
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            return _cache[uri.GetBaseUri()];
        }

        /// <inheritdoc/>
        public void ReportInvalid(Uri uri)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            uri = uri.GetBaseUri();
            _cache.Remove(uri);
            _inner.ReportInvalid(uri);
        }
    }
}
