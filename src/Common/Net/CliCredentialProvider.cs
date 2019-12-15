// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Net;
using NanoByte.Common.Cli;
using NanoByte.Common.Properties;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks the user for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using a command-line prompt.
    /// </summary>
    public class CliCredentialProvider : CredentialProviderBase
    {
        /// <summary>
        /// Creates a new command-line credential provider.
        /// </summary>
        /// <param name="handler">Used to determine whether and how to ask the user for input.</param>
        public CliCredentialProvider(ITaskHandler handler)
            : base(handler)
        {}

        /// <inheritdoc/>
        public override NetworkCredential? GetCredential(Uri uri, string authType)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            if (!Interactive) return null;

            Log.Debug("Prompt for credentials on command-line: " + uri.ToStringRfc());
            if (WasReportedInvalid(uri))
                Log.Error(string.Format(Resources.InvalidCredentials, uri.ToStringRfc()));
            Console.Error.WriteLine(Resources.PleasEnterCredentials, uri.ToStringRfc());
            return new NetworkCredential(
                CliUtils.ReadString(Resources.UserName),
                CliUtils.ReadPassword(Resources.Password));
        }
    }
}
