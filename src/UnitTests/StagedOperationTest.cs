// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

public class StagedOperationTest
{
    private sealed class MockStagedOperation : StagedOperation
    {
        public bool Staged { get; private set; }
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        protected override void OnStage() => Staged = true;
        protected override void OnCommit() => Committed = true;
        protected override void OnRollback() => RolledBack = true;
    }

    [Fact]
    public void StageThenCommitDoesNotRollBack()
    {
        var op = new MockStagedOperation();
        op.Stage();
        op.Commit();
        op.Dispose();

        op.Staged.Should().BeTrue();
        op.Committed.Should().BeTrue();
        op.RolledBack.Should().BeFalse();
    }

    [Fact]
    public void DisposeWithoutCommitTriggersRollback()
    {
        var op = new MockStagedOperation();
        op.Stage();
        op.Dispose();

        op.RolledBack.Should().BeTrue();
        op.Committed.Should().BeFalse();
    }

    [Fact]
    public void DisposeWithoutStageDoesNothing()
    {
        var op = new MockStagedOperation();
        op.Dispose();

        op.Staged.Should().BeFalse();
        op.Committed.Should().BeFalse();
        op.RolledBack.Should().BeFalse();
    }

    [Fact]
    public void StageCannotBeCalledTwice()
    {
        var op = new MockStagedOperation();
        op.Stage();

        op.Invoking(x => x.Stage()).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CommitWithoutStageThrows()
    {
        var op = new MockStagedOperation();

        op.Invoking(x => x.Commit()).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CommitCannotBeCalledTwice()
    {
        var op = new MockStagedOperation();
        op.Stage();
        op.Commit();

        op.Invoking(x => x.Commit()).Should().Throw<InvalidOperationException>();
    }
}
