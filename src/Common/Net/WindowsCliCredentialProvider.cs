// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user for <see cref="NetworkCredential"/>s using the Windows Credential Manager command-line interface.
/// </summary>
/// <param name="beforePrompt">An optional callback to be invoked right before the user is prompted for credentials</param>
[SupportedOSPlatform("windows")]
public class WindowsCliCredentialProvider(Action? beforePrompt = null) : WindowsCredentialProvider
{
    /// <inheritdoc/>
    protected override NetworkCredential GetCredential(string target, WindowsCredentialsFlags flags)
    {
        if (flags.HasFlag(WindowsCredentialsFlags.IncorrectPassword))
            Log.Error(string.Format(Resources.InvalidCredentials, target));

        beforePrompt?.Invoke();
        return WindowsCredentials.PromptCli(target, flags);
    }
}
