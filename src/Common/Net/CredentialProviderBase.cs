using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Common base class for <see cref="ICredentialProvider"/> implementations.
    /// </summary>
    public abstract class CredentialProviderBase : MarshalNoTimeout, ICredentialProvider
    {
        /// <inheritdoc/>
        public bool Interactive { get; }

        /// <summary>
        /// Creates a new credential provider.
        /// </summary>
        /// <param name="interactive">Indicates whether the credential provider is interactive, i.e., can ask the user for input.</param>
        protected CredentialProviderBase(bool interactive) => Interactive = interactive;

        /// <inheritdoc/>
        [CanBeNull]
        public abstract NetworkCredential GetCredential(Uri uri, string authType);

        private readonly HashSet<Uri> _invalidList = new HashSet<Uri>();

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
        protected bool WasReportedInvalid([NotNull] Uri uri)
        {
            lock (_invalidList)
                return _invalidList.Remove(uri);
        }
    }
}
