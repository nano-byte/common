// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user for <see cref="NetworkCredential"/>s using the Windows Credential Manager command-line interface.
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsCliCredentialProvider : WindowsCredentialProvider
{
    private readonly Action? _beforePrompt;

    /// <summary>
    /// Creates a new Windows CLI credential provider.
    /// </summary>
    /// <param name="beforePrompt">An optional callback to be invoked right before the user is prompted for credentials</param>
    public WindowsCliCredentialProvider(Action? beforePrompt = null)
    {
        _beforePrompt = beforePrompt;
    }

    /// <inheritdoc/>
    protected override NetworkCredential GetCredential(string target, WindowsCredentialsFlags flags)
    {
        if (flags.HasFlag(WindowsCredentialsFlags.IncorrectPassword))
            Log.Error(string.Format(Resources.InvalidCredentials, target));

        _beforePrompt?.Invoke();
        return WindowsCredentials.PromptCli(target, flags);
    }
}
