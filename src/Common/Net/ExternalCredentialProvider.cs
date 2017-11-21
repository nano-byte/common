﻿/*
 * Copyright 2006-2017 Bastian Eicher
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

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Uses an external callback to loook up <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s.
    /// </summary>
    public class ExternalCredentialProvider : CredentialProviderBase
    {
        private readonly Func<Uri, NetworkCredential> _lookup;

        /// <summary>
        /// Creates a new external credential provider.
        /// </summary>
        /// <param name="lookup">A callback used to look up credentials for a specific URI.</param>
        public ExternalCredentialProvider([NotNull] Func<Uri, NetworkCredential> lookup)
            : base(interactive: false) => _lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));

        /// <inheritdoc/>
        public override NetworkCredential GetCredential(Uri uri, string authType) => _lookup(uri);
    }
}