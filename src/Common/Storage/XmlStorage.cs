// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Storage;

/// <summary>
/// Provides easy serialization to XML files.
/// </summary>
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "[DynamicallyAccessedMembers] captures top-level types, library users are expected to manually root transient dependencies.")]
public static class XmlStorage
{
    /// <summary>
    /// The XML namespace used for XML Schema instance.
    /// </summary>
    public const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

    /// <summary>
    /// Loads an object from an XML file.
    /// </summary>
    /// <typeparam name="T">The type of object the XML stream shall be converted into.</typeparam>
    /// <param name="stream">The stream to read the encoded XML data from.</param>
    /// <returns>The loaded object.</returns>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the XML data.</exception>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
    public static T LoadXml<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Stream stream)
    {
        #region Sanity checks
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        #endregion

        if (stream.CanSeek) stream.Position = 0;
        try
        {
            return (T)GetSerializer(typeof(T)).Deserialize(stream)!;
        }
        #region Error handling
        catch (InvalidOperationException ex)
        {
            // Convert exception type
            throw new InvalidDataException(ex.Message, ex.InnerException) {Source = ex.Source};
        }
        #endregion
    }

    /// <summary>
    /// Loads an object from an XML file.
    /// </summary>
    /// <typeparam name="T">The type of object the XML stream shall be converted into.</typeparam>
    /// <param name="path">The path of the file to load.</param>
    /// <returns>The loaded object.</returns>
    /// <exception cref="IOException">A problem occurred while reading the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the XML data.</exception>
    /// <remarks>Uses <see cref="AtomicRead"/> internally.</remarks>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
    public static T LoadXml<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([Localizable(false)] string path)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        #endregion

        try
        {
            using (new AtomicRead(path))
            {
                using var fileStream = File.OpenRead(path);
                return LoadXml<T>(fileStream);
            }
        }
        #region Error handling
        catch (InvalidDataException ex)
        {
            // Change exception message to add context information
            throw new InvalidDataException(string.Format(Resources.ProblemLoading, path) + Environment.NewLine + ex.GetMessageWithInner(), ex.InnerException);
        }
        #endregion
    }

    /// <summary>
    /// Loads an object from an XML string.
    /// </summary>
    /// <typeparam name="T">The type of object the XML string shall be converted into.</typeparam>
    /// <param name="data">The XML string to be parsed.</param>
    /// <returns>The loaded object.</returns>
    /// <exception cref="InvalidDataException">A problem occurred while deserializing the XML data.</exception>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to determine the type of returned object")]
    public static T FromXmlString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([Localizable(false)] string data)
    {
        #region Sanity checks
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        // Copy string to a stream and then parse
        using var stream = data.ToStream();
        return LoadXml<T>(stream);
    }

    /// <summary>
    /// Saves an object in an XML stream ending with a line break.
    /// </summary>
    /// <param name="data">The object to be stored.</param>
    /// <param name="stream">The stream to write the encoded XML data to.</param>
    /// <param name="stylesheet">The path of an XSL stylesheet; can be <c>null</c>.</param>
    public static void SaveXml<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this T data, Stream stream, [Localizable(false)] string? stylesheet = null) where T : notnull
    {
        #region Sanity checks
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        #endregion

        var type = data.GetType();

        var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Encoding = EncodingUtils.Utf8,
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\n"
        });

        // Add stylesheet processor instruction
        if (!string.IsNullOrEmpty(stylesheet))
            xmlWriter.WriteProcessingInstruction("xml-stylesheet", $"""
                type="text/xsl" href="{stylesheet}"
                """);

        var serializer = GetSerializer(type);
        var qualifiedNames = GetQualifiedNames(type);
        if (qualifiedNames.Length == 0) serializer.Serialize(xmlWriter, data);
        else serializer.Serialize(xmlWriter, data, new XmlSerializerNamespaces(qualifiedNames));

        // End file with line break
        xmlWriter.Flush();
        if (xmlWriter.Settings != null)
        {
            byte[] newLine = xmlWriter.Settings.Encoding.GetBytes(xmlWriter.Settings.NewLineChars);
            stream.Write(newLine, 0, newLine.Length);
        }
    }

    private static XmlQualifiedName[] GetQualifiedNames(Type type)
    {
        var qualifiedNames = type.GetCustomAttributes<XmlNamespaceAttribute>().Select(attr => attr.QualifiedName);

        return type.GetCustomAttribute<XmlRootAttribute>() is {} rootAttribute
            ? [..qualifiedNames, new XmlQualifiedName("", rootAttribute.Namespace)]
            : qualifiedNames.ToArray();
    }

    /// <summary>
    /// Saves an object in an XML file ending with a line break.
    /// </summary>
    /// <remarks>This method performs an atomic write operation when possible.</remarks>
    /// <typeparam name="T">The type of object to be saved in an XML stream.</typeparam>
    /// <param name="data">The object to be stored.</param>
    /// <param name="path">The path of the file to write.</param>
    /// <param name="stylesheet">The path of an XSL stylesheet for <typeparamref name="T"/>; can be <c>null</c>.</param>
    /// <exception cref="IOException">A problem occurred while writing the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Write access to the file is not permitted.</exception>
    /// <remarks>Uses <see cref="AtomicWrite"/> internally.</remarks>
    public static void SaveXml<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this T data, [Localizable(false)] string path, [Localizable(false)] string? stylesheet = null)
        where T : notnull
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        #endregion

        using var atomic = new AtomicWrite(path);
        using (var fileStream = File.Create(atomic.WritePath))
            SaveXml(data, fileStream, stylesheet);
        atomic.Commit();
    }

    /// <summary>
    /// Returns an object as an XML string ending with a line break.
    /// </summary>
    /// <param name="data">The object to be stored.</param>
    /// <param name="stylesheet">The path of an XSL stylesheet; can be <c>null</c>.</param>
    /// <returns>A string containing the XML code.</returns>
    public static string ToXmlString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this T data, [Localizable(false)] string? stylesheet = null) where T : notnull
    {
        using var stream = new MemoryStream();
        SaveXml(data, stream, stylesheet);
        string result = stream.ReadToString();

        // Remove encoding="utf-8" because we don't know how the string will actually be encoded on-dik
        const string prefixWithEncoding = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        const string prefixWithoutEncoding = "<?xml version=\"1.0\"?>";
        return prefixWithoutEncoding + result[prefixWithEncoding.Length..];
    }

    private static XmlSerializer GetSerializer(Type type)
        => ExceptionUtils.Retry<COMException, XmlSerializer>(() => new XmlSerializer(type));
}
