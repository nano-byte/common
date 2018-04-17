// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains test methods for <see cref="ExpandableRectangleArray{T}"/>.
    /// </summary>
    public class ExpandableRectangleArrayTest
    {
        [Fact]
        public void TestGetArrayBase()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(1, 1), new[,] {{5, 6}, {7, 8}});
            expandableRectangleArray.AddFirst(new Point(2, 2), new[,] {{1, 2}, {3, 4}});

            var result = expandableRectangleArray.GetArray(new[,] {{-1, -2, -3, -4}, {-5, -6, -7, -8}, {-9, -10, -11, -12}, {-13, -14, -15, -16}});
            result.Should().Equal(new[,] {{5, 6, -8}, {7, 8, 2}, {-14, 3, 4}});
            expandableRectangleArray.TotalArea.Should().Be(new Rectangle(1, 1, 3, 3));
        }

        [Fact]
        public void TestGetArraySmallBase()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(1, 1), new[,] {{5, 6}, {7, 8}});
            expandableRectangleArray.AddFirst(new Point(2, 2), new[,] {{1, 2}, {3, 4}});

            var result = expandableRectangleArray.GetArray(new[,] {{-1, -2, -3}, {-5, -6, -7}, {-9, -10, -11}, {-13, -14, -15}});
            result.Should().Equal(new[,] {{5, 6}, {7, 8}, {-14, 3}});
            expandableRectangleArray.TotalArea.Should().Be(new Rectangle(1, 1, 3, 3));
        }

        [Fact]
        public void TestGetArrayNoBase()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(1, 1), new[,] {{5, 6}, {7, 8}});
            expandableRectangleArray.AddFirst(new Point(2, 2), new[,] {{1, 2}, {3, 4}});

            var result = expandableRectangleArray.GetArray();
            result.Should().Equal(new[,] {{5, 6, 0}, {7, 8, 2}, {0, 3, 4}});
            expandableRectangleArray.TotalArea.Should().Be(new Rectangle(1, 1, 3, 3));
        }

        [Fact]
        public void TestNegativeArea()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(-1, -1), new[,] {{1, 2}, {1, 2}});

            var result = expandableRectangleArray.GetArray();
            result.Should().Equal(new[,] {{2}});
            expandableRectangleArray.TotalArea.Should().Be(new Rectangle(0, 0, 1, 1));
        }
    }
}
