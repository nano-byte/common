// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="MonitoredCollection{T}"/>.
/// </summary>
public class MonitoredCollectionTest
{
    [Fact]
    public void AddRaisesEvents()
    {
        var collection = new MonitoredCollection<string>();
        int changedCount = 0;
        int addedCount = 0;
        string? addedItem = null;

        collection.Changed += () => changedCount++;
        collection.Added += item =>
        {
            addedCount++;
            addedItem = item;
        };

        collection.Add("test");

        changedCount.Should().Be(1);
        addedCount.Should().Be(1);
        addedItem.Should().Be("test");
    }

    [Fact]
    public void RemoveRaisesEvents()
    {
        var collection = new MonitoredCollection<string> {"test"};
        int removingCount = 0;
        int removedCount = 0;
        string? removingItem = null;
        string? removedItem = null;

        collection.Removing += item =>
        {
            removingCount++;
            removingItem = item;
        };
        collection.Removed += item =>
        {
            removedCount++;
            removedItem = item;
        };

        collection.Remove("test");

        removingCount.Should().Be(1);
        removingItem.Should().Be("test");
        removedCount.Should().Be(1);
        removedItem.Should().Be("test");
    }

    [Fact]
    public void ClearRaisesEvents()
    {
        var collection = new MonitoredCollection<string> {"item1", "item2"};
        int changedCount = 0;
        int removedCount = 0;

        collection.Changed += () => changedCount++;
        collection.Removed += _ => removedCount++;

        collection.Clear();

        changedCount.Should().Be(1);
        removedCount.Should().Be(2);
    }

    [Fact]
    public void InsertRaisesEvents()
    {
        var collection = new MonitoredCollection<string>();
        int changedCount = 0;
        int addedCount = 0;

        collection.Changed += () => changedCount++;
        collection.Added += _ => addedCount++;

        collection.Insert(0, "test");

        changedCount.Should().Be(1);
        addedCount.Should().Be(1);
    }

    [Fact]
    public void SetItemRaisesEvents()
    {
        var collection = new MonitoredCollection<string> {"old"};
        int changedCount = 0;
        int removedCount = 0;
        int addedCount = 0;

        collection.Changed += () => changedCount++;
        collection.Removed += _ => removedCount++;
        collection.Added += _ => addedCount++;

        collection[0] = "new";

        changedCount.Should().Be(1);
        removedCount.Should().Be(1);
        addedCount.Should().Be(1);
    }

    [Fact]
    public void MaxElementsLimitEnforced()
    {
        var collection = new MonitoredCollection<string>(maxElements: 2);
        collection.Add("item1");
        collection.Add("item2");

        collection.Invoking(c => c.Add("item3"))
                  .Should().Throw<InvalidOperationException>();
    }
}
