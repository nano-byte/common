// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

public class HttpServerTest
{
    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

    private sealed class TestServer : HttpServer
    {
        private readonly Action<HttpListenerContext> _handleRequest;

        public TestServer(Action<HttpListenerContext> handleRequest, int maxConcurrentRequests = 64)
            : base(localOnly: true)
        {
            _handleRequest = handleRequest;
            MaxConcurrentRequests = maxConcurrentRequests;
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

    [Fact]
    public async Task RejectsRequestsExceedingMaxConcurrentRequests()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var handling = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();

        using var server = new TestServer(maxConcurrentRequests: 1, handleRequest: _ =>
        {
            handling.Set();
            release.Wait(_timeout, cancellationToken);
        });

        // ReSharper disable once ShortLivedHttpClient
        using var client = new HttpClient();

        var occupySlot = Task.Run(() => client.Send(new(HttpMethod.Get, server.Uri), cancellationToken), cancellationToken);
        handling.Wait(_timeout, cancellationToken).Should().BeTrue("the first request should be picked up for handling");

        using var rejected = client.Send(new(HttpMethod.Get, server.Uri), cancellationToken);
        rejected.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

        release.Set();
        using var first = await occupySlot;
        first.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void HandlesRequestsAgainAfterReachingMaxConcurrentRequests()
    {
        using var server = new TestServer(maxConcurrentRequests: 1, handleRequest: _ => {});

        // ReSharper disable once ShortLivedHttpClient
        using var client = new HttpClient();
        for (int i = 0; i < 3; i++)
        {
            using var response = client.Send(new(HttpMethod.Get, server.Uri), TestContext.Current.CancellationToken);
            response.StatusCode.Should().Be(HttpStatusCode.OK, "the slot should be released after each request");
        }
    }
}
