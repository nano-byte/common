// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Contains test methods for <see cref="AddToCollection{T}"/> and <see cref="RemoveFromCollection{T}"/>.
    /// </summary>
    public class CollectionTest
    {
        [Fact]
        public void TestAddToCollection()
        {
            var collection = new List<string> {"a", "b", "c"};
            var command = new AddToCollection<string>(collection, "d");

            command.Execute();
            collection.Should().Contain("d");

            command.Undo();
            collection.Should().NotContain("d");
        }

        [Fact]
        public void TestRemoveFromCollection()
        {
            var collection = new List<string> {"a", "b", "c"};
            var command = new RemoveFromCollection<string>(collection, "b");

            command.Execute();
            collection.Should().Contain("a");
            collection.Should().NotContain("b");
            collection.Should().Contain("c");

            command.Undo();
            collection.Should().Contain("a");
            collection.Should().Contain("b");
            collection.Should().Contain("c");
        }
    }
}
