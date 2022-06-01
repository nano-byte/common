// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets <see cref="NetworkCredential"/>s using the Windows Credential Manager.
/// </summary>
[SupportedOSPlatform("windows")]
public abstract class WindowsCredentialProvider : ICredentialProvider
{
    /// <inheritdoc/>
    public NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        // Use URI without path as credential target identifier
        string target = new UriBuilder(uri) {Path = null, UserName = null, Password = null}.ToString();
        Log.Debug($"Get credentials for {target} from Windows Credential Manager");

        var flags = WindowsCredentialsFlags.GenericCredentials | WindowsCredentialsFlags.ExcludeCertificates | WindowsCredentialsFlags.ShowSaveCheckBox;
        if (previousIncorrect)
            flags |= WindowsCredentialsFlags.IncorrectPassword | WindowsCredentialsFlags.AlwaysShowUI;

        return GetCredential(target, flags);
    }

    /// <summary>
    /// Performs the actual <see cref="WindowsCredentials"/> API call to prompt the user or the credential store for credentials.
    /// </summary>
    /// <param name="target">A string identifying the target the credentials are intended for.</param>
    /// <param name="flags">Flags for configuring the prompt.</param>
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
    protected abstract NetworkCredential? GetCredential(string target, WindowsCredentialsFlags flags);
}
