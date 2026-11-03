// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Streams;

#if NET
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
#else
using Newtonsoft.Json;
#endif

namespace NanoByte.Common.Storage;

/// <summary>
/// Provides easy serialization to JSON files.
/// </summary>
/// <remarks>
/// On .NET, serialization uses System.Text.Json. On .NET Framework, it uses Newtonsoft.Json.
/// </remarks>
public static class JsonStorage
{
    private const string
        UnreferencedCodeMessage = "JSON serialization uses reflection, which is not compatible with trimming. Use an overload taking a JsonTypeInfo<T> instead.",
        DynamicCodeMessage = "JSON serialization uses reflection, which requires runtime code generation. Use an overload taking a JsonTypeInfo<T> instead.";

#if NET
    private static JsonSerializerOptions? _options;

    /// <summary>
    /// Options for reflection-based serialization that mimic the defaults of Newtonsoft.Json (used on .NET Framework).
    /// </summary>
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    private static JsonSerializerOptions GetOptions()
        => _options ??= new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(), // Enable reflection in trimmed apps
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            IncludeFields = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
            Converters = {new TypeConverterJsonConverterFactory()}
        };

    /// <summary>
    /// Reads JSON text from a stream, detecting the encoding from byte order marks.
    /// </summary>
    private static string ReadText(Stream stream)
    {
        if (stream.CanSeek) stream.Position = 0;
        using var reader = new StreamReader(stream, EncodingUtils.Utf8, detectEncodingFromByteOrderMarks: true, bufferSize: -1, leaveOpen: true);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Deserializes JSON text, converting exceptions and rejecting <c>null</c>.
    /// </summary>
    private static T Deserialize<T>(Func<T?> deserialize)
    {
        try
        {
            return deserialize() ?? throw new InvalidDataException("JSON deserialized to null");
        }
        #region Error handling
        catch (JsonException ex)
        {
            // Convert exception type
            throw new InvalidDataException(ex.Message, ex.InnerException) {Source = ex.Source};
        }
        #endregion
    }
#endif

    /// <summary>
    /// Loads an object from an JSON file.
    /// </summary>
    /// <typeparam name="T">The type of object the JSON stream shall be converted into.</typeparam>
    /// <param name="stream">The stream to read the encoded JSON data from.</param>
    /// <returns>The loaded object.</returns>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static T LoadJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Stream stream)
    {
        #region Sanity checks
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        #endregion

#if NET
        string json = ReadText(stream);
        return Deserialize(() => JsonSerializer.Deserialize<T>(json, GetOptions()));
#else
        if (stream.CanSeek) stream.Position = 0;

        using var reader = new JsonTextReader(new StreamReader(stream, EncodingUtils.Utf8, detectEncodingFromByteOrderMarks: true)) {CloseInput = false};
        try
        {
            return new JsonSerializer().Deserialize<T>(reader)
                ?? throw new InvalidDataException("JSON deserialized to null");
        }
        #region Error handling
        catch (JsonException ex)
        {
            // Convert exception type
            throw new InvalidDataException(ex.Message, ex.InnerException) {Source = ex.Source};
        }
        #endregion
#endif
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
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static T LoadJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([Localizable(false)] string path)
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
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static T FromJsonString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([Localizable(false)] string data)
    {
        #region Sanity checks
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

#if NET
        return Deserialize(() => JsonSerializer.Deserialize<T>(data, GetOptions()));
#else
        // Copy string to a stream and then parse
        using var stream = data.ToStream();
        return LoadJson<T>(stream);
#endif
    }

    /// <summary>
    /// Loads an object from an JSON string using an anonymous type as the target.
    /// </summary>
    /// <typeparam name="T">The type of object the JSON string shall be converted into.</typeparam>
    /// <param name="data">The JSON string to be parsed.</param>
    /// <param name="anonymousType">An instance of the anonymous type to parse to.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static T FromJsonString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string data, T anonymousType)
    {
        #region Sanity checks
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (anonymousType == null) throw new ArgumentNullException(nameof(anonymousType));
        #endregion

#if NET
        return Deserialize(() => JsonSerializer.Deserialize<T>(data, GetOptions()));
#else
        try
        {
            return JsonConvert.DeserializeAnonymousType(data, anonymousType)
                ?? throw new InvalidDataException("JSON deserialized to null");
        }
        #region Error handling
        catch (JsonException ex)
        {
            // Convert exception type
            throw new InvalidDataException(ex.Message, ex.InnerException) {Source = ex.Source};
        }
        #endregion
#endif
    }

    /// <summary>
    /// Saves an object in an JSON stream.
    /// </summary>
    /// <typeparam name="T">The type of object to be saved in an JSON stream.</typeparam>
    /// <param name="data">The object to be stored.</param>
    /// <param name="stream">The stream to write the encoded JSON data to.</param>
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static void SaveJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this T data, Stream stream)
    {
        #region Sanity checks
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        #endregion

#if NET
        // Serialize the runtime type (like Newtonsoft.Json) rather than just the declared type
        JsonSerializer.Serialize(stream, data, data?.GetType() ?? typeof(T), GetOptions());
#else
        var writer = new StreamWriter(stream, EncodingUtils.Utf8);
        new JsonSerializer().Serialize(writer, data);
        writer.Flush();
#endif
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
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static void SaveJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this T data, [Localizable(false)] string path)
        where T : notnull
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        #endregion

        using var atomic = new AtomicWrite(path);
        using (var fileStream = File.Create(atomic.WritePath))
            data.SaveJson(fileStream);
        atomic.Commit();
    }

    /// <summary>
    /// Returns an object as an JSON string.
    /// </summary>
    /// <typeparam name="T">The type of object to be saved in an JSON string.</typeparam>
    /// <param name="data">The object to be stored.</param>
    /// <returns>A string containing the JSON code.</returns>
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static string ToJsonString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this T data)
        where T : notnull
    {
        using var stream = new MemoryStream();
        data.SaveJson(stream);
        return stream.ReadToString();
    }

    /// <summary>
    /// Reparses an object previously deserialized from JSON into a different representation.
    /// </summary>
    /// <typeparam name="T">The type of object the data shall be converted into.</typeparam>
    /// <param name="data">The object to be parsed again.</param>
    /// <returns>The deserialized object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static T ReparseAsJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this object data)
        => FromJsonString<T>(data.ToJsonString());

    /// <summary>
    /// Reparses an object previously deserialized from JSON into a different representation using an anonymous type as the target.
    /// </summary>
    /// <typeparam name="T">The type of object the data shall be converted into.</typeparam>
    /// <param name="data">The object to be parsed again.</param>
    /// <param name="anonymousType">An instance of the anonymous type to parse to.</param>
    /// <returns>The deserialized object.</returns>
    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [RequiresDynamicCode(DynamicCodeMessage)]
    public static T ReparseAsJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this object data, T anonymousType)
        => FromJsonString(data.ToJsonString(), anonymousType);

