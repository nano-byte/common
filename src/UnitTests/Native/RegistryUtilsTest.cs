// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.Versioning;
using Microsoft.Win32;

namespace NanoByte.Common.Native;

[SupportedOSPlatform("windows")]
public class RegistryUtilsTest
{
    public RegistryUtilsTest()
    {
        Assert.SkipUnless(WindowsUtils.IsWindows, "Can only test registry functions on Windows or newer");
    }

#if !NET20
    [Fact]
    public void TestGetLastWriteTime()
    {
        Registry.CurrentUser.GetLastWriteTime()
                .Should().BeAfter(new DateTime(2000, 1, 1));
    }
#endif
}
