// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Streams;

/// <summary>
/// Reads the contents of a stream.
/// </summary>
/// <param name="name">A name describing the task in human-readable form.</param>
/// <param name="stream">The stream to read.</param>
/// <param name="callback">Called with a <see cref="ProgressStream"/> wrapped around the <paramref name="stream"/>.</param>
public class ReadStream([Localizable(true)] string name, Stream stream, Action<Stream> callback) : TaskBase
{
    /// <inheritdoc/>
    public override string Name { get; } = name;

    /// <inheritdoc/>
    protected override bool UnitsByte => true;

    /// <inheritdoc/>
    protected override void Execute()
    {
        State = TaskState.Header;
        try
        {
            UnitsTotal = stream.Length;
        }
        catch (NotSupportedException) {}

        State = TaskState.Data;
        using var progressStream = new ProgressStream(
            stream,
            new SynchronousProgress<long>(bytes => UnitsProcessed = bytes),
            CancellationToken);
        callback(progressStream);
    }
}
