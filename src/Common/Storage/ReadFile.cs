// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Streams;
using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Storage;

/// <summary>
/// Reads a file from disk to a stream.
/// </summary>
/// <param name="path">The path of the file to read.</param>
/// <param name="callback">Called with a stream providing the file content.</param>
/// <param name="name">A name describing the task in human-readable form.</param>
public sealed class ReadFile([Localizable(false)] string path, Action<Stream> callback, [Localizable(true)] string? name = null) : TaskBase
{
    /// <inheritdoc/>
    public override string Name { get; } = name ?? string.Format(Resources.ReadingFile, path);

    /// <inheritdoc/>
    protected override bool UnitsByte => true;

    /// <inheritdoc/>
    protected override void Execute()
    {
        State = TaskState.Header;
        UnitsTotal = new FileInfo(path).Length;

        State = TaskState.Data;
        using var stream = new ProgressStream(
            File.OpenRead(path),
            new SynchronousProgress<long>(bytes => UnitsProcessed = bytes),
            CancellationToken);
        callback(stream);
    }
}
