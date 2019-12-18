// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Contains test methods for <see cref="Merge"/>.
    /// </summary>
    public class MergeTest
    {
        #region Inner class
        private class MergeTestData : IMergeable<MergeTestData>
        {
            public string MergeID { get; set; }

            public string Data { get; set; }

            public DateTime Timestamp { get; set; }

            public static IEnumerable<MergeTestData> BuildList(params string[] mergeIDs) => mergeIDs.Select(value => new MergeTestData {MergeID = value}).ToList();

            public override string ToString() => MergeID + " (" + Data + ")";

            #region Equality
            public bool Equals(MergeTestData other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.MergeID, MergeID) && Equals(other.Data, Data);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == typeof(MergeTestData) && Equals((MergeTestData)obj);
            }

            public override int GetHashCode()
                => HashCode.Combine(MergeID, Data);
            #endregion
        }
        #endregion

        /// <summary>
        /// Ensures that <see cref="Merge.TwoWay{T}"/> correctly detects added and removed elements.
        /// </summary>
        [Fact]
        public void TestMergeSimple()
        {
            ICollection<int> toRemove = new List<int>();
            ICollection<int> toAdd = new List<int>();
            Merge.TwoWay(
                theirs: new[] {16, 8, 4},
                mine: new[] {1, 2, 4},
                added: toAdd.Add, removed: toRemove.Add);

            toAdd.Should().Equal(16, 8);
            toRemove.Should().Equal(1, 2);
        }

        /// <summary>
        /// Ensures that <see cref="Merge.ThreeWay{T}"/> correctly detects unchanged lists.
        /// </summary>
        [Fact]
        public void TestMergeEquals()
        {
            var list = new[] {new MergeTestData {MergeID = "1"}};

            Merge.ThreeWay(reference: new MergeTestData[0], theirs: list, mine: list,
                added: element => throw new AssertionFailedException(element + " should not be detected as added."),
                removed: element => throw new AssertionFailedException(element + " should not be detected as removed."));
        }

        /// <summary>
        /// Ensures that <see cref="Merge.ThreeWay{T}"/> correctly detects added and removed elements.
        /// </summary>
        [Fact]
        public void TestMergeAddAndRemove()
        {
            ICollection<MergeTestData> toRemove = new List<MergeTestData>();
            ICollection<MergeTestData> toAdd = new List<MergeTestData>();
            Merge.ThreeWay(
                reference: MergeTestData.BuildList("a", "b", "c"),
                theirs: MergeTestData.BuildList("a", "b", "d"),
                mine: MergeTestData.BuildList("a", "c", "e"),
                added: toAdd, removed: toRemove);

            toAdd.Should().Equal(MergeTestData.BuildList("d"));
            toRemove.Should().Equal(MergeTestData.BuildList("c"));
        }

        /// <summary>
        /// Ensures that <see cref="Merge.ThreeWay{T}"/> correctly modified elements.
        /// </summary>
        [Fact]
        public void TestMergeModify()
        {
            var reference = MergeTestData.BuildList("a", "b", "c", "d", "e");
            var theirs = new[]
            {
                new MergeTestData {MergeID = "a"},
                new MergeTestData {MergeID = "b", Data = "123", Timestamp = new DateTime(2000, 1, 1)},
                new MergeTestData {MergeID = "c"},
                new MergeTestData {MergeID = "d", Data = "456", Timestamp = new DateTime(2000, 1, 1)},
                new MergeTestData {MergeID = "e", Data = "789", Timestamp = new DateTime(2999, 1, 1)}
            };
            var mine = new[]
            {
                new MergeTestData {MergeID = "a"},
                new MergeTestData {MergeID = "b"},
                new MergeTestData {MergeID = "c", Data = "abc", Timestamp = new DateTime(2000, 1, 1)},
                new MergeTestData {MergeID = "d", Data = "def", Timestamp = new DateTime(2999, 1, 1)},
                new MergeTestData {MergeID = "e", Data = "ghi", Timestamp = new DateTime(2000, 1, 1)}
            };

            ICollection<MergeTestData> toRemove = new List<MergeTestData>();
            ICollection<MergeTestData> toAdd = new List<MergeTestData>();
            Merge.ThreeWay(reference, theirs, mine, toAdd, toRemove);

            toRemove.Should().Equal(mine[1], mine[4]);
            toAdd.Should().Equal(theirs[1], theirs[4]);
        }
    }
}
