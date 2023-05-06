// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Net.Http;

#if NETFRAMEWORK
using NanoByte.Common.Threading;
#endif

namespace NanoByte.Common.Net;

/// <summary>
/// Provides extension methods for <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Sends an HTTP request and waits for the result is synchronously.
    /// </summary>
    /// <exception cref="HttpRequestException">The request failed due to a network, DNS or certificate issue.</exception>
    public static HttpResponseMessage Send(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
#if NETFRAMEWORK
        => ThreadUtils.RunTask(() => client.SendAsync(request, cancellationToken));
#else
        => client.Send(request, cancellationToken);
#endif

    /// <summary>
    /// Reads the content as a stream.
    /// </summary>
    public static Stream ReadAsStream(this HttpContent content, CancellationToken cancellationToken = default)
#if NETFRAMEWORK
        => ThreadUtils.RunTask(content.ReadAsStreamAsync);
#else
        => content.ReadAsStream(cancellationToken);
#endif
}
#endif
