// Copyright Bastian Eicher
// Licensed under the MIT License

using ICSharpCode.SharpZipLib.Zip;

namespace NanoByte.Common.Storage.SlimDX
{
    /// <summary>
    /// Represents a file in a content archive.
    /// </summary>
    internal struct ContentArchiveEntry
    {
        /// <summary>
        /// The archive containing the file.
        /// </summary>
        public ZipFile ZipFile { get; }

        /// <summary>
        /// The actual content file.
        /// </summary>
        public ZipEntry ZipEntry { get; }

        /// <summary>
        /// Creates a new content file representation
        /// </summary>
        /// <param name="zipFile">The archive containing the file</param>
        /// <param name="zipEntry">The actual content file</param>
        public ContentArchiveEntry(ZipFile zipFile, ZipEntry zipEntry)
        {
            ZipFile = zipFile;
            ZipEntry = zipEntry;
        }
    }
}
