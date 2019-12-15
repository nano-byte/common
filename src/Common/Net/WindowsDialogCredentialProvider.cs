// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Net;
using NanoByte.Common.Native;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using <see cref="WindowsCredentials.PromptDialog"/>.
    /// </summary>
    public class WindowsDialogCredentialProvider : WindowsCredentialProvider
    {
        /// <summary>
        /// Creates a new Windows GUI credential provider.
        /// </summary>
        /// <param name="handler">Used to determine whether and how to ask the user for input.</param>
        public WindowsDialogCredentialProvider(ITaskHandler handler)
            : base(handler)
        {}

        /// <inheritdoc/>
        protected override NetworkCredential Prompt(string target, WindowsCredentialsFlags flags)
            => WindowsCredentials.PromptDialog(target, flags);
    }
}
