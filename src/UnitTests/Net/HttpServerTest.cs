// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

public class HttpServerTest
{
    private sealed class TestServer : HttpServer
    {
        private readonly Action<HttpListenerContext> _handleRequest;

        public TestServer(Action<HttpListenerContext> handleRequest)
            : base(localOnly: true)
        {
            _handleRequest = handleRequest;
            StartHandlingRequests();
        }

        public Uri Uri => new($"http://localhost:{Port}/");

        protected override void HandleRequest(HttpListenerContext context) => _handleRequest(context);
    }

    [Fact]
    public void ReportsHandlerFailureAsInternalServerError()
    {
        using var server = new TestServer(_ => throw new InvalidOperationException("Simulated failure"));

        // ReSharper disable once ShortLivedHttpClient
        using var client = new HttpClient();
        using var response = client.Send(new(HttpMethod.Get, server.Uri), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public void TruncatesResponseWhenHandlerFailsAfterSendingHeaders()
        => TestTruncation(response => response.ContentLength64 = 1024);

    [Fact]
    public void TruncatesChunkedResponseWhenHandlerFailsAfterSendingHeaders()
    {
        Assert.SkipUnless(WindowsUtils.IsWindows, reason: "The managed HttpListener implementation used on non-Windows systems always appends the terminating chunk, even when the response is aborted.");

        TestTruncation(response => response.SendChunked = true);
    }

    private static void TestTruncation(Action<HttpListenerResponse> announceBody)
    {
        using var server = new TestServer(context =>
        {
            announceBody(context.Response);
            context.Response.OutputStream.WriteByte(1);
            context.Response.OutputStream.Flush();
            throw new InvalidOperationException("Simulated failure while streaming");
        });

        // ReSharper disable once ShortLivedHttpClient
        using var client = new HttpClient();
        using var response = client.Send(new(HttpMethod.Get, server.Uri), HttpCompletionOption.ResponseHeadersRead, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK, "the status code is already on the wire and cannot be changed anymore");

        // So the failure can only be signalled by aborting the connection
        Action readToEnd = () =>
        {
            using var stream = response.Content.ReadAsStream(TestContext.Current.CancellationToken);
            var buffer = new byte[1024];
            while (stream.Read(buffer, 0, buffer.Length) > 0) {}
        };
        readToEnd.Should().Throw<IOException>("the client must not mistake a partial response for a complete one");
    }

}
