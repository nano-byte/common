// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

/// <summary>
/// Provides a wrapper around <see cref="Mutex"/> that automatically acquires on creating and releases on <see cref="Dispose"/>.
/// </summary>
/// <example>
/// Instead of <c>lock (_object) { code(); }</c> for per-process locking use
/// <c>using (new MutexLock("name") { code(); }</c> for inter-process locking.
/// </example>
/// <remarks>Automatically handles <see cref="AbandonedMutexException"/> with <see cref="Log.Warn(string,System.Exception?)"/>.</remarks>
[MustDisposeResource]
public sealed class MutexLock : IDisposable
{
    private readonly Mutex _mutex;

    /// <summary>
    /// Acquires <see cref="Mutex"/> with <paramref name="name"/>.
    /// </summary>
    public MutexLock(string name)
    {
        _mutex = new(false, name);
        try
        {
            _mutex.WaitOne();
        }
        catch (AbandonedMutexException)
        {
            Log.Warn($"Mutex '{name}' was abandoned by another instance");
            // Abandoned mutexes get acquired despite exception
        }
    }

    /// <summary>
    /// Releases the <see cref="Mutex"/>.
    /// </summary>
    public void Dispose()
    {
        try
        {
            _mutex.ReleaseMutex();
        }
        finally
        {
            _mutex.Close();
        }
    }
}
