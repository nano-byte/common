// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Concurrent;
using System.Diagnostics;
using NanoByte.Common.Info;

namespace NanoByte.Common;

/// <summary>
/// Contains unit tests for <see cref="Log"/>.
/// </summary>
public class LogTest
{
    [Fact]
    public void TestBuffer()
    {
        Log.Info("Log Unit Test Token");
        Log.GetBuffer().Should().Contain("Log Unit Test Token");
    }

    [Fact]
    public void TestHandler()
    {
        // Log is global, so tests running in parallel may invoke the handler on other threads
        var events = new ConcurrentQueue<(LogSeverity, string?, Exception?)>();
        void Handler(LogSeverity severity, string? message, Exception? exception) => events.Enqueue((severity, message, exception));

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

    [Fact]
    public void TestReadLastErrorFrom()
    {
        string message = $"{nameof(Log.ReadLastErrorFrom)} test: {new Random().Next()}";

        Log.Error("Dummy message");
        Log.Error(message);

        Log.ReadLastErrorFrom(AppInfo.Current.Name!, Process.GetCurrentProcess().Id)
           .Should().Be(message);
    }
}
