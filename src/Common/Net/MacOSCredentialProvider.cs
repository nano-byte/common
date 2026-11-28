#if NET
// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets <see cref="NetworkCredential"/>s stored as internet passwords in the MacOS X Keychain.
/// Falls back to another provider if no suitable credentials can be found.
/// Saves credentials the user asked to remember as internet passwords.
/// </summary>
/// <remarks>
/// This matches the items stored by Safari and <c>git-credential-osxkeychain</c>.
/// Credentials can also be stored manually, e.g.: <c>security add-internet-password -s example.com -r htps -a myname -w</c>
/// </remarks>
/// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in the Keychain.</param>
/// <param name="interactive">Allow the Keychain to ask the user for permission or to unlock it if necessary.</param>
[SupportedOSPlatform("macos")]
public class MacOSCredentialProvider(ICredentialProvider? innerProvider = null, bool interactive = false) : KeyringCredentialProvider(innerProvider)
{
    /// <inheritdoc/>
    protected override string KeyringName => "Keychain";

    /// <inheritdoc/>
    protected override NetworkCredential? Lookup(Uri uri)
    {
        foreach (int port in GetPorts(uri))
        {
            if (MacOSKeychain.FindInternetPassword(uri.Host, uri.Scheme, port, interactive) is var (account, password))
                return new(account, password);
        }
        return null;
    }

    /// <inheritdoc/>
    protected override void Store(Uri uri, NetworkCredential credential)
        => MacOSKeychain.AddInternetPassword(uri.Host, uri.Scheme, uri.IsDefaultPort ? 0 : uri.Port, credential.UserName, credential.Password, label: uri.Host, interactive);

    /// <inheritdoc/>
    protected override void Delete(Uri uri, string userName)
    {
        foreach (int port in GetPorts(uri))
            MacOSKeychain.DeleteInternetPassword(uri.Host, uri.Scheme, port, userName, interactive);
    }

    /// <summary>
    /// Gets the Keychain port values that identify items for the <paramref name="uri"/>.
    /// </summary>
    /// <remarks>Items for the default port may be stored with an explicit port or without one (port <c>0</c>).</remarks>
    private static int[] GetPorts(Uri uri)
        => uri.IsDefaultPort ? [uri.Port, 0] : [uri.Port];
}
#endif
