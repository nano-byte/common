// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Streams;
using Newtonsoft.Json;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Provides easy serialization to JSON files.
    /// </summary>
    public static class JsonStorage
    {
        /// <summary>
        /// Loads an object from an JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object the JSON stream shall be converted into.</typeparam>
        /// <param name="stream">The stream to read the encoded JSON data from.</param>
        /// <returns>The loaded object.</returns>
        /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
        public static T LoadJson<T>(Stream stream)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            if (stream.CanSeek) stream.Position = 0;

            using var reader = new JsonTextReader(new StreamReader(stream, EncodingUtils.Utf8, detectEncodingFromByteOrderMarks: true)) {CloseInput = false};
            try
            {
                return new JsonSerializer().Deserialize<T>(reader)
                    ?? throw new InvalidDataException("JSON deserialized to null");
            }
            #region Error handling
            catch (JsonReaderException ex)
            {
                // Convert exception type
                throw new InvalidDataException(ex.Message, ex.InnerException) {Source = ex.Source};
            }
            #endregion
        }

        /// <summary>
        /// Loads an object from an JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object the JSON stream shall be converted into.</typeparam>
        /// <param name="path">The path of the file to load.</param>
        /// <returns>The loaded object.</returns>
        /// <exception cref="IOException">A problem occurred while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
        /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
        /// <remarks>Uses <see cref="AtomicRead"/> internally.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
        public static T LoadJson<T>([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            try
            {
                using (new AtomicRead(path))
                {
                    using var fileStream = File.OpenRead(path);
                    return LoadJson<T>(fileStream);
                }
            }
            #region Error handling
            catch (InvalidDataException ex)
            {
                // Change exception message to add context information
                throw new InvalidDataException(string.Format(Resources.ProblemLoading, path) + Environment.NewLine + ex.Message, ex.InnerException);
            }
            #endregion
        }

        /// <summary>
        /// Loads an object from an JSON string.
        /// </summary>
        /// <typeparam name="T">The type of object the JSON string shall be converted into.</typeparam>
        /// <param name="data">The JSON string to be parsed.</param>
        /// <returns>The loaded object.</returns>
        /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
        public static T FromJsonString<T>([Localizable(false)] string data)
        {
            #region Sanity checks
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            // Copy string to a stream and then parse
            using var stream = data.ToStream();
            return LoadJson<T>(stream);
        }

        /// <summary>
        /// Loads an object from an JSON string using an anonymous type as the target.
        /// </summary>
        /// <typeparam name="T">The type of object the JSON string shall be converted into.</typeparam>
        /// <param name="data">The JSON string to be parsed.</param>
        /// <param name="anonymousType">An instance of the anonymous type to parse to.</param>
        /// <returns>The deserialized object.</returns>
        public static T FromJsonString<T>(string data, T anonymousType)
            => JsonConvert.DeserializeAnonymousType(
                   data ?? throw new ArgumentNullException(nameof(data)),
                   anonymousType ?? throw new ArgumentNullException(nameof(anonymousType)))
            ?? throw new InvalidDataException("JSON deserialized to null");

        /// <summary>
        /// Saves an object in an JSON stream.
        /// </summary>
        /// <typeparam name="T">The type of object to be saved in an JSON stream.</typeparam>
        /// <param name="data">The object to be stored.</param>
        /// <param name="stream">The stream to write the encoded JSON data to.</param>
        public static void SaveJson<T>(this T data, Stream stream)
        {
            #region Sanity checks
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            #endregion

            var writer = new StreamWriter(stream, EncodingUtils.Utf8);
            new JsonSerializer().Serialize(writer, data);
            writer.Flush();
        }

        /// <summary>
        /// Saves an object in an JSON file.
        /// </summary>
        /// <remarks>This method performs an atomic write operation when possible.</remarks>
        /// <typeparam name="T">The type of object to be saved in an JSON stream.</typeparam>
        /// <param name="data">The object to be stored.</param>
        /// <param name="path">The path of the file to write.</param>
        /// <exception cref="IOException">A problem occurred while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the file is not permitted.</exception>
        /// <remarks>Uses <see cref="AtomicWrite"/> internally.</remarks>
        public static void SaveJson<T>(this T data, [Localizable(false)] string path)
            where T : notnull
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            using var atomic = new AtomicWrite(path);
            using (var fileStream = File.Create(atomic.WritePath))
                SaveJson(data, fileStream);
            atomic.Commit();
        }

        /// <summary>
        /// Returns an object as an JSON string.
        /// </summary>
        /// <typeparam name="T">The type of object to be saved in an JSON string.</typeparam>
        /// <param name="data">The object to be stored.</param>
        /// <returns>A string containing the JSON code.</returns>
        public static string ToJsonString<T>(this T data)
            where T : notnull
        {
            using var stream = new MemoryStream();
            SaveJson(data, stream);
            return stream.ReadToString();
        }

        /// <summary>
        /// Reparses an object previously deserialized from JSON into a different representation.
        /// </summary>
        /// <typeparam name="T">The type of object the data shall be converted into.</typeparam>
        /// <param name="data">The object to be parsed again.</param>
        /// <returns>The deserialized object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
        public static T ReparseAsJson<T>(this object data)
            => FromJsonString<T>(data.ToJsonString());

        /// <summary>
        /// Reparses an object previously deserialized from JSON into a different representation using an anonymous type as the target.
        /// </summary>
        /// <typeparam name="T">The type of object the data shall be converted into.</typeparam>
        /// <param name="data">The object to be parsed again.</param>
        /// <param name="anonymousType">An instance of the anonymous type to parse to.</param>
        /// <returns>The deserialized object.</returns>
        public static T ReparseAsJson<T>(this object data, T anonymousType)
            => FromJsonString(data.ToJsonString(), anonymousType);
    }
}
