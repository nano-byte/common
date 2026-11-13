// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Reflection;
using System.Text;

namespace NanoByte.Common.Streams;

/// <summary>
/// Provides <see cref="Stream"/>-related helper methods.
/// </summary>
public static class StreamUtils
{
    /// <param name="stream">The stream to read from.</param>
    extension(Stream stream)
    {
        /// <summary>
        /// Reads a fixed number of bytes from a stream starting from the current offset.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read from the stream.</returns>
        /// <exception cref="IOException">The desired number of bytes could not be read from the stream.</exception>
        public byte[] Read(int count)
            => stream.TryRead(count) ?? throw new IOException(string.Format(Resources.UnableToReadBytesFromStream, count));

        /// <summary>
        /// Reads a sequence of bytes from the stream.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes into.</param>
        /// <returns>The number of bytes read from the stream. May be less than the size of the <paramref name="buffer"/>.</returns>
        public int Read(ArraySegment<byte> buffer)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            if (buffer.Array == null) return 0;
            return stream.Read(buffer.Array!, buffer.Offset, buffer.Count);
        }

        /// <summary>
        /// Reads a fixed number of bytes from a stream starting from the current offset.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read from the stream; <c>null</c> if the desired number of bytes could not be read from the stream.</returns>
        public byte[]? TryRead(int count)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            byte[] buffer = new byte[count];
            int offset = 0;

            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0) return null;
                offset += read;
            }

            return buffer;
        }

        /// <summary>
        /// Reads the entire content of a stream. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <returns>The entire content of the stream.</returns>
        public ArraySegment<byte> ReadAll()
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            if (stream.CanSeek) stream.Position = 0;

            long GetLength()
            {
                try
                {
                    return stream.Length > 0 ? stream.Length : 64;
                }
                catch (Exception)
                {
                    return 64;
                }
            }

            byte[] buffer = new byte[GetLength()];
            int count = 0;
            while (true)
            {
                if (buffer.Length == count)
                {
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, count);
                    buffer = newBuffer;
                }

                int read = stream.Read(buffer, count, buffer.Length - count);
                count += read;
                if (read == 0) return new ArraySegment<byte>(buffer, 0, count);
            }
        }

        /// <summary>
        /// Skips a number of bytes in the stream.
        /// Uses <see cref="Stream.Seek"/> if supported, <see cref="Stream.Read(byte[],int,int)"/> otherwise.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        /// <exception cref="IOException">The desired number of bytes could not be skipped in the stream.</exception>
        public void Skip(int count)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Must not be negative.");
            #endregion

            if (stream.CanSeek)
            {
                stream.Seek(count, SeekOrigin.Current);
                return;
            }

            var pool = System.Buffers.ArrayPool<byte>.Shared;
            byte[] buffer = pool.Rent(Math.Min(count, 64));
            try
            {
                int toRead = count;
                while (toRead > 0)
                {
                    int read = stream.Read(buffer, 0, Math.Min(toRead, buffer.Length));
                    if (read == 0) throw new IOException(string.Format(Resources.UnableToReadBytesFromStream, count));
                    toRead -= read;
                }
            }
            finally
            {
                pool.Return(buffer);
            }
        }

        /// <summary>
        /// Writes the entire contents of an array to a stream.
        /// </summary>
        /// <param name="data">The array containing the bytes to write.</param>
        public void Write(params byte[] data)
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
        /// Writes the entire contents of a buffer to a stream.
        /// </summary>
        /// <param name="buffer">The buffer containing the bytes to write.</param>
        public void Write(ArraySegment<byte> buffer)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            if (buffer.Array == null) return;
            stream.Write(buffer.Array, buffer.Offset, buffer.Count);
        }

        /// <summary>
        /// The entire content of a stream as an array. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>. Avoids copying the underlying array if possible.
        /// </summary>
        public byte[] AsArray()
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

#if !NET45
            if (stream is MemoryStream memory && memory.TryGetBuffer(out var segment))
                return segment.AsArray();
