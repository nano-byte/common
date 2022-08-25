// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Remembers the latest call made to <see cref="Report"/>. Forwards that call (if any) and all future calls to a target <see cref="IProgress{T}"/> implementation once it is set.
/// </summary>
/// <typeparam name="T">The type of progress update value.</typeparam>
/// <remarks>
/// If <see cref="Report"/> and <see cref="SetTarget"/> are called on different threads individual progress reports may be lost.
/// This tradeoff is made intentionally to avoid locking for better performance.
/// </remarks>
public class DeferredProgress<T> : IProgress<T>
{
    private IProgress<T>? _target;
    private T? _lastValue;
    private bool _hasValue;

    /// <summary>
    /// Sets the target <see cref="IProgress{T}"/> implementation to forward <see cref="Report"/> calls to.
    /// </summary>
    /// <exception cref="InvalidOperationException">The target is already set.</exception>
    public void SetTarget(IProgress<T> target)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (_target != null) throw new InvalidOperationException("The target is already set.");

        if (_hasValue && _lastValue != null)
            target.Report(_lastValue);

        _target = target;
    }

    /// <inheritdoc/>
    public void Report(T value)
    {
        if (_target == null)
        {
            _lastValue = value;
            _hasValue = true;
        }
        else _target.Report(value);
    }
}
