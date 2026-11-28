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
        public readonly List<(Uri Uri, NetworkCredential Credential)> Stored = [];

        protected override string KeyringName => "Mock";

        protected override NetworkCredential? Lookup(Uri uri)
        {
            Lookups++;
            if (error != null) throw error;
            return stored;
        }

        public readonly List<string> Operations = [];

        protected override void Store(Uri uri, NetworkCredential credential)
        {
            if (error != null) throw error;
            Stored.Add((uri, credential));
            Operations.Add("store:" + credential.UserName);
        }

        public readonly List<string> Deleted = [];
        public Exception? DeleteError { get; set; }

        protected override void Delete(Uri uri, string userName)
        {
            if ((error ?? DeleteError) is {} ex) throw ex;
            Deleted.Add(userName);
            Operations.Add("delete:" + userName);
        }
    }

    private class MockInner(NetworkCredential? credential = null) : ICredentialProvider, ICredentialFeedback
    {
        public readonly List<bool> Calls = [];
        public readonly List<NetworkCredential> Accepted = [];

        public NetworkCredential GetCredential(Uri uri, bool previousIncorrect = false)
        {
            Calls.Add(previousIncorrect);
            return credential ?? _innerCredential;
        }

        void ICredentialFeedback.Accepted(Uri uri, NetworkCredential credential) => Accepted.Add(credential);
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
    public void SavesAcceptedCredentialIfRequested()
    {
        var prompted = new PromptedCredential("user", "password", remember: true);
        var inner = new MockInner(prompted);
        var provider = new MockKeyring(null, inner);

        var credential = provider.GetCredential(_uri)!;
        provider.Accepted(_uri, credential);

        provider.Stored.Should().Equal((_uri, prompted));
        inner.Accepted.Should().Equal(prompted);
    }

    [Fact]
    public void ReplacesRejectedCredential()
    {
        var prompted = new PromptedCredential("other-user", "password", remember: true);
        var provider = new MockKeyring(_keyringCredential, new MockInner(prompted));

        provider.GetCredential(_uri).Should().BeSameAs(_keyringCredential);
        provider.GetCredential(_uri, previousIncorrect: true).Should().BeSameAs(prompted);
        provider.Accepted(_uri, prompted);

        provider.Stored.Should().Equal((_uri, prompted));
        provider.Deleted.Should().Equal(_keyringCredential.UserName);

        provider.GetCredential(_uri);
        provider.Lookups.Should().Be(2, because: "the keyring should be queried again once it contains working credentials");
    }

    [Fact]
    public void DeletesRejectedCredentialForSameUserBeforeStoring()
    {
        var prompted = new PromptedCredential(_keyringCredential.UserName, "new-password", remember: true);
        var provider = new MockKeyring(_keyringCredential, new MockInner(prompted));

        provider.GetCredential(_uri);
        provider.GetCredential(_uri, previousIncorrect: true);
        provider.Accepted(_uri, prompted);

        provider.Stored.Should().Equal((_uri, prompted));
        provider.Operations.Should().Equal(
            new[] {"delete:" + _keyringCredential.UserName, "store:" + _keyringCredential.UserName},
            because: "Store() may not replace items stored with different attributes, e.g., by other tools");
    }

    [Fact]
    public void StoresEvenIfDeletingRejectedCredentialFails()
    {
        var prompted = new PromptedCredential("other-user", "password", remember: true);
        var provider = new MockKeyring(_keyringCredential, new MockInner(prompted)) {DeleteError = new InvalidOperationException("Locked")};

        provider.GetCredential(_uri);
        provider.GetCredential(_uri, previousIncorrect: true);
        provider.Accepted(_uri, prompted);

        provider.Stored.Should().Equal((_uri, prompted));
    }

    [Fact]
    public void DoesNotDeleteRejectedCredentialIfNotRequested()
    {
        var prompted = new PromptedCredential("other-user", "password", remember: false);
        var provider = new MockKeyring(_keyringCredential, new MockInner(prompted));

        provider.GetCredential(_uri);
        provider.GetCredential(_uri, previousIncorrect: true);
        provider.Accepted(_uri, prompted);

        provider.Deleted.Should().BeEmpty();
    }

    [Fact]
    public void DoesNotSaveIfNotRequested()
    {
        var provider = new MockKeyring(null);
        provider.Accepted(_uri, new PromptedCredential("user", "password", remember: false));
        provider.Accepted(_uri, _innerCredential);
        provider.Stored.Should().BeEmpty();
    }

    [Fact]
    public void DoesNotSaveCredentialFromKeyring()
    {
        var provider = new MockKeyring(_keyringCredential);
        provider.Accepted(_uri, provider.GetCredential(_uri)!);
        provider.Stored.Should().BeEmpty();
    }

    [Fact]
    public void IgnoresSaveErrors()
    {
        var provider = new MockKeyring(null, error: new InvalidOperationException("No keyring"));
        provider.Invoking(x => x.Accepted(_uri, new PromptedCredential("user", "password", remember: true))).Should().NotThrow();
    }

    [Fact]
    public void NetrcForwardsAccepted()
    {
        var inner = new MockInner();
        new NetrcCredentialProvider(inner).Accepted(_uri, _innerCredential);
        inner.Accepted.Should().Equal(_innerCredential);
    }

    [Fact]
    public void ForCurrentOSFallsBackForUnknownHost()
    {
        var inner = new MockInner();
        KeyringCredentialProvider.ForCurrentOS(inner)!
                                 .GetCredential(new("https://keyring-test.invalid/"))
                                 .Should().BeSameAs(_innerCredential);
    }
}