#endif

            return stream.ReadAll().AsArray();
        }

        /// <summary>
        /// Copies the entire content of a stream to a <see cref="MemoryStream"/>. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <returns>A new stream. Shares the underlying array with <paramref name="stream"/> if possible.</returns>
        public MemoryStream ToMemory()
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            var read = stream.ReadAll();
            return new MemoryStream(read.Array!, read.Offset, read.Count, true, true);
        }

        /// <summary>
        /// Copies the content of one stream to another. Seeks to the beginning of the <paramref name="stream"/> stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="destination">The destination stream to copy to.</param>
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        public void CopyToEx(Stream destination)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            #endregion

            if (stream.CanSeek) stream.Position = 0;

            stream.CopyTo(destination);
        }

        /// <summary>
        /// Writes the entire content of a stream to a file. Seeks to the beginning of the <paramref name="stream"/> if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="path">The path of the file to write.</param>
        public void CopyToFile([Localizable(false)] string path)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            using var fileStream = File.Create(path);
            stream.CopyToEx(fileStream);
        }

        /// <summary>
        /// Adds seek buffering to a stream unless it already <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="bufferSize">The maximum number of bytes to buffer for seeking backwards. Set this to 0 to allow forward but no backward seeking.</param>
        [Pure]
        public Stream WithSeekBuffer(int bufferSize = SeekBufferStream.DefaultBufferSize)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            return stream.CanSeek ? stream : new SeekBufferStream(stream, bufferSize);
        }

        /// <summary>
        /// Overrides the value returned by <see cref="Stream.Length"/>.
        /// </summary>
        /// <param name="length">The value to return for <see cref="Stream.Length"/>.</param>
        [Pure]
        public Stream WithLength(long length)
        {
            var result = new ProgressStream(stream);
            result.SetLength(length);
            return result;
        }

        /// <summary>
        /// Reads the entire content of a stream as string data. Seeks to the beginning of the stream if <see cref="Stream.CanSeek"/>.
        /// </summary>
        /// <param name="encoding">The encoding of the string; leave <c>null</c> to default to <see cref="EncodingUtils.Utf8"/>.</param>
        /// <returns>A entire content of the stream.</returns>
        public string ReadToString(Encoding? encoding = null)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            if (stream.CanSeek) stream.Position = 0;
            var reader = new StreamReader(stream, encoding ?? EncodingUtils.Utf8);
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Creates a new <see cref="MemoryStream"/> and fills it with string data.
    /// </summary>
    /// <param name="data">The data to fill the stream with.</param>
    /// <param name="encoding">The encoding of the string; leave <c>null</c> to default to <see cref="EncodingUtils.Utf8"/>.</param>
    /// <returns>A filled stream with the position set to zero.</returns>
    [Pure]
    public static MemoryStream ToStream(this string data, Encoding? encoding = null)
        => new((encoding ?? EncodingUtils.Utf8).GetBytes(data ?? throw new ArgumentNullException(nameof(data))));

    /// <summary>
    /// Creates a new <see cref="MemoryStream"/> using the existing array as the underlying storage.
    /// </summary>
    /// <param name="array">The array to create the stream from.</param>
    /// <param name="writable">Controls whether the stream is writable (i.e., can modify the array).</param>
    [Pure]
    public static MemoryStream ToStream(this byte[] array, bool writable = false)
        => new(array ?? throw new ArgumentNullException(nameof(array)), writable);

    /// <summary>
    /// Creates a new <see cref="MemoryStream"/> using the existing array segment as the underlying storage.
    /// </summary>
    /// <param name="segment">The array segment to create the stream from.</param>
    /// <param name="writable">Controls whether the stream is writable (i.e., can modify the array).</param>
    [Pure]
    public static MemoryStream ToStream(this ArraySegment<byte> segment, bool writable = false)
        => new(
            segment.Array ?? throw new ArgumentNullException(nameof(segment)),
            segment.Offset,
            segment.Count,
            writable);

    /// <param name="type">A type that is located in the same namespace as the embedded resource.</param>
    extension(Type type)
    {
        /// <summary>
        /// Returns an embedded resource as a stream.
        /// </summary>
        /// <param name="name">The name of the embedded resource.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        [Pure]
        public Stream GetEmbeddedStream([Localizable(false)] string name)
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
        /// <param name="name">The name of the embedded resource.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        [Pure]
        public byte[] GetEmbeddedBytes([Localizable(false)] string name)
            => type.GetEmbeddedStream(name).AsArray();

        /// <summary>
        /// Returns an embedded resource as a string.
        /// </summary>
        /// <param name="name">The name of the embedded resource.</param>
        /// <param name="encoding">The encoding of the string; leave <c>null</c> to default to <see cref="EncodingUtils.Utf8"/>.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        [Pure]
        public string GetEmbeddedString([Localizable(false)] string name, Encoding? encoding = null)
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
        /// <param name="name">The name of the embedded resource.</param>
        /// <param name="path">The path of the file to write.</param>
        /// <exception cref="ArgumentException">The specified embedded resource does not exist.</exception>
        public void CopyEmbeddedToFile([Localizable(false)] string name, [Localizable(false)] string path)
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
