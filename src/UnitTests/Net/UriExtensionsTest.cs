// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

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

    [Theory]
    [InlineData("http://test/test/my%20file.ext", "my file.ext")]
    [InlineData("file:///test/my%20file.ext", "my file.ext")]
    [InlineData("file:///test/", "test")]
    public void TestGetLocalFilePath(string uri, string path)
        => new Uri(uri).GetLocalFileName().Should().Be(path);

    [Theory]
    [InlineData("http://example.com/", "http://example.com/")]
    [InlineData("http://example.com/test", "http://example.com/")]
    [InlineData("http://example.com:80/test", "http://example.com:80/")]
    [InlineData("http://example.com:8080/test", "http://example.com:8080/")]
    [InlineData("./relative", "./relative")]
    [InlineData("file:///test/", "file:///test/")]
    public void TestGetRoot(string uri, string root)
        => new Uri(uri, UriKind.RelativeOrAbsolute).GetRoot().Should().Be(new Uri(root, UriKind.RelativeOrAbsolute));
}
