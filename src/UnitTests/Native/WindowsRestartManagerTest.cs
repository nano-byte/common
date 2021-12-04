// Copyright Bastian Eicher
// Licensed under the MIT License

using Xunit;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Contains test methods for <see cref="WindowsRestartManager"/>.
    /// </summary>
    public class WindowsRestartManagerTest
    {
        public WindowsRestartManagerTest()
        {
            Skip.IfNot(WindowsUtils.IsWindowsVista, reason: "Restart Manager only available on Windows Vista or higher");
        }

        [SkippableFact]
        public void TestListApps()
        {
            using var restartManager = new WindowsRestartManager();
            restartManager.RegisterResources(@"C:\Windows\explorer.exe");
            restartManager.ListApps();
        }
    }
}
