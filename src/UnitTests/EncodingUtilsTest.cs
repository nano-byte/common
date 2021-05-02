// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Security.Cryptography;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common
{
    public class EncodingUtilsTest
    {
        [Fact]
        public void TestHash()
        {
            const string sha1ForEmptyString = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
            "".Hash(SHA1.Create()).Should().Be(sha1ForEmptyString);
        }

        [Fact]
        public void TestBase64Utf8Encode()
        {
            "".Base64Utf8Encode().Should().Be("");
            "test".Base64Utf8Encode().Should().Be("dGVzdA==");
        }

        [Fact]
        public void TestBase64Utf8Decode()
        {
            "".Base64Utf8Decode().Should().Be("");
            "dGVzdA==".Base64Utf8Decode().Should().Be("test");
        }

        [Fact]
        public void TestBase32Encode() => new byte[] {65, 66}.Base32Encode().Should().Be("IFBA");

        [Fact]
        public void TestBase16Encode() => new byte[] {65, 66}.Base16Encode().Should().Be("4142");

        [Fact]
        public void TestBase16Decode() => "4142".Base16Decode().Should().Equal(65, 66);
    }
}
