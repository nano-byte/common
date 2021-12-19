// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks the user or a keyring for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s.
    /// </summary>
    /// <remarks>Implementations of this interface are thread-safe.</remarks>
    public interface ICredentialProvider : ICredentials
    {
        /// <summary>
        /// Report that the credentials that were retrieved for <paramref name="uri"/> were incorrect.
        /// </summary>
        void ReportInvalid(Uri uri);
    }
}
