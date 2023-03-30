// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Net;
using System.Net.Http.Headers;

namespace NanoByte.Common.Net;

/// <summary>
/// Provides extension methods for <see cref="NetworkCredential"/>s.
/// </summary>
public static class NetworkCredentialExtension
{
    /// <summary>
    /// Creates a HTTP basic authentication header from the provided credentials.
    /// </summary>
    public static AuthenticationHeaderValue ToBasicAuth(this NetworkCredential credential)
        => new("Basic", (credential.UserName + ":" + credential.Password).Base64Utf8Encode());
}
#endif
