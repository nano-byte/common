// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Contains test methods for <see cref="ReplaceInList{T}"/>.
    /// </summary>
    public class ReplaceInListTest
    {
        /// <summary>
        /// Makes sure <see cref="ReplaceInList{T}"/> correctly performs executions and undos.
        /// </summary>
        [Fact]
        public void TestExecute()
        {
            var list = new List<string> {"a", "b", "c"};
            var command = new ReplaceInList<string>(list, "b", "x");

            command.Execute();
            list.Should().Equal("a", "x", "c");

            command.Undo();
            list.Should().Equal("a", "b", "c");
        }
    }
}
