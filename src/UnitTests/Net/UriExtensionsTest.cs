// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains test methods for <see cref="UriExtensions"/>.
    /// </summary>
    public class UriExtensionsTest
    {
        [Fact]
        public void TestToStringRfc()
        {
            new Uri("http://test/absolute path").ToStringRfc().Should().Be("http://test/absolute%20path");
            new Uri("relative%20path", UriKind.Relative).ToStringRfc().Should().Be("relative%20path");

            new Uri(WindowsUtils.IsWindows ? @"C:\absolute path" : "/absolute path").ToStringRfc().Should().Be(
                WindowsUtils.IsWindows ? "file:///C:/absolute%20path" : "file:///absolute%20path");
            new Uri("relative path", UriKind.Relative).ToStringRfc().Should().Be(
                "relative path");
        }

        [Fact]
        public void TestEnsureTrailingSlashAbsolute()
            => new Uri("http://test/test", UriKind.Absolute).EnsureTrailingSlash().Should().Be(
                new Uri("http://test/test/", UriKind.Absolute));

        [Fact]
        public void TestEnsureTrailingSlashRelative()
            => new Uri("test", UriKind.Relative).EnsureTrailingSlash().Should().Be(
                new Uri("test/", UriKind.Relative));

        [Fact]
        public void TestGetLocalFilePath()
        {
            new Uri("http://test/test/my%20file.ext").GetLocalFileName().Should().Be("my file.ext");
            new Uri("file:///test/my%20file.ext").GetLocalFileName().Should().Be("my file.ext");
            new Uri("file:///test/").GetLocalFileName().Should().Be("test");
        }

        [Fact]
        public void TestGetBaseUri()
        {
            new Uri("http://domain/directory/file").GetBaseUri().Should().Be(new Uri("http://domain/directory/"));
            new Uri("http://domain/directory/").GetBaseUri().Should().Be(new Uri("http://domain/directory/"));
            new Uri("http://domain/file").GetBaseUri().Should().Be(new Uri("http://domain/"));
            new Uri("http://domain").GetBaseUri().Should().Be(new Uri("http://domain/"));
        }
    }
}
