// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Contains unit tests for <see cref="Log"/>.
/// </summary>
public class LogTest
{
    [Fact]
    public void TestContent()
    {
        Log.Info("Log Unit Test Token");
        Log.Content.Contains("Log Unit Test Token").Should().BeTrue();
    }

    [Fact]
    public void TestHandler()
    {
        var events = new List<(LogSeverity, string?, Exception?)>();
        void Handler(LogSeverity severity, string? message, Exception? exception) => events.Add((severity, message, exception));

        Log.Handler += Handler;
        try
        {
            Log.Info("Log Unit Test Token");
            events.Should().Contain((LogSeverity.Info, "Log Unit Test Token", null));
        }
        finally
        {
            Log.Handler -= Handler;
        }
    }
}
