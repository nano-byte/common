// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Contains test methods for <see cref="CompositeCommand"/>.
/// </summary>
public class CompositeCommandTest
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
        var command = new CompositeCommand(
            new MockCommand(() => executeCalls.Add(0), () => undoCalls.Add(0)),
            new MockCommand(() => executeCalls.Add(1), () => undoCalls.Add(1)),
            new MockCommand(() => executeCalls.Add(2), () => undoCalls.Add(2)));

        command.Execute();
        executeCalls.Should().Equal(new[] {0, 1, 2}, because: "Child commands should be executed in ascending order");

        command.Undo();
        undoCalls.Should().Equal(new[] {2, 1, 0}, because: "Child commands should be undone in descending order");
    }

    [Fact]
    public void TestExecuteRollback()
    {
        var executeCalls = new List<int>(3);
        var undoCalls = new List<int>(3);
        var command = new CompositeCommand(
            new MockCommand(() => executeCalls.Add(0), () => undoCalls.Add(0)),
            new MockCommand(() => executeCalls.Add(1), () => undoCalls.Add(1)),
            new MockCommand(() => throw new OperationCanceledException(), () => undoCalls.Add(2)));

        command.Invoking(x => x.Execute()).Should().Throw<OperationCanceledException>(because: "Exceptions should be passed through after rollback");
        executeCalls.Should().Equal(new[] {0, 1}, because: "After an exception the rest of the commands should not be executed");
        undoCalls.Should().Equal(new[] {1, 0}, because: "After an exception all successful executions should be undone");
    }

    [Fact]
    public void TestUndoRollback()
    {
        var executeCalls = new List<int>(3);
        var undoCalls = new List<int>(3);
        var command = new CompositeCommand(
            new MockCommand(() => executeCalls.Add(0), () => throw new OperationCanceledException()),
            new MockCommand(() => executeCalls.Add(1), () => undoCalls.Add(1)),
            new MockCommand(() => executeCalls.Add(2), () => undoCalls.Add(2)));

        command.Execute();
        executeCalls.Should().Equal(new[] {0, 1, 2}, because: "Child commands should be executed in ascending order");

        executeCalls.Clear();
        command.Invoking(x => x.Undo()).Should().Throw<OperationCanceledException>(because: "Exceptions should be passed through after rollback");
        undoCalls.Should().Equal(new[] {2, 1}, because: "After an exception the rest of the undoes should not be performed");
        executeCalls.Should().Equal(new[] {1, 2}, because: "After an exception all successful undoes should be re-executed");
    }

    [Fact]
    public void TestWrongOrder()
    {
        var command = new CompositeCommand();
        command.Invoking(x => x.Undo()).Should().Throw<InvalidOperationException>(because: "Should not allow Undo before Execute");

        command.Execute();
        command.Invoking(x => x.Execute()).Should().Throw<InvalidOperationException>(because: "Should not allow two Executes without an Undo in between");
    }
}