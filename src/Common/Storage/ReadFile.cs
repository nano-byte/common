// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;
using NanoByte.Common.Properties;
using NanoByte.Common.Streams;
using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Reads a file from disk to a stream.
    /// </summary>
    public sealed class ReadFile : TaskBase
    {
        private readonly string _path;
        private readonly Action<Stream> _callback;

        /// <summary>
        /// Creates a new file read task.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <param name="callback">Called with a stream providing the file content.</param>
        /// <param name="name">A name describing the task in human-readable form.</param>
        public ReadFile([Localizable(false)] string path, Action<Stream> callback, [Localizable(true)] string? name = null)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Name = name ?? string.Format(Resources.ReadingFile, _path);
        }

        /// <inheritdoc/>
        public override string Name { get; }

        /// <inheritdoc/>
        protected override bool UnitsByte => true;

        /// <inheritdoc/>
        protected override void Execute()
        {
            State = TaskState.Header;
            UnitsTotal = new FileInfo(_path).Length;

            State = TaskState.Data;
            using var stream = new ProgressStream(
                File.OpenRead(_path),
                new SynchronousProgress<long>(bytes => UnitsProcessed = bytes),
                CancellationToken);
            _callback(stream);
        }
    }
}
