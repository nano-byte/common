// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user for <see cref="NetworkCredential"/>s using the Windows Credential Manager GUI.
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsGuiCredentialProvider : WindowsCredentialProvider
{
    private static readonly object _lock = new();

    /// <inheritdoc/>
    protected override NetworkCredential GetCredential(string target, WindowsCredentialsFlags flags)
    {
        lock (_lock)
            return WindowsCredentials.PromptGui(target, flags);
    }
}
