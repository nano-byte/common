// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Native
{
    public class CygwinUtilsTest
    {
        private static readonly byte[] _symlinkBytes
            = CygwinUtils.SymlinkCookie
                         .Concat(Encoding.Unicode.GetPreamble())
                         .Concat(Encoding.Unicode.GetBytes("target\0"))
                         .ToArray();

        [Fact]
        public void TestIsSymlinkNoMatch()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string normalFile = Path.Combine(tempDir, "normal");
            FileUtils.Touch(normalFile);

            CygwinUtils.IsSymlink(normalFile).Should().BeFalse();

            CygwinUtils.IsSymlink(normalFile, out _).Should().BeFalse();
        }

        [SkippableFact]
        public void TestIsSymlinkMatch()
        {
            Skip.IfNot(WindowsUtils.IsWindows, reason: "The 'system' file attribute can only be checked on the Windows platform.");

            using var tempDir = new TemporaryDirectory("unit-tests");
            string symlinkFile = Path.Combine(tempDir, "symlink");
            File.WriteAllBytes(symlinkFile, _symlinkBytes);
            File.SetAttributes(symlinkFile, FileAttributes.System);

            CygwinUtils.IsSymlink(symlinkFile).Should().BeTrue();

            CygwinUtils.IsSymlink(symlinkFile, out string? target).Should().BeTrue();
            target.Should().Be("target");
        }

        [SkippableFact]
        public void TestCreateSymlink()
        {
            Skip.IfNot(WindowsUtils.IsWindows, reason: "The 'system' file attribute can only be set on the Windows platform.");

            using var tempDir = new TemporaryDirectory("unit-tests");
            string symlinkFile = Path.Combine(tempDir, "symlink");
            CygwinUtils.CreateSymlink(symlinkFile, "target");

            File.Exists(symlinkFile).Should().BeTrue();
            File.ReadAllBytes(symlinkFile).Should().Equal(_symlinkBytes);
        }
    }
}
