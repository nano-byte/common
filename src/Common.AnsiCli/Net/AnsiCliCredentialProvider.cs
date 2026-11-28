// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user for <see cref="NetworkCredential"/>s using an ANSI console prompt.
/// </summary>
/// <param name="beforePrompt">An optional callback to be invoked right before the user is prompted for credentials</param>
/// <param name="offerRemember">Ask the user whether to remember the credentials and return a <see cref="PromptedCredential"/> with the answer.</param>
public class AnsiCliCredentialProvider(Action? beforePrompt = null, bool offerRemember = false) : ICredentialProvider
{
#if NET9_0_OR_GREATER
    private static readonly Lock _lock = new();
#else
    private static readonly object _lock = new();
#endif

    /// <inheritdoc/>
    public NetworkCredential GetCredential(Uri uri, bool previousIncorrect = false)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        Log.Debug($"Prompt for credentials for {uri} on command-line");

        if (previousIncorrect)
            Log.Error(string.Format(Resources.InvalidCredentials, uri.ToStringRfc()));

        beforePrompt?.Invoke();

        lock (_lock)
        {
            AnsiCli.Error.WriteLine(string.Format(Resources.PleaseEnterCredentials, uri.ToStringRfc()));
            string userName = AnsiCli.Error.Prompt(new TextPrompt<string>(Resources.UserName));
            string password = AnsiCli.Error.Prompt(new TextPrompt<string>(Resources.Password) {IsSecret = true});
            return offerRemember
                ? new PromptedCredential(userName, password,
                    remember: AnsiCli.Error.Prompt(new ConfirmationPrompt(Resources.RememberCredentials) {DefaultValue = false}))
                : new NetworkCredential(userName, password);
        }
    }
}
