// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions.Execution;

namespace NanoByte.Common.Dispatch;

/// <summary>
/// Contains test methods for <see cref="Merge"/>.
/// </summary>
public class MergeTest
{
    #region Inner class
    private class MergeTestData : IMergeable<MergeTestData>
    {
        public string MergeID { get; }

        public string? Data { get; }

        public DateTime Timestamp { get; }

        public MergeTestData(string mergeID, string? data = null, DateTime timestamp = default)
        {
            MergeID = mergeID;
            Data = data;
            Timestamp = timestamp;
        }

        public static IEnumerable<MergeTestData> BuildList(params string[] mergeIDs)
            => mergeIDs.Select(mergeID => new MergeTestData(mergeID)).ToList();

        public override string ToString() => $"{MergeID} ({Data})";

        #region Equality
        public bool Equals(MergeTestData? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.MergeID, MergeID) && Equals(other.Data, Data);
        }

        public override bool Equals(object? obj)
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
        var list = new[] {new MergeTestData(mergeID: "1")};

        Merge.ThreeWay(reference: Array.Empty<MergeTestData>(), theirs: list, mine: list,
            added: element => throw new AssertionFailedException($"{element} should not be detected as added."),
            removed: element => throw new AssertionFailedException($"{element} should not be detected as removed."));
    }

    /// <summary>
    /// Ensures that <see cref="Merge.ThreeWay{T}"/> correctly detects added and removed elements.
    /// </summary>
    [Fact]
    public void TestMergeAddAndRemove()
    {
        var toRemove = new List<MergeTestData>();
        var toAdd = new List<MergeTestData>();
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
            new MergeTestData(mergeID: "a"),
            new MergeTestData(mergeID: "b", data: "123", timestamp: new DateTime(2000, 1, 1)),
            new MergeTestData(mergeID: "c"),
            new MergeTestData(mergeID: "d", data: "456", timestamp: new DateTime(2000, 1, 1)),
            new MergeTestData(mergeID: "e", data: "789", timestamp: new DateTime(2999, 1, 1))
        };
        var mine = new[]
        {
            new MergeTestData(mergeID: "a"),
            new MergeTestData(mergeID: "b"),
            new MergeTestData(mergeID: "c", data: "abc", timestamp: new DateTime(2000, 1, 1)),
            new MergeTestData(mergeID: "d", data: "def", timestamp: new DateTime(2999, 1, 1)),
            new MergeTestData(mergeID: "e", data: "ghi", timestamp: new DateTime(2000, 1, 1))
        };

        var toRemove = new List<MergeTestData>();
        var toAdd = new List<MergeTestData>();
        Merge.ThreeWay(reference, theirs, mine, toAdd, toRemove);

        toRemove.Should().Equal(mine[1], mine[4]);
        toAdd.Should().Equal(theirs[1], theirs[4]);
    }
}