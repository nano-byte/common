// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;

#if NET20
using NanoByte.Common.Values;
#endif

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks the user for <see cref="NetworkCredential"/>s using the Windows Credential Manager command-line interface.
    /// </summary>
    public class WindowsCliCredentialProvider : WindowsCredentialProvider
    {
        /// <inheritdoc/>
        protected override NetworkCredential Prompt(string target, WindowsCredentialsFlags flags)
        {
            if (flags.HasFlag(WindowsCredentialsFlags.IncorrectPassword))
                Log.Error(string.Format(Resources.InvalidCredentials, target));
            return WindowsCredentials.PromptCli(target, flags);
        }
    }
}
