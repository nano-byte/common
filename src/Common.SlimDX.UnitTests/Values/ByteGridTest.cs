// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains test methods for <see cref="ByteGrid"/>.
    /// </summary>
    public class ByteGridTest
    {
        [Fact]
        public void TestSaveLoad()
        {
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                var grid = new ByteGrid(new byte[,] {{10, 20}, {100, 200}});
                grid.Save(tempFile);

                using (var stream = File.OpenRead(tempFile))
                    grid = ByteGrid.Load(stream);

                grid[0, 0].Should().Be(10);
                grid[0, 1].Should().Be(20);
                grid[1, 0].Should().Be(100);
                grid[1, 1].Should().Be(200);
            }
        }

        [Fact]
        public void TestSampledRead()
        {
            new ByteGrid(new byte[,] {{10}, {0}}).SampledRead(0.5f, 0).Should().Be(5);
            new ByteGrid(new byte[,] {{10, 0}}).SampledRead(0.5f, 0.5f).Should().Be(5);
            new ByteGrid(new byte[,] {{10, 5}, {0, 0}}).SampledRead(0.5f, 0.5f).Should().Be(3.75f);
        }
    }
}
