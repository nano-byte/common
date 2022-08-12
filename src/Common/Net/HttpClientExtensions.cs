// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET
using System.Net;
using System.Net.Http;

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
            response = client.Send(request, cancellationToken);
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
}
#endif
