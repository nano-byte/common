// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Streams;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Net;

public class MicroServerTest
{
    [Fact]
    public void Stress()
    {
        const string content = "abc";

        StressTest.Run(() =>
        {
            using var server = new MicroServer("file", content.ToStream());
            using var client = new HttpClient();
            using var response = client.Send(new(HttpMethod.Get, server.FileUri));
            response.EnsureSuccessStatusCode();
            response.Content.ReadAsStream().ReadToString().Should().Be(content);
        });
    }
}
