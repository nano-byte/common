#if !NET
// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Provides easy serialization to binary files (optionally wrapped in ZIP archives).
    /// </summary>
    public static class BinaryStorage
    {
        private static readonly BinaryFormatter _serializer = new();

        /// <summary>
        /// Loads an object from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object the binary stream shall be converted into.</typeparam>
        /// <param name="stream">The binary file to be loaded.</param>
        /// <returns>The loaded object.</returns>
        /// <exception cref="InvalidDataException">A problem occurred while deserializing the binary data.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
        public static T LoadBinary<T>(Stream stream)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            if (stream.CanSeek) stream.Position = 0;
            try
            {
                return (T)_serializer.Deserialize(stream);
            }
            #region Error handling
            catch (SerializationException ex)
            { // Convert exception type
                throw new InvalidDataException(ex.Message, ex.InnerException) {Source = ex.Source};
            }
            #endregion
        }

        /// <summary>
        /// Loads an object from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object the binary stream shall be converted into.</typeparam>
        /// <param name="path">The binary file to be loaded.</param>
        /// <returns>The loaded object.</returns>
        /// <exception cref="IOException">A problem occurred while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
        /// <exception cref="InvalidDataException">A problem occurred while deserializing the binary data.</exception>
        /// <remarks>Uses see cref="AtomicRead"/> internally.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
        public static T LoadBinary<T>([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            using (new AtomicRead(path))
            {
                using var fileStream = File.OpenRead(path);
                return LoadBinary<T>(fileStream);
            }
        }

        /// <summary>
        /// Saves an object in a binary stream.
        /// </summary>
        /// <typeparam name="T">The type of object to be saved in a binary stream.</typeparam>
        /// <param name="data">The object to be stored.</param>
        /// <param name="stream">The binary file to be written.</param>
        public static void SaveBinary<T>(this T data, Stream stream)
            where T : notnull
            => _serializer.Serialize(stream ?? throw new ArgumentNullException(nameof(stream)), data);

        /// <summary>
        /// Saves an object in a binary file.
        /// </summary>
        /// <remarks>This method performs an atomic write operation when possible.</remarks>
        /// <typeparam name="T">The type of object to be saved in a binary stream.</typeparam>
        /// <param name="data">The object to be stored.</param>
        /// <param name="path">The binary file to be written.</param>
        /// <exception cref="IOException">A problem occurred while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the file is not permitted.</exception>
        /// <remarks>Uses <seealso cref="AtomicWrite"/> internally.</remarks>
        public static void SaveBinary<T>(this T data, [Localizable(false)] string path)
            where T : notnull
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            using var atomic = new AtomicWrite(path);
            using var fileStream = File.Create(atomic.WritePath);
            SaveBinary(data, fileStream);
            atomic.Commit();
        }
    }
}
#endif
