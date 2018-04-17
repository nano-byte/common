// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains test methods for <see cref="ByteVector4Grid"/>.
    /// </summary>
    public class ByteVector4GridTest
    {
        [Fact]
        public void TestSaveLoad()
        {
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                var grid = new ByteVector4Grid(new[,]
                {
                    {new ByteVector4(0, 1, 2, 3), new ByteVector4(3, 2, 1, 0)},
                    {new ByteVector4(0, 10, 20, 30), new ByteVector4(30, 20, 10, 0)}
                });
                grid.Save(tempFile);

                using (var stream = File.OpenRead(tempFile))
                    grid = ByteVector4Grid.Load(stream);

                grid[0, 0].Should().Be(new ByteVector4(0, 1, 2, 3));
                grid[0, 1].Should().Be(new ByteVector4(3, 2, 1, 0));
                grid[1, 0].Should().Be(new ByteVector4(0, 10, 20, 30));
                grid[1, 1].Should().Be(new ByteVector4(30, 20, 10, 0));
            }
        }
    }
}
