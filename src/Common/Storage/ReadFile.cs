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
        /// <inheritdoc/>
        public override string Name => string.Format(Resources.ReadingFile, Path);

        /// <inheritdoc/>
        protected override bool UnitsByte => true;

        /// <summary>
        /// The path of the file to read.
        /// </summary>
        [Description("The path of the file to read.")]
        public string Path { get; }

        private readonly Action<Stream> _stream;

        /// <summary>
        /// Creates a new file read task.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <param name="callback">Called with a stream providing the file content.</param>
        public ReadFile(string path, Action<Stream> callback)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            _stream = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            State = TaskState.Header;
            UnitsTotal = new FileInfo(Path).Length;

            State = TaskState.Data;
            using var stream = new ProgressStream(
                File.OpenRead(Path),
                new SynchronousProgress<long>(bytes => UnitsProcessed = bytes),
                CancellationToken);
            _stream(stream);
        }
    }
}
