// Copyright Bastian Eicher
// Licensed under the MIT License

using Xunit;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains test methods for <see cref="NetUtils"/>.
    /// </summary>
    public class NetUtilsTest
    {
        [Fact]
        public void TestConfigureTls() => NetUtils.ConfigureTls();
    }
}
