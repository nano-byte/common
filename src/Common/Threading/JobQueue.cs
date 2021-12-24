// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

/// <summary>
/// Runs jobs on a single background thread that is started and stopped on demand.
/// </summary>
public class JobQueue
{
    private readonly CancellationToken _cancellationToken;
    private readonly object _lock = new();
    private readonly Queue<Action> _jobs = new();
    private bool _threadRunning;

    /// <summary>
    /// Creates a new job queue.
    /// </summary>
    /// <param name="cancellationToken">Used to stop processing jobs.</param>
    public JobQueue(CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;
    }

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

#if !NET20
            using (new CancellationGuard(_cancellationToken))
#endif
            {
                if (_cancellationToken.IsCancellationRequested) return;
                job();
            }
        }
    }
}
