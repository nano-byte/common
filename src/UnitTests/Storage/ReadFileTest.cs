// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using FluentAssertions;
using NanoByte.Common.Streams;
using Xunit;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Contains test methods for <see cref="ReadFile"/>.
    /// </summary>
    public class ReadFileTest
    {
        [Fact]
        public void TestRun()
        {
            var tempFile = new TemporaryFile("unit-tests");
            File.WriteAllBytes(tempFile, new byte[] {1, 2, 3});

            ArraySegment<byte> data = default;
            new ReadFile(tempFile, stream => data = stream.ReadAll()).Run();
            data.Should().Equal(1, 2, 3);
        }
    }
}
