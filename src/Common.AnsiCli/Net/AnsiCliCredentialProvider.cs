// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user for <see cref="NetworkCredential"/>s using an ANSI console prompt.
/// </summary>
public class AnsiCliCredentialProvider : ICredentialProvider
{
    /// <inheritdoc/>
    public NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        Log.Debug($"Prompt for credentials for {uri} on command-line");

        if (previousIncorrect)
            Log.Error(string.Format(Resources.InvalidCredentials, uri.ToStringRfc()));

        AnsiCli.Error.WriteLine(string.Format(Resources.PleaseEnterCredentials, uri.ToStringRfc()));
        return new NetworkCredential(
            AnsiCli.Error.Prompt(new TextPrompt<string>(Resources.UserName)),
            AnsiCli.Error.Prompt(new TextPrompt<string>(Resources.Password) {IsSecret = true}));
    }
}
