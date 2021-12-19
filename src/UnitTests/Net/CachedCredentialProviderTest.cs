// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Contains unit tests for <see cref="CachedCredentialProvider"/>
/// </summary>
public class CachedCredentialProviderTest : IDisposable
{
    private readonly Mock<ICredentialProvider> _innerMock = new(MockBehavior.Strict);
    private readonly CachedCredentialProvider _provider;

    public CachedCredentialProviderTest()
    {
        _provider = new CachedCredentialProvider(_innerMock.Object);
    }

    public void Dispose() => _innerMock.VerifyAll();

    [Fact]
    public void TestGetCredential()
    {
        var uriOuter = new Uri("http://domain/dir/file");
        var uriInner = new Uri("http://domain/dir/");
        var credential = new NetworkCredential("userName", "password");

        _innerMock.Setup(x => x.GetCredential(uriInner, null!)).Returns(credential);

        _provider.GetCredential(uriOuter, null!).Should().BeSameAs(credential);
        _provider.GetCredential(uriOuter, null!).Should().BeSameAs(credential);

        _innerMock.Verify(x => x.GetCredential(uriInner, null!), Times.Exactly(1));
    }

    [Fact]
    public void TestReportInvalid()
    {
        var uriOuter = new Uri("http://domain/dir/file");
        var uriInner = new Uri("http://domain/dir/");

        _innerMock.Setup(x => x.ReportInvalid(uriInner));

        _provider.ReportInvalid(uriOuter);
    }
}