// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets <see cref="NetworkCredential"/>s stored in an operating system keyring. Does not prompt for new credentials.
/// Falls back to another provider if no suitable credentials can be found or the credentials were rejected.
/// </summary>
/// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in the keyring.</param>
public abstract class KeyringCredentialProvider(ICredentialProvider? innerProvider = null) : ICredentialProvider
{
#if NET9_0_OR_GREATER
    private static readonly Lock _lookupLock = new();
    private readonly Lock _stateLock = new();
#else
    private static readonly object _lookupLock = new();
    private readonly object _stateLock = new();
#endif

    /// <summary>Targets for which the last credentials returned came from the keyring, mapped to their user names.</summary>
    private readonly Dictionary<string, string> _fromKeyring = [];

    /// <summary>Targets for which the keyring returned credentials that were rejected, mapped to their user names.</summary>
    private readonly Dictionary<string, string> _rejected = [];

    /// <summary>Targets for which the keyring returned no credentials, e.g., because none were stored or the user denied access.</summary>
    private readonly HashSet<string> _notFound = [];

    /// <inheritdoc/>
    public NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        string target = uri.GetRoot().ToStringRfc();

        bool lookup;
        lock (_stateLock)
        {
            if (previousIncorrect && _fromKeyring.TryGetValue(target, out string? userName))
            {
                Log.Error(string.Format(Resources.InvalidCredentials, $"{target}@{KeyringName}"));
                _fromKeyring.Remove(target);
                _rejected[target] = userName;
                previousIncorrect = false; // The rejected credentials did not come from the inner provider
                lookup = false;
            }
            else lookup = !_rejected.ContainsKey(target) && !_notFound.Contains(target);
        }

        if (lookup)
        {
            if (TryLookup(uri, target) is {} credential)
            {
                lock (_stateLock) _fromKeyring[target] = credential.UserName;
                return credential;
            }
            lock (_stateLock) _notFound.Add(target); // Avoid repeated lookups and unlock prompts on retries
        }

        return innerProvider?.GetCredential(uri, previousIncorrect);
    }

    private NetworkCredential? TryLookup(Uri uri, string target)
    {
        if (!uri.IsAbsoluteUri || uri.IsFile || uri.IsUnc) return null;

        try
        {
            Log.Debug($"Get credentials for {target} from {KeyringName}");
            lock (_lookupLock)
                return Lookup(uri);
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug($"Unable to get credentials for {target} from {KeyringName}", ex);
            return null;
        }
        #endregion
    }

    /// <summary>
    /// Creates a provider for the keyring of the current operating system, if supported.
    /// </summary>
    /// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in the keyring.</param>
    /// <param name="interactive">Allow the keyring to ask the user for permission or to unlock it if necessary.</param>
    /// <returns>A keyring provider wrapping <paramref name="innerProvider"/> if the current operating system is supported; <paramref name="innerProvider"/> otherwise.</returns>
    /// <remarks>Windows is not handled here, since the Windows Credential Manager can also prompt for credentials. See <see cref="WindowsCredentialProvider"/>.</remarks>
    public static ICredentialProvider? ForCurrentOS(ICredentialProvider? innerProvider = null, bool interactive = false)
    {
#if NET
        if (UnixUtils.IsLinux) return new LinuxCredentialProvider(innerProvider, interactive);
        if (UnixUtils.IsMacOSX) return new MacOSCredentialProvider(innerProvider, interactive);
#endif
        return innerProvider;
    }

    /// <summary>
    /// A human-readable name of the keyring for use in log messages.
    /// </summary>
    protected abstract string KeyringName { get; }

    /// <summary>
    /// Looks up stored credentials for a specific <see cref="Uri"/> in the keyring.
    /// </summary>
    /// <param name="uri">An absolute URI that the client is providing authentication for.</param>
    /// <returns>The credentials; <c>null</c> if none are stored.</returns>
    /// <remarks>Implementations may throw any exception to indicate that the keyring is unavailable. Calls are serialized.</remarks>
    protected abstract NetworkCredential? Lookup(Uri uri);
}
