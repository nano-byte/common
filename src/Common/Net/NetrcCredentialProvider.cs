// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets credentials from <see cref="Netrc"/> if possible. Falls back to another provider otherwise.
/// </summary>
/// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in <see cref="Netrc"/>.</param>
public class NetrcCredentialProvider(ICredentialProvider? innerProvider = null) : ICredentialProvider, ICredentialFeedback
{
    private readonly Lazy<Netrc> _netrc = new(Netrc.LoadSafe);

    /// <summary>
    /// Creates a provider for specific <see cref="Netrc"/> entries instead of the default file. For unit tests.
    /// </summary>
    internal NetrcCredentialProvider(Netrc netrc, ICredentialProvider? innerProvider = null) : this(innerProvider)
    {
        _netrc = new(() => netrc);
    }

#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    /// <summary>Hosts for which the credentials from <see cref="Netrc"/> were rejected.</summary>
    private readonly HashSet<string> _rejected = [];

    /// <inheritdoc/>
    public NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        lock (_lock)
        {
            if (!_rejected.Contains(uri.Host) && _netrc.Value.TryGetValue(uri.Host, out var value))
            {
                if (!previousIncorrect)
                {
                    Log.Debug($"Got credentials for {uri.Host} from .netrc file");
                    return value;
                }

                Log.Error(string.Format(Resources.InvalidCredentials, $"{uri.Host}@.netrc"));
                _rejected.Add(uri.Host); // Pass on future rejections to the inner provider
                previousIncorrect = false; // The rejected credentials did not come from the inner provider
            }
        }

        return innerProvider?.GetCredential(uri, previousIncorrect);
    }

    /// <inheritdoc/>
    public void Accepted(Uri uri, NetworkCredential credential)
        => (innerProvider as ICredentialFeedback)?.Accepted(uri, credential);
}
