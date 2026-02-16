// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.Versioning;

namespace NanoByte.Common.Native;

/// <summary>
/// Contains test methods for <see cref="WindowsRestartManager"/>.
/// </summary>
[SupportedOSPlatform("windows6.0")]
public class WindowsRestartManagerTest
{
    public WindowsRestartManagerTest()
    {
        Assert.SkipUnless(WindowsUtils.IsWindowsVista, reason: "Restart Manager only available on Windows Vista or higher");
    }

    [Fact]
    public void TestListApps()
    {
        using var restartManager = new WindowsRestartManager();
        restartManager.RegisterResources(@"C:\Windows\explorer.exe");
        restartManager.ListApps(TestContext.Current.CancellationToken);
    }
}
