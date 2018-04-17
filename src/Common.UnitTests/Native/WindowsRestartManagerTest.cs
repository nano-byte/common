// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;
using Xunit;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Contains test methods for <see cref="WindowsRestartManager"/>.
    /// </summary>
    public class WindowsRestartManagerTest
    {
        [SkippableFact]
        public void TestListApps()
        {
            Skip.IfNot(WindowsUtils.IsWindowsVista, reason: "Restart Manager only available on Windows Vista or higher");

            using (var restartManager = new WindowsRestartManager())
            {
                restartManager.RegisterResources(@"C:\Windows\explorer.exe");
                restartManager.ListApps(new SilentTaskHandler());
            }
        }
    }
}
