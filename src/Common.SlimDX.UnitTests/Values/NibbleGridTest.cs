// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains test methods for <see cref="NibbleGrid"/>.
    /// </summary>
    public class NibbleGridTest
    {
        [Fact]
        public void TestSaveLoad()
        {
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                var grid = new NibbleGrid(new byte[,] {{2, 4}, {5, 10}});
                grid.Save(tempFile);

                using (var stream = File.OpenRead(tempFile))
                    grid = NibbleGrid.Load(stream);

                grid[0, 0].Should().Be(2);
                grid[0, 1].Should().Be(4);
                grid[1, 0].Should().Be(5);
                grid[1, 1].Should().Be(10);
            }
        }
    }
}
