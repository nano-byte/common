// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Streams;

/// <summary>
/// Reads the contents of a stream.
/// </summary>
public class ReadStream : TaskBase
{
    private readonly Stream _stream;
    private readonly Action<Stream> _callback;

    /// <summary>
    /// Creates a new stream read task.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="stream">The stream to read.</param>
    /// <param name="callback">Called with a <see cref="ProgressStream"/> wrapped around the <paramref name="stream"/>.</param>
    public ReadStream([Localizable(true)] string name, Stream stream, Action<Stream> callback)
    {
        Name = name;
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    protected override bool UnitsByte => true;

    /// <inheritdoc/>
    protected override void Execute()
    {
        State = TaskState.Header;
        try
        {
            UnitsTotal = _stream.Length;
        }
        catch (NotSupportedException) {}

        State = TaskState.Data;
        using var stream = new ProgressStream(
            _stream,
            new SynchronousProgress<long>(bytes => UnitsProcessed = bytes),
            CancellationToken);
        _callback(stream);
    }
}
