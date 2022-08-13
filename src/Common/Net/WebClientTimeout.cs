// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET
using System.Net;
using NanoByte.Common.Info;

namespace NanoByte.Common.Net;

/// <summary>
/// Adds a customizable timout to <see cref="WebClient"/>.
/// </summary>
public class WebClientTimeout : WebClient
{
    /// <summary>
    /// The default timeout value, in milliseconds, used when no explicit value is specified.
    /// </summary>
    public const int DefaultTimeout = 20000; // 20 seconds

    private readonly int _timeout;

    /// <summary>
    /// Creates a new <see cref="WebClient"/> using <see cref="DefaultTimeout"/>.
    /// </summary>
    public WebClientTimeout()
        : this(DefaultTimeout)
    {}

    /// <summary>
    /// Creates a new <see cref="WebClient"/>.
    /// </summary>
    /// <param name="timeout">The length of time, in milliseconds, before requests made by this <see cref="WebClient"/> time out.</param>
    public WebClientTimeout(int timeout)
    {
        _timeout = timeout;
    }

    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = base.GetWebRequest(address);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (request != null)
            request.Timeout = _timeout;

        if (request is HttpWebRequest httpRequest)
            httpRequest.UserAgent = AppInfo.Current.NameVersion;

        return request!;
    }
}
#endif
