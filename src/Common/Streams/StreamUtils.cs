// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Provides <see cref="Stream"/>-related helper methods.
    /// </summary>
    public static class StreamUtils
    {
        /// <summary>
        /// Reads a fixed number of bytes from a stream starting from the current offset.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read from the stream.</returns>
        /// <exception cref="IOException">The desired number of bytes could not be read from the stream.</exception>
        public static byte[] Read(this Stream stream, int count)
            => stream.TryRead(count) ?? throw new IOException("The desired number of bytes could not be read from the stream.");

        /// <summary>
        /// Reads a fixed number of bytes from a stream starting from the current offset.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read from the stream; <c>null</c> if the desired number of bytes could not be read from the stream .</returns>
        public static byte[]? TryRead(this Stream stream, int count)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            var buffer = new byte[count];
            int offset = 0;

            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0) break;
                offset += read;
            }

            return offset == count ? buffer : null;
        }

        /// <summary>
        /// Reads the entire content of a stream to an array. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>A entire content of the stream.</returns>
        public static byte[] ToArray(this Stream stream)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            using var memoryStream = new MemoryStream();
            stream.CopyToEx(memoryStream);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Reads the entire content of a stream as string data. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding of the string; leave <c>null</c> to default to <see cref="UTF8Encoding"/>.</param>
        /// <returns>A entire content of the stream.</returns>
        public static string ReadToString(this Stream stream, Encoding? encoding = null)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            if (stream.CanSeek) stream.Position = 0;
            var reader = new StreamReader(stream, encoding ?? new UTF8Encoding(false));
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Copies the content of one stream to another. Seeks to the beginning of the <paramref name="source"/> stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="source">The source stream to copy from.</param>
        /// <param name="destination">The destination stream to copy to.</param>
        /// <param name="bufferSize">The size of the buffer to use for copying in bytes.</param>
        /// <param name="cancellationToken">Used to signal when the user wishes to cancel the copy process.</param>
        /// <param name="progress">Used to report back the number of bytes that have been copied so far.</param>
        /// <remarks>Will try to <see cref="Stream.Seek"/> to the start of <paramref name="source"/>.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        public static void CopyToEx(this Stream source, Stream destination, int bufferSize = 81920, CancellationToken cancellationToken = default, IProgress<long>? progress = null)
        {
            #region Sanity checks
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            #endregion

            if (source.CanSeek) source.Position = 0;

            var buffer = new byte[bufferSize];
            int read;
            long sum = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                sum += read = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, read);
                progress?.Report(sum);
            } while (read != 0);
        }

        /// <summary>
        /// Writes the entire content of a stream to a file.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="path">The path of the file to write.</param>
        /// <param name="bufferSize">The size of the buffer to use for copying in bytes.</param>
        /// <param name="cancellationToken">Used to signal when the user wishes to cancel the copy process.</param>
        /// <param name="progress">Used to report back the number of bytes that have been copied so far. Callbacks are rate limited to once every 250ms.</param>
        public static void CopyToFile(this Stream stream, [Localizable(false)] string path, int bufferSize = 81920, CancellationToken cancellationToken = default, IProgress<long>? progress = null)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            using var fileStream = File.Create(path);
            stream.CopyToEx(fileStream, bufferSize, cancellationToken, progress);
        }

        /// <summary>
        /// Writes the entire contents of an array to a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="data">The array containing the bytes to write.</param>
        public static void Write(this Stream stream, byte[] data)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

#if NETFRAMEWORK
            stream.Write(data, 0, data.Length);
#else
            stream.Write(data);
#endif
        }

        /// <summary>
        /// Compares two streams for bit-wise equality. Seeks to the beginnings of the streams if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <remarks>Will try to <see cref="Stream.Seek"/> to the start of both streams.</remarks>
        public static bool ContentEquals(this Stream stream1, Stream stream2)
        {
            #region Sanity checks
            if (stream1 == null) throw new ArgumentNullException(nameof(stream1));
            if (stream2 == null) throw new ArgumentNullException(nameof(stream2));
            #endregion

            if (stream1.CanSeek) stream1.Position = 0;
            if (stream2.CanSeek) stream2.Position = 0;

            while (true)
            {
                int byte1 = stream1.ReadByte();
                int byte2 = stream2.ReadByte();
                if (byte1 != byte2) return false;
                else if (byte1 == -1) return true;
            }
        }

        /// <summary>
        /// Creates a new <see cref="MemoryStream"/> and fills it with string data.
        /// </summary>
        /// <param name="data">The data to fill the stream with.</param>
        /// <param name="encoding">The encoding of the string; leave <c>null</c> to default to <see cref="UTF8Encoding"/>.</param>
        /// <returns>A filled stream with the position set to zero.</returns>
        public static MemoryStream ToStream(this string data, Encoding? encoding = null)
        {
            #region Sanity checks
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            var byteArray = (encoding ?? new UTF8Encoding(false)).GetBytes(data);
            var stream = new MemoryStream(byteArray);
            return stream;
        }

        /// <summary>
        /// Returns an embedded resource as a stream.
        /// </summary>
        /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        public static Stream GetEmbeddedStream(this Type type, [Localizable(false)] string name)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            #endregion

            var stream = Assembly.GetAssembly(type)?.GetManifestResourceStream(type, name);
            if (stream == null) throw new ArgumentException($"Embedded resource '{name}' not found.", nameof(name));
            return stream;
        }

        /// <summary>
        /// Returns an embedded resource as a byte array.
        /// </summary>
        /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        public static byte[] GetEmbeddedBytes(this Type type, [Localizable(false)] string name)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            #endregion

            return type.GetEmbeddedStream(name).ToArray();
        }

        /// <summary>
        /// Returns an embedded resource as a string.
        /// </summary>
        /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <param name="encoding">The encoding of the string; leave <c>null</c> to default to <see cref="UTF8Encoding"/>.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        public static string GetEmbeddedString(this Type type, [Localizable(false)] string name, Encoding? encoding = null)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            #endregion

            using var stream = type.GetEmbeddedStream(name);
            return stream.ReadToString(encoding);
        }

        /// <summary>
        /// Copies an embedded resource to a file.
        /// </summary>
        /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <param name="path">The path of the file to write.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        public static void CopyEmbeddedToFile(this Type type, [Localizable(false)] string name, [Localizable(false)] string path)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            #endregion

            using var stream = type.GetEmbeddedStream(name);
            stream.CopyToFile(path);
        }
    }
}
