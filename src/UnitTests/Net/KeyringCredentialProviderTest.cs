// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

public class KeyringCredentialProviderTest
{
    private static readonly Uri _uri = new("https://example.com/path");
    private static readonly NetworkCredential
        _keyringCredential = new("keyring-user", "keyring-password"),
        _innerCredential = new("inner-user", "inner-password");

    private class MockKeyring(NetworkCredential? stored, ICredentialProvider? innerProvider = null, Exception? error = null) : KeyringCredentialProvider(innerProvider)
    {
        public int Lookups { get; private set; }

        protected override string KeyringName => "Mock";

        protected override NetworkCredential? Lookup(Uri uri)
        {
            Lookups++;
            if (error != null) throw error;
            return stored;
        }
    }

    private class MockInner : ICredentialProvider
    {
        public readonly List<bool> Calls = [];

        public NetworkCredential GetCredential(Uri uri, bool previousIncorrect = false)
        {
            Calls.Add(previousIncorrect);
            return _innerCredential;
        }
    }

    [Fact]
    public void ReturnsStoredCredential()
    {
        var inner = new MockInner();
        new MockKeyring(_keyringCredential, inner).GetCredential(_uri).Should().BeSameAs(_keyringCredential);
        inner.Calls.Should().BeEmpty();
    }

    [Fact]
    public void FallsBackIfNotStored()
    {
        var inner = new MockInner();
        new MockKeyring(null, inner).GetCredential(_uri).Should().BeSameAs(_innerCredential);
        inner.Calls.Should().Equal(false);
    }

    [Fact]
    public void FallsBackIfKeyringUnavailable()
    {
        var inner = new MockInner();
        new MockKeyring(_keyringCredential, inner, new InvalidOperationException("No keyring")).GetCredential(_uri).Should().BeSameAs(_innerCredential);
    }

    [Fact]
    public void FallsBackIfStoredCredentialRejected()
    {
        var inner = new MockInner();
        var provider = new MockKeyring(_keyringCredential, inner);

        provider.GetCredential(_uri).Should().BeSameAs(_keyringCredential);
        provider.GetCredential(_uri, previousIncorrect: true).Should().BeSameAs(_innerCredential);
        provider.GetCredential(_uri, previousIncorrect: true).Should().BeSameAs(_innerCredential);

        inner.Calls.Should().Equal(false, true); // Only report incorrect credentials that came from the inner provider
        provider.Lookups.Should().Be(1); // Do not query keyring again after rejection
    }

    [Fact]
    public void DoesNotLookUpAgainIfNotFound()
    {
        var inner = new MockInner();
        var provider = new MockKeyring(null, inner);

        provider.GetCredential(_uri).Should().BeSameAs(_innerCredential);
        provider.GetCredential(_uri, previousIncorrect: true).Should().BeSameAs(_innerCredential);

        inner.Calls.Should().Equal(false, true);
        provider.Lookups.Should().Be(1, because: "retries should not cause repeated lookups or unlock prompts");
    }

    [Fact]
    public void NetrcPassesOnRejectionOfKeyringCredential()
    {
        var inner = new MockInner();
        var keyring = new MockKeyring(_keyringCredential, inner);
        var provider = new NetrcCredentialProvider(new Netrc {[_uri.Host] = new("netrc-user", "netrc-password")}, keyring);

        provider.GetCredential(_uri)!.UserName.Should().Be("netrc-user");
        provider.GetCredential(_uri, previousIncorrect: true).Should().BeSameAs(_keyringCredential);
        provider.GetCredential(_uri, previousIncorrect: true).Should().BeSameAs(_innerCredential, because: "the rejection of the keyring credentials must reach the keyring provider");
        provider.GetCredential(_uri, previousIncorrect: true).Should().BeSameAs(_innerCredential);

        inner.Calls.Should().Equal(false, true);
    }

    [Fact]
    public void ReturnsNullWithoutInnerProvider()
        => new MockKeyring(null).GetCredential(_uri).Should().BeNull();

    [Fact]
    public void ForCurrentOSFallsBackForUnknownHost()
    {
        var inner = new MockInner();
        KeyringCredentialProvider.ForCurrentOS(inner)!
                                 .GetCredential(new("https://keyring-test.invalid/"))
                                 .Should().BeSameAs(_innerCredential);
    }
}
