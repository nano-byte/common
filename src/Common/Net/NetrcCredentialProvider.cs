// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets credentials from <see cref="Netrc"/> if possible. Falls back to another provider otherwise.
/// </summary>
public class NetrcCredentialProvider : ICredentialProvider
{
    private readonly Netrc _netrc = Netrc.LoadSafe();
    private readonly ICredentialProvider? _innerProvider;

    /// <summary>
    /// Creates a new <see cref="Netrc"/> credential provider.
    /// </summary>
    /// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in <see cref="Netrc"/>.</param>
    public NetrcCredentialProvider(ICredentialProvider? innerProvider = null)
    {
        _innerProvider = innerProvider;
    }

    /// <inheritdoc/>
    public NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false)
    {
        if (_netrc.TryGetValue(uri.Host, out var value))
        {
            if (previousIncorrect)
            {
                Log.Error(string.Format(Resources.InvalidCredentials, uri.Host + "@.netrc"));
                return _innerProvider?.GetCredential(uri);
            }

            Log.Debug($"Got credentials for {uri.Host} from .netrc file");
            return value;
        }

        return _innerProvider?.GetCredential(uri, previousIncorrect);
    }
}
