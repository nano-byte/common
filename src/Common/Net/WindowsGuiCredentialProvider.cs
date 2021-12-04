// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks the user for <see cref="NetworkCredential"/>s using the Windows Credential Manager GUI.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class WindowsGuiCredentialProvider : WindowsCredentialProvider
    {
        /// <inheritdoc/>
        protected override NetworkCredential Prompt(string target, WindowsCredentialsFlags flags)
            => WindowsCredentials.PromptGui(target, flags);
    }
}
