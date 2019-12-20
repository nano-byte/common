// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Net;
using NanoByte.Common.Collections;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Caching decorator for <see cref="ICredentialProvider"/>s.
    /// </summary>
    public class CachedCredentialProvider : MarshalNoTimeout, ICredentialProvider
    {
        private readonly ICredentialProvider _inner;

        /// <inheritdoc/>
        public bool Interactive => _inner.Interactive;

        private readonly TransparentCache<Uri, NetworkCredential> _cache;

        /// <summary>
        /// Creates a new caching decorator.
        /// </summary>
        /// <param name="inner">The inner <see cref="ICredentialProvider"/> to wrap.</param>
        public CachedCredentialProvider(ICredentialProvider inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = new TransparentCache<Uri, NetworkCredential>(uri => inner.GetCredential(uri, null));
        }

        /// <inheritdoc/>
        public NetworkCredential GetCredential(Uri uri, string? authType)
            => _cache[(uri ?? throw new ArgumentNullException(nameof(uri))).GetBaseUri()];

        /// <inheritdoc/>
        public void ReportInvalid(Uri uri)
        {
            uri = (uri ?? throw new ArgumentNullException(nameof(uri))).GetBaseUri();
            _cache.Remove(uri);
            _inner.ReportInvalid(uri);
        }
    }
}
