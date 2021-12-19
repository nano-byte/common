// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Net;

/// <summary>
/// Contains test methods for <see cref="NetUtils"/>.
/// </summary>
public class NetUtilsTest
{
    [Fact]
    public void TestConfigureTls() => NetUtils.ConfigureTls();
}