// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using JetBrains.Annotations;

namespace NanoByte.Common.Storage.SlimDX
{
    /// <summary>
    /// Information about an additional file to be stored along side an ZIP archive using <see cref="Storage.XmlStorage"/> or <see cref="BinaryStorage"/>.
    /// </summary>
    public struct EmbeddedFile
    {
        #region Variables
        #endregion

        #region Properties
        /// <summary>
        /// The filename in the archive
        /// </summary>
        [NotNull]
        public string Filename { get; }

        /// <summary>
        /// The level of compression (0-9) to apply to this entry
        /// </summary>
        public int CompressionLevel { get; }

        /// <summary>
        /// The delegate to be called when the data is ready to be read/written to/form a stream
        /// </summary>
        [NotNull]
        public Action<Stream> StreamDelegate { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new XML ZIP entry for reading
        /// </summary>
        /// <param name="filename">The filename in the archive</param>
        /// <param name="readDelegate">The delegate to be called when the data is ready to be read</param>
        public EmbeddedFile([NotNull] string filename, [NotNull] Action<Stream> readDelegate)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            CompressionLevel = 0;
            StreamDelegate = readDelegate ?? throw new ArgumentNullException(nameof(readDelegate));
        }

        /// <summary>
        /// Creates a new XML ZIP entry for writing
        /// </summary>
        /// <param name="filename">The filename in the archive</param>
        /// <param name="compressionLevel">The level of compression (0-9) to apply to this entry</param>
        /// <param name="writeDelegate">The delegate to be called when the data is ready to be written</param>
        public EmbeddedFile([NotNull] string filename, int compressionLevel, [NotNull] Action<Stream> writeDelegate)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            CompressionLevel = compressionLevel;
            StreamDelegate = writeDelegate ?? throw new ArgumentNullException(nameof(writeDelegate));
        }
        #endregion
    }
}
