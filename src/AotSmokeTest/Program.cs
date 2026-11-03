// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using NanoByte.Common;
using NanoByte.Common.Info;
using NanoByte.Common.Native;
using NanoByte.Common.Net;
using NanoByte.Common.Storage;
using Spectre.Console;

// Runs a set of checks against NanoByte.Common published with Native AOT.
// Exits with a non-zero code if any check fails.

var failures = new List<string>();

void Check(string name, Func<bool> check)
{
    try
    {
        if (check()) Console.WriteLine($"OK    {name}");
        else Fail(name, "unexpected result");
    }
    catch (Exception ex)
    {
        Fail(name, $"{ex.GetType().Name}: {ex.Message}");
    }

    void Fail(string checkName, string reason)
    {
        Console.WriteLine($"FAIL  {checkName}: {reason}");
        failures.Add(checkName);
    }
}

// Errors that are only logged (e.g., D-Bus not available) are fine, unless they indicate trimming or AOT problems
var aotErrors = new List<Exception>();
Log.Handler += (_, _, exception) =>
{
    if (exception is AggregateException aggregate) exception = aggregate.Flatten().InnerException;
    if (exception is MissingMemberException or TypeLoadException or NotSupportedException or InvalidProgramException or TypeInitializationException)
        lock (aotErrors) aotErrors.Add(exception);
};

Check("AppInfo.Load", () => AppInfo.Load(typeof(Program).Assembly).Name == "NanoByte.Common.AotSmokeTest");
Check("AppInfo.Current", () => AppInfo.Current.Name == "NanoByte.Common.AotSmokeTest");

Check("ConversionUtils", () => "42".ConvertFromString<int>() == 42);

Check("JsonStorage roundtrip", () =>
{
    var data = new TestData {Name = "outer", Child = new TestChild {Value = 42}};
    string json = data.ToJsonString(TestJsonContext.Default.TestData);
    return JsonStorage.FromJsonString(json, TestJsonContext.Default.TestData) is {Name: "outer", Child.Value: 42};
});

Check("AnsiCli.Table", () =>
{
    var writer = new StringWriter();
    var console = AnsiConsole.Create(new AnsiConsoleSettings {Out = new AnsiConsoleOutput(writer)});
    console.Profile.Width = 80; // Not attached to a terminal
    console.Write(AnsiCli.Table(new[] {new TestChild {Value = 42}}));
    return writer.ToString().Contains("42");
});

if (UnixUtils.IsUnix) UnixChecks();

[UnsupportedOSPlatform("windows")]
void UnixChecks()
{
    Check("UnixUtils.OSName", () => !string.IsNullOrEmpty(UnixUtils.OSName));
    Check("UnixUtils.CpuType", () => !string.IsNullOrEmpty(UnixUtils.CpuType));

    using var tempDir = new TemporaryDirectory("aot-smoke-test");
    string file = Path.Combine(tempDir, "file");
    File.WriteAllText(file, "data");

    Check("UnixUtils.CreateSymlink", () =>
    {
        string link = Path.Combine(tempDir, "symlink");
        UnixUtils.CreateSymlink(link, "file");
        return UnixUtils.IsSymlink(link, out string? target) && target == "file" && !UnixUtils.IsRegularFile(link);
    });
    Check("UnixUtils.CreateHardlink", () =>
    {
        string link = Path.Combine(tempDir, "hardlink");
        UnixUtils.CreateHardlink(link, file);
        return UnixUtils.GetInode(link) == UnixUtils.GetInode(file) && UnixUtils.IsRegularFile(link);
    });
    Check("UnixUtils.SetExecutable", () =>
    {
        UnixUtils.SetExecutable(file, true);
        bool executable = UnixUtils.IsExecutable(file);
        UnixUtils.SetExecutable(file, false);
        return executable && !UnixUtils.IsExecutable(file);
    });
    Check("UnixUtils.Rename", () =>
    {
        string destination = Path.Combine(tempDir, "renamed");
        File.WriteAllText(destination, "old");
        UnixUtils.Rename(file, destination);
        return !File.Exists(file) && File.ReadAllText(destination) == "data";
    });
}

// Results depend on the environment (e.g., whether D-Bus is available), so only check that no AOT-related errors occur
Check("NetUtils.GetInternetConnectivity", () => Enum.IsDefined(NetUtils.GetInternetConnectivity()));
Check("OSUtils.PreventIdle", () =>
{
    OSUtils.PreventIdle("AOT smoke test")?.Dispose();
    OSUtils.PreventDisplayOff("AOT smoke test")?.Dispose();
    return true;
});
Check("No trimming/AOT errors logged", () =>
{
    lock (aotErrors)
    {
        foreach (var error in aotErrors) Console.WriteLine($"      {error.GetType().Name}: {error.Message}");
        return aotErrors.Count == 0;
    }
});

if (failures.Count == 0)
{
    Console.WriteLine("All checks passed");
    return 0;
}
else
{
    Console.WriteLine($"{failures.Count} check(s) failed");
    return 1;
}

namespace NanoByte.Common
{
    public class TestData
    {
        public string? Name { get; set; }
        public TestChild? Child { get; set; }
    }

    public class TestChild
    {
        public int Value { get; set; }
    }

    [JsonSerializable(typeof(TestData))]
    internal partial class TestJsonContext : JsonSerializerContext;
}
