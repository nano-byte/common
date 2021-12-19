// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="CollectionExtensions"/>.
    /// </summary>
    public class CollectionExtensionsTest
    {
        /// <summary>
        /// Ensures that <see cref="CollectionExtensions.AddIfNew{T}"/> correctly detects pre-existing entries.
        /// </summary>
        [Fact]
        public void TestAddIfNew()
        {
            var list = new List<string> {"a", "b", "c"};

            list.AddIfNew("b").Should().BeFalse();
            list.Should().Equal("a", "b", "c");

            list.AddIfNew("d").Should().BeTrue();
            list.Should().Equal("a", "b", "c", "d");

            list.Invoking(x => x.RemoveLast(-1)).Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
