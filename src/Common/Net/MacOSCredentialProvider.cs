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
        => MacOSKeychain.FindInternetPassword(uri.Host, uri.Scheme, uri.IsDefaultPort ? null : uri.Port, interactive) is var (account, password)
            ? new NetworkCredential(account, password)
            : null;
}
#endif
