// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Provides extension methods for <see cref="IWebProxy"/>s.
/// </summary>
public static class WebProxyExtensions
{
    /// <summary>
    /// Determines whether custom (non-default and non-empty) credentials are configured for this proxy.
    /// </summary>
    public static bool HasCustomCredentials(this IWebProxy proxy)
        => proxy.Credentials != null && proxy.Credentials != CredentialCache.DefaultCredentials;
}
