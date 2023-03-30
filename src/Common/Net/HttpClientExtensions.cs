// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Net;
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
    /// Sends an HTTP request and ensures that the result is successful.
    /// </summary>
    /// <exception cref="WebException">The request failed or returned a non-2xx status code.</exception>
    public static HttpResponseMessage SendEnsureSuccess(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response = null;
        try
        {
#if NETFRAMEWORK
            response = ThreadUtils.RunTask(() => client.SendAsync(request, cancellationToken));
#else
            response = client.Send(request, cancellationToken);
#endif
            response.EnsureSuccessStatusCode();
            return response;
        }
        #region Error handling
        catch (HttpRequestException ex)
        {
            response?.Dispose();

            // Wrap exception since only certain exception types are allowed
            throw new WebException(ex.Message, ex);
        }
        #endregion
    }

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
