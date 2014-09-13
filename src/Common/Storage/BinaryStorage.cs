/*
 * Copyright 2006-2014 Bastian Eicher
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Provides easy serialization to binary files (optionally wrapped in ZIP archives).
    /// </summary>
    /// <remarks>This class serializes private fields.</remarks>
    public static class BinaryStorage
    {
        private static readonly BinaryFormatter _serializer = new BinaryFormatter();

        //--------------------//

        #region Load plain
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
            if (stream == null) throw new ArgumentNullException("stream");
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
        /// Loads an object from a binary filen
        /// </summary>
        /// <typeparam name="T">The type of object the binary stream shall be converted into.</typeparam>
        /// <param name="path">The binary file to be loaded.</param>
        /// <returns>The loaded object.</returns>
        /// <exception cref="IOException">A problem occurred while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
        /// <exception cref="InvalidDataException">A problem occurred while deserializing the binary data.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
        public static T LoadBinary<T>(string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            #endregion

            using (var fileStream = File.OpenRead(path))
                return LoadBinary<T>(fileStream);
        }
        #endregion

        #region Save plain
        /// <summary>
        /// Saves an object in a binary stream.
        /// </summary>
        /// <typeparam name="T">The type of object to be saved in a binary stream.</typeparam>
        /// <param name="data">The object to be stored.</param>
        /// <param name="stream">The binary file to be written.</param>
        public static void SaveBinary<T>(this T data, Stream stream)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException("stream");
            #endregion

            _serializer.Serialize(stream, data);
        }

        /// <summary>
        /// Saves an object in a binary file.
        /// </summary>
        /// <remarks>This method performs an atomic write operation when possible.</remarks>
        /// <typeparam name="T">The type of object to be saved in a binary stream.</typeparam>
        /// <param name="data">The object to be stored.</param>
        /// <param name="path">The binary file to be written.</param>
        /// <exception cref="IOException">A problem occurred while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the file is not permitted.</exception>
        public static void SaveBinary<T>(this T data, string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            #endregion

            using (var atomic = new AtomicWrite(path))
            using (var fileStream = File.Create(atomic.WritePath))
            {
                SaveBinary(data, fileStream);
                atomic.Commit();
            }
        }
        #endregion
    }
}
