// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Net;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Net;

/// <summary>
/// A minimalistic HTTP server that only serves a single file on localhost. Useful for unit tests.
/// </summary>
[CLSCompliant(false)]
public sealed class MicroServer : HttpServer
{
    /// <summary>
    /// The URL under which the server root can be reached. Usually you should use <see cref="FileUri"/> instead.
    /// </summary>
    public Uri ServerUri => new($"http://localhost:{Port}/");

    /// <summary>
    /// The complete URL under which the server provides its file.
    /// </summary>
    public Uri FileUri => new(ServerUri, _resourceName);

    /// <summary>
    /// The content of the file to be served under <see cref="FileUri"/>.
    /// </summary>
    public Stream FileContent { get; private set; }

    /// <summary>
    /// Wait for twenty seconds every time before finishing a response.
    /// </summary>
    public bool Slow { get; set; }

    private readonly string _resourceName;

    /// <summary>
    /// Starts serving a single file via HTTP on localhost.
    /// </summary>
    /// <param name="resourceName">The HTTP resource name under which to provide the content.</param>
    /// <param name="fileContent">The content of the file to serve.</param>
    public MicroServer([Localizable(false)] string resourceName, Stream fileContent) : base(localOnly: true)
    {
        _resourceName = resourceName;
        FileContent = fileContent;

        StartHandlingRequests();
    }

    protected override void HandleRequest(HttpListenerContext context)
    {
        // Only return one specific file
        if (context.Request.RawUrl != $"/{_resourceName}")
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        // Delay finishing the file transfer if Slow-mode is active
        if (Slow) Thread.Sleep(20000);

        switch (context.Request.HttpMethod)
        {
            case "GET":
                context.Response.ContentLength64 = FileContent.Length;
                FileContent.CopyToEx(context.Response.OutputStream);
                break;

            case "PUT":
                FileContent = new MemoryStream();
                context.Request.InputStream.CopyToEx(FileContent);
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                break;
        }
    }
}
#endif
