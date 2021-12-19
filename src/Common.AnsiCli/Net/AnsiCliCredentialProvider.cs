// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user for <see cref="NetworkCredential"/>s using an ANSI console prompt.
/// </summary>
public class AnsiCliCredentialProvider : CredentialProviderBase
{
    /// <inheritdoc/>
    public override NetworkCredential GetCredential(Uri uri, string authType)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        Log.Debug("Prompt for credentials on command-line: " + uri.ToStringRfc());
        if (WasReportedInvalid(uri))
            Log.Error(string.Format(Resources.InvalidCredentials, uri.ToStringRfc()));

        AnsiCli.Error.WriteLine(Resources.PleasEnterCredentials, uri.ToStringRfc());
        return new NetworkCredential(
            AnsiCli.Error.Prompt(new TextPrompt<string>(Resources.UserName)),
            AnsiCli.Error.Prompt(new TextPrompt<string>(Resources.Password) {IsSecret = true}));
    }
}