#if NET
    /// <summary>
    /// Loads an object from a JSON stream using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of object the JSON stream shall be converted into.</typeparam>
    /// <param name="stream">The stream to read the encoded JSON data from.</param>
    /// <param name="typeInfo">Metadata about the type to convert, usually generated by a <see cref="JsonSerializerContext"/>.</param>
    /// <returns>The loaded object.</returns>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
    public static T LoadJson<T>(Stream stream, JsonTypeInfo<T> typeInfo)
    {
        #region Sanity checks
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
        #endregion

        string json = ReadText(stream);
        return Deserialize(() => JsonSerializer.Deserialize(json, typeInfo));
    }

    /// <summary>
    /// Loads an object from a JSON file using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of object the JSON file shall be converted into.</typeparam>
    /// <param name="path">The path of the file to load.</param>
    /// <param name="typeInfo">Metadata about the type to convert, usually generated by a <see cref="JsonSerializerContext"/>.</param>
    /// <returns>The loaded object.</returns>
    /// <exception cref="IOException">A problem occurred while reading the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
    /// <remarks>Uses <see cref="AtomicRead"/> internally.</remarks>
    public static T LoadJson<T>([Localizable(false)] string path, JsonTypeInfo<T> typeInfo)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
        #endregion

        try
        {
            using (new AtomicRead(path))
            {
                using var fileStream = File.OpenRead(path);
                return LoadJson(fileStream, typeInfo);
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
    /// Loads an object from a JSON string using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of object the JSON string shall be converted into.</typeparam>
    /// <param name="data">The JSON string to be parsed.</param>
    /// <param name="typeInfo">Metadata about the type to convert, usually generated by a <see cref="JsonSerializerContext"/>.</param>
    /// <returns>The loaded object.</returns>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the JSON data.</exception>
    public static T FromJsonString<T>([Localizable(false)] string data, JsonTypeInfo<T> typeInfo)
    {
        #region Sanity checks
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
        #endregion

        return Deserialize(() => JsonSerializer.Deserialize(data, typeInfo));
    }

    /// <summary>
    /// Saves an object in a JSON stream using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of object to be saved in a JSON stream.</typeparam>
    /// <param name="data">The object to be stored.</param>
    /// <param name="stream">The stream to write the encoded JSON data to.</param>
    /// <param name="typeInfo">Metadata about the type to convert, usually generated by a <see cref="JsonSerializerContext"/>.</param>
    public static void SaveJson<T>(this T data, Stream stream, JsonTypeInfo<T> typeInfo)
    {
        #region Sanity checks
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
        #endregion

        JsonSerializer.Serialize(stream, data, typeInfo);
    }

    /// <summary>
    /// Saves an object in a JSON file using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of object to be saved in a JSON file.</typeparam>
    /// <param name="data">The object to be stored.</param>
    /// <param name="path">The path of the file to write.</param>
    /// <param name="typeInfo">Metadata about the type to convert, usually generated by a <see cref="JsonSerializerContext"/>.</param>
    /// <exception cref="IOException">A problem occurred while writing the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Write access to the file is not permitted.</exception>
    /// <remarks>Uses <see cref="AtomicWrite"/> internally.</remarks>
    public static void SaveJson<T>(this T data, [Localizable(false)] string path, JsonTypeInfo<T> typeInfo)
        where T : notnull
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
        #endregion

        using var atomic = new AtomicWrite(path);
        using (var fileStream = File.Create(atomic.WritePath))
            data.SaveJson(fileStream, typeInfo);
        atomic.Commit();
    }

    /// <summary>
    /// Returns an object as a JSON string using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of object to be saved in a JSON string.</typeparam>
    /// <param name="data">The object to be stored.</param>
    /// <param name="typeInfo">Metadata about the type to convert, usually generated by a <see cref="JsonSerializerContext"/>.</param>
    /// <returns>A string containing the JSON code.</returns>
    public static string ToJsonString<T>(this T data, JsonTypeInfo<T> typeInfo)
        where T : notnull
        => JsonSerializer.Serialize(data, typeInfo ?? throw new ArgumentNullException(nameof(typeInfo)));
#endif
}
