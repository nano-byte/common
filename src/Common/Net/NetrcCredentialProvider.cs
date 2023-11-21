// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets credentials from <see cref="Netrc"/> if possible. Falls back to another provider otherwise.
/// </summary>
/// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in <see cref="Netrc"/>.</param>
public class NetrcCredentialProvider(ICredentialProvider? innerProvider = null) : ICredentialProvider
{
    private readonly Netrc _netrc = Netrc.LoadSafe();

    /// <inheritdoc/>
    public NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false)
    {
        if (_netrc.TryGetValue(uri.Host, out var value))
        {
            if (previousIncorrect)
            {
                Log.Error(string.Format(Resources.InvalidCredentials, $"{uri.Host}@.netrc"));
                return innerProvider?.GetCredential(uri);
            }

            Log.Debug($"Got credentials for {uri.Host} from .netrc file");
            return value;
        }

        return innerProvider?.GetCredential(uri, previousIncorrect);
    }
}
