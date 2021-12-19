// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Common base class for <see cref="ICredentialProvider"/> implementations.
    /// </summary>
    public abstract class CredentialProviderBase : ICredentialProvider
    {
        /// <inheritdoc/>
        public abstract NetworkCredential? GetCredential(Uri uri, string authType);

        private readonly HashSet<Uri> _invalidList = new();

        /// <inheritdoc/>
        public void ReportInvalid(Uri uri)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            lock (_invalidList)
                _invalidList.Add(uri);
        }

        /// <summary>
        /// Checks whether <paramref name="uri"/> was previously reported as invalid and resets the flag.
        /// </summary>
        protected bool WasReportedInvalid(Uri uri)
        {
            lock (_invalidList)
                return _invalidList.Remove(uri);
        }
    }
}
