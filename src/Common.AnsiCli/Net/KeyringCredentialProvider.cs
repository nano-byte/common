// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET
using System.Net;
using GnomeStack.Os.Secrets.Darwin;
using GnomeStack.Os.Secrets.Linux;
using NanoByte.Common.Native;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets and stores <see cref="NetworkCredential"/>s using the OS keyring.
/// </summary>
/// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in the OS keyring (yet).</param>
public class KeyringCredentialProvider(ICredentialProvider? innerProvider = null) : ICredentialProvider
{
    /// <inheritdoc/>
    public NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false)
    {
        string service = uri.ToStringRfc();

        try
        {
            if (UnixUtils.IsLinux && LibSecret.ListSecrets(service) is [var secret, ..])
            {
                if (previousIncorrect) LibSecret.DeleteSecret(service, secret.Account);
                else return new(secret.Account, secret.Secret);
            }
            if (UnixUtils.IsMacOSX)
            {
                if (previousIncorrect) KeyChain.DeleteSecret(service);
                else if (KeyChain.GetSecret(service, "") is {} json)
                    return JsonStorage.FromJsonString<NetworkCredential>(json);
            }
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Info("Failed to read from OS keyring", ex);
        }
        #endregion

        var credential = innerProvider?.GetCredential(uri, previousIncorrect);
        if (credential != null)
        {
            try
            {
                if (UnixUtils.IsLinux) LibSecret.SetSecret(service, credential.UserName, credential.Password);
                if (UnixUtils.IsMacOSX) KeyChain.SetSecret(service, "", new {credential.UserName, credential.Password}.ToJsonString());
            }
            #region Error handling
            catch (Exception ex)
            {
                Log.Info("Failed to write to OS keyring", ex);
            }
            #endregion
        }

        return credential;
    }
}
#endif
