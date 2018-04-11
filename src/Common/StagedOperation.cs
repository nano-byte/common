/*
 * Copyright 2006-2016 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;

namespace NanoByte.Common
{
    /// <summary>
    /// Common base class for operations that are first staged and then either commited or rolled back.
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _stageStarted && !_commitCompleted) OnRollback();
        }

        /// <summary>
        /// Template method to revert any changes made by <see cref="OnStage"/>.
        /// </summary>
        protected abstract void OnRollback();
    }
}
