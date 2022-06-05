// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Storage;

namespace NanoByte.Common.Net;

public class NetrcTest
{
    [Fact]
    public void TestParse()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        File.WriteAllText(tempFile, "machine example.com login user1 unknown extension password password1\nmachine 0install.net login user2 password password2");
        var netrc = Netrc.Load(tempFile);

        netrc.Should().BeEquivalentTo(new Netrc
        {
            ["example.com"] = new("user1", "password1"),
            ["0install.net"] = new("user2", "password2")
        });
    }
}
