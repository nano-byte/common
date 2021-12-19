// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Common base class for operations that are first staged and then either committed or rolled back.
/// </summary>
public abstract class StagedOperation : IDisposable
{
    /// <summary>
    /// Stages changes for later <see cref="Commit"/> or rollback.
    /// </summary>
    public void Stage()
    {
        if (_stageStarted) throw new InvalidOperationException("Stage() can only be called once!");

        _stageStarted = true;
        OnStage();
    }

    private bool _stageStarted;

    /// <summary>
    /// Template method to stage changes.
    /// </summary>
    protected abstract void OnStage();

    /// <summary>
    /// Commits the <see cref="Stage"/>d changes.
    /// </summary>
    public void Commit()
    {
        if (!_stageStarted) throw new InvalidOperationException("Stage() must be called first!");
        if (_commitCompleted) throw new InvalidOperationException("Commit() can only be called once!");

        OnCommit();
        _commitCompleted = true;
    }

    private bool _commitCompleted;

    /// <summary>
    /// Template method to commit the changes made by <see cref="OnStage"/>.
    /// </summary>
    protected abstract void OnCommit();

    /// <summary>
    /// Performs a rollback of all changes made by <see cref="Stage"/> if <see cref="Commit"/> has not been called and completed yet.
    /// </summary>
    public virtual void Dispose()
    {
        if (_stageStarted && !_commitCompleted) OnRollback();
    }

    /// <summary>
    /// Template method to revert any changes made by <see cref="OnStage"/>.
    /// </summary>
    protected abstract void OnRollback();
}