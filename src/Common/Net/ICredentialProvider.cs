// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user or a keyring for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s.
/// </summary>
/// <remarks>Implementations of this interface are thread-safe.</remarks>
public interface ICredentialProvider
{
    /// <summary>
    /// Returns <see cref="NetworkCredential"/>s for a specific <see cref="Uri"/>.
    /// </summary>
    /// <param name="uri">The URI that the client is providing authentication for.</param>
    /// <param name="previousIncorrect">Reports that the credentials previously returned by this provider were incorrect.</param>
    NetworkCredential? GetCredential(Uri uri, bool previousIncorrect = false);
}
