// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Gets <see cref="NetworkCredential"/>s using the Windows Credential Manager.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public abstract class WindowsCredentialProvider : CredentialProviderBase
    {
        /// <inheritdoc/>
        public override NetworkCredential? GetCredential(Uri uri, string authType)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            var flags = WindowsCredentialsFlags.GenericCredentials | WindowsCredentialsFlags.ExcludeCertificates | WindowsCredentialsFlags.ShowSaveCheckBox;
            if (WasReportedInvalid(uri))
                flags |= WindowsCredentialsFlags.IncorrectPassword | WindowsCredentialsFlags.AlwaysShowUI;

            return Prompt(uri.ToStringRfc(), flags);
        }

        /// <summary>
        /// Performs the actual <see cref="WindowsCredentials"/> API call to prompt the user or the credential store for credentials.
        /// </summary>
        /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
        /// <param name="flags">Flags for configuring the prompt.</param>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
        protected abstract NetworkCredential? Prompt(string target, WindowsCredentialsFlags flags);
    }
}
