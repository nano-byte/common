// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Net;
using JetBrains.Annotations;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;
using NanoByte.Common.Tasks;
using NanoByte.Common.Values;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using <see cref="WindowsCredentials.PromptCli"/>.
    /// </summary>
    public class WindowsCliCredentialProvider : WindowsCredentialProvider
    {
        /// <summary>
        /// Creates a new Windows command-line credential provider.
        /// </summary>
        /// <param name="handler">Used to determine whether and how to ask the user for input.</param>
        public WindowsCliCredentialProvider([NotNull] ITaskHandler handler)
            : base(handler)
        {}

        /// <inheritdoc/>
        protected override NetworkCredential Prompt(string target, WindowsCredentialsFlags flags)
        {
            if (flags.HasFlag(WindowsCredentialsFlags.IncorrectPassword))
                Log.Error(string.Format(Resources.InvalidCredentials, target));
            return WindowsCredentials.PromptCli(target, flags);
        }
    }
}
