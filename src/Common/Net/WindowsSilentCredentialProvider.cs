﻿// Copyright Bastian Eicher
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
    /// Gets <see cref="NetworkCredential"/>s stored in the Windows Credential Manager. Does not show any UI for missing credentials.
    /// </summary>
    public class WindowsSilentCredentialProvider : WindowsCredentialProvider
    {
        /// <inheritdoc/>
        protected override NetworkCredential? Prompt(string target, WindowsCredentialsFlags flags)
        {
            if (flags.HasFlag(WindowsCredentialsFlags.IncorrectPassword))
            {
                Log.Error(string.Format(Resources.InvalidCredentials, target));
                return null;
            }

            return WindowsCredentials.IsCredentialStored(target)
                ? WindowsCredentials.PromptGui(target, WindowsCredentialsFlags.None)
                : null;
        }
    }
}
