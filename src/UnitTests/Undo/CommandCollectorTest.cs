// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Contains test methods for <see cref="CommandCollector"/>.
    /// </summary>
    public class CommandCollectorTest
    {
        private class MockCommand : IUndoCommand
        {
            public bool Executed { get; private set; }

            public void Execute() => Executed = true;

            public void Undo() => Executed = false;
        }

        /// <summary>
        /// Makes sure <see cref="CommandCollector"/> correctly collects and composes commands.
        /// </summary>
        [Fact]
        public void Test()
        {
            var collector = new CommandCollector();

            var command1 = new MockCommand();
            collector.Execute(command1);
            command1.Executed.Should().BeTrue(because: "Should execute while collecting");
            var command2 = new MockCommand();
            collector.Execute(command2);
            command2.Executed.Should().BeTrue(because: "Should execute while collecting");

            var composite = collector.BuildComposite();
            composite.Execute();
            composite.Undo();
            command1.Executed.Should().BeFalse(because: "Should undo as part of composite");
            // ReSharper disable once HeuristicUnreachableCode
            command2.Executed.Should().BeFalse(because: "Should undo as part of composite");
        }
    }
}
