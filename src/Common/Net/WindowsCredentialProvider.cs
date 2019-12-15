// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using NanoByte.Common.Native;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using <see cref="WindowsCredentials"/>.
    /// </summary>
    public abstract class WindowsCredentialProvider : CredentialProviderBase
    {
        /// <inheritdoc/>
        protected WindowsCredentialProvider(ITaskHandler handler)
            : base(handler)
        {}

        /// <inheritdoc/>
        public override NetworkCredential? GetCredential(Uri uri, string authType)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            string target = uri.ToStringRfc();

            if (!Interactive && !WindowsCredentials.IsCredentialStored(target))
                return null;

            var flags = WindowsCredentialsFlags.GenericCredentials | WindowsCredentialsFlags.ExcludeCertificates | WindowsCredentialsFlags.ShowSaveCheckBox;
            if (WasReportedInvalid(uri))
                flags |= WindowsCredentialsFlags.IncorrectPassword | WindowsCredentialsFlags.AlwaysShowUI;

            return Prompt(target, flags);
        }

        /// <summary>
        /// Performs the actual <see cref="WindowsCredentials"/> API call to prompt the user or the credential store for credentials.
        /// </summary>
        /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
        /// <param name="flags">Flags for configuring the prompt.</param>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
        protected abstract NetworkCredential Prompt(string target, WindowsCredentialsFlags flags);
    }
}
