// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Optionally implemented by <see cref="ICredentialProvider"/>s that want to know when the credentials they returned were accepted, e.g., to save them.
/// </summary>
/// <remarks>Implementations of this interface are thread-safe.</remarks>
public interface ICredentialFeedback
{
    /// <summary>
    /// Reports that credentials previously returned by <see cref="ICredentialProvider.GetCredential"/> were accepted by the server.
    /// </summary>
    /// <param name="uri">The URI that the credentials were requested for.</param>
    /// <param name="credential">The credentials exactly as returned by <see cref="ICredentialProvider.GetCredential"/>.</param>
    void Accepted(Uri uri, NetworkCredential credential);
}
