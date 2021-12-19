// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

/// <summary>
/// Runs jobs on a single background thread that is started and stopped on demand.
/// </summary>
public class JobQueue
{
    private readonly object _lock = new();
    private readonly Queue<Action> _jobs = new();
    private bool _threadRunning;

    /// <summary>
    /// Adds a job to the work queue.
    /// </summary>
    public void Enqueue(Action job)
    {
        lock (_lock)
        {
            _jobs.Enqueue(job);

            if (!_threadRunning)
            {
                _threadRunning = true;
                ThreadUtils.StartBackground(Work, name: nameof(JobQueue));
            }
        }
    }

    private void Work()
    {
        while (true)
        {
            Action job;
            lock (_lock)
            {
                if (_jobs.Count == 0)
                {
                    _threadRunning = false;
                    return;
                }

                job = _jobs.Dequeue();
            }

            job();
        }
    }
}