// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks the user for <see cref="NetworkCredential"/>s using the Windows Credential Manager GUI.
    /// </summary>
    public class WindowsGuiCredentialProvider : WindowsCredentialProvider
    {
        /// <inheritdoc/>
        protected override NetworkCredential Prompt(string target, WindowsCredentialsFlags flags)
            => WindowsCredentials.PromptGui(target, flags);
    }
}
