/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using NanoByte.Common.Tasks;

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
        /// <param name="throwOnEnd">Controls whether the method throws an exception when the end of the stream is reached instead of returning <c>null</c>.</param>
        /// <returns>The bytes read from the stream; may be <c>null</c> if <paramref name="throwOnEnd"/> is <c>false</c>.</returns>
        /// <exception cref="IOException">The desired number of bytes could not be read from the stream and <paramref name="throwOnEnd"/> is <c>true</c>.</exception>
        [ContractAnnotation("throwOnEnd:true => notnull; throwOnEnd:false => canbenull")]
        public static byte[] Read([NotNull] this Stream stream, int count, bool throwOnEnd = true)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException("stream");
            #endregion

            var buffer = new byte[count];
            int offset = 0;

            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0) break;
                offset += read;
            }

            if (offset == count) return buffer;
            else
            {
                if (throwOnEnd) throw new IOException("The desired number of bytes could not be read from the stream.");
                else return null;
            }
        }

        /// <summary>
        /// Reads the entire content of a stream to an array. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>A entire content of the stream.</returns>
        [NotNull]
        public static byte[] ToArray([NotNull] this Stream stream)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException("stream");
            #endregion

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyToEx(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Reads the entire content of a stream as string data. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding of the string; leave <c>null</c> to default to <see cref="UTF8Encoding"/>.</param>
        /// <returns>A entire content of the stream.</returns>
        [NotNull]
        public static string ReadToString([NotNull] this Stream stream, [CanBeNull] Encoding encoding = null)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException("stream");
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
        public static void CopyToEx([NotNull] this Stream source, [NotNull] Stream destination, int bufferSize = 4096, CancellationToken cancellationToken = default(CancellationToken), Tasks.IProgress<long> progress = null)
        {
            #region Sanity checks
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
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
                if (progress != null) progress.Report(sum);
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
        public static void CopyToFile([NotNull] this Stream stream, [NotNull, Localizable(false)] string path, int bufferSize = 4096, CancellationToken cancellationToken = default(CancellationToken), Tasks.IProgress<long> progress = null)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException("stream");
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            #endregion

            using (var fileStream = File.Create(path))
                stream.CopyToEx(fileStream, bufferSize, cancellationToken, progress);
        }

        /// <summary>
        /// Writes the entire contents of an array to a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="data">The array containing the bytes to write.</param>
        public static void Write([NotNull] this Stream stream, [NotNull] byte[] data)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException("stream");
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Compares two streams for bit-wise equality. Seeks to the beginnings of the streams if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <remarks>Will try to <see cref="Stream.Seek"/> to the start of both streams.</remarks>
        public static bool ContentEquals([NotNull] this Stream stream1, [NotNull] Stream stream2)
        {
            #region Sanity checks
            if (stream1 == null) throw new ArgumentNullException("stream1");
            if (stream2 == null) throw new ArgumentNullException("stream2");
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
        [NotNull]
        public static MemoryStream ToStream([NotNull] this string data, [CanBeNull] Encoding encoding = null)
        {
            #region Sanity checks
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            byte[] byteArray = (encoding ?? new UTF8Encoding(false)).GetBytes(data);
            var stream = new MemoryStream(byteArray);
            return stream;
        }

        /// <summary>
        /// Returns an embedded resource as a stream.
        /// </summary>
        /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        [NotNull]
        public static Stream GetEmbeddedStream([NotNull] this Type type, [NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            var assembly = Assembly.GetAssembly(type);
            var stream = assembly.GetManifestResourceStream(type, name);
            if (stream == null) throw new ArgumentException(string.Format("Embedded resource '{0}' not found.", name), "name");
            return stream;
        }

        /// <summary>
        /// Returns an embedded resource as a byte array.
        /// </summary>
        /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        [NotNull]
        public static byte[] GetEmbeddedBytes([NotNull] this Type type, [NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
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
        [NotNull]
        public static string GetEmbeddedString([NotNull] this Type type, [NotNull, Localizable(false)] string name, [CanBeNull] Encoding encoding = null)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            using (var stream = type.GetEmbeddedStream(name))
                return stream.ReadToString(encoding);
        }

        /// <summary>
        /// Copies an embedded resource to a file.
        /// </summary>
        /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <param name="path">The path of the file to write.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        public static void CopyEmbeddedToFile([NotNull] this Type type, [NotNull, Localizable(false)] string name, [NotNull, Localizable(false)] string path)
        {
            #region Sanity checks
            if (type == null) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            using (var stream = type.GetEmbeddedStream( name))
                stream.CopyToFile(path);
        }
    }
}
