// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Contains test methods for <see cref="PreExecutedCompositeCommand"/>.
/// </summary>
public class PreExecutedCompositeCommandTest
{
    private class MockCommand : IUndoCommand
    {
        private readonly Action _executeCallback, _undoCallback;

        public MockCommand(Action executeCallback, Action undoCallback)
        {
            _executeCallback = executeCallback;
            _undoCallback = undoCallback;
        }

        public void Execute() => _executeCallback();

        public void Undo() => _undoCallback();
    }

    [Fact]
    public void TestExecuteUndo()
    {
        var executeCalls = new List<int>(3);
        var undoCalls = new List<int>(3);
        var command = new PreExecutedCompositeCommand([
            new MockCommand(() => executeCalls.Add(0), () => undoCalls.Add(0)),
            new MockCommand(() => executeCalls.Add(1), () => undoCalls.Add(1)),
            new MockCommand(() => executeCalls.Add(2), () => undoCalls.Add(2))
        ]);

        command.Execute();
        executeCalls.Should().BeEmpty(because: "First execution should do nothing");

        command.Undo();
        undoCalls.Should().Equal([2, 1, 0], because: "Child commands should be undone in descending order");

        command.Execute();
        executeCalls.Should().Equal([0, 1, 2], because: "Child commands should be executed in ascending order");
    }
}
