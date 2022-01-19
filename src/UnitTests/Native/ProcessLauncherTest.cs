// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Native;

/// <summary>
/// Contains test methods for <see cref="ProcessLauncher"/>.
/// </summary>
public class ProcessLauncherTest
{
    [Fact]
    public void Run()
    {
        new ProcessLauncher(WindowsUtils.IsWindows ? "attrib" : "ls").Run();
    }

    [Fact]
    public void RunAndCapture()
    {
        string output = new ProcessLauncher(WindowsUtils.IsWindows ? "attrib" : "ls").RunAndCapture();
        output.Length.Should().BeGreaterThan(1);
    }
}
