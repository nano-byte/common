// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Xml.Serialization;
using NanoByte.Common.Info;
using NanoByte.Common.Storage;

#if !NET20 && !NET40
using System.Net.Http;
using System.Threading.Tasks;
#endif

namespace NanoByte.Common;

/// <summary>
/// Wraps information about an crash in a serializer-friendly format.
/// </summary>
// Note: Must be public, not internal, so XML Serialization will work
[XmlRoot("error-report"), XmlType("error-report")]
public class ErrorReport
{
    /// <summary>
    /// Information about the current application.
    /// </summary>
    [XmlElement("application")]
    public AppInfo Application { get; set; }

    /// <summary>
    /// Information about the current operating system.
    /// </summary>
    [XmlElement("os")]
    public OSInfo OS { get; set; }

    /// <summary>
    /// Information about the exception that occurred.
    /// </summary>
    [XmlElement("exception")]
    public ExceptionInfo? Exception { get; set; }

    /// <summary>
    /// The contents of the <see cref="Log"/> file.
    /// </summary>
    [XmlElement("log"), DefaultValue("")]
    public string? Log { get; set; }

    /// <summary>
    /// Comments about the crash entered by the user.
    /// </summary>
    [XmlElement("comments"), DefaultValue("")]
    public string? Comments { get; set; }

    /// <summary>
    /// Determines whether an exception should be reported.
    /// </summary>
    public static bool ShouldReport(Exception exception)
        => !IsExternalProblem(exception)
        && !IsExternalProblem(exception.InnerException);

    private static bool IsExternalProblem(Exception? ex)
        => ex is ExternalException or SEHException or OutOfMemoryException
        || (ex is FileNotFoundException && ex.Message.Contains("PublicKeyToken="));

    /// <summary>
    /// Generates an error report.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="comments">Comments about the crash entered by the user.</param>
    public static ErrorReport Generate(Exception exception, string comments) => new()
    {
        Application = AppInfo.Current,
        OS = OSInfo.Current,
        Exception = new ExceptionInfo(exception),
        Log = NanoByte.Common.Log.GetBuffer(),
        Comments = comments
    };

    /// <summary>
    /// Serializes the error report to an XML string, removing user-specific information.
    /// </summary>
    [RequiresDynamicCode("XML serialization requires runtime code generation.")]
    public string ToXmlStringAnonymized()
    {
        string serialized = this.ToXmlString();
        return Environment.UserName.Length > 1
            ? serialized.Replace(Environment.UserName, "[USERNAME]")
            : serialized;
    }

#if !NET20 && !NET40
    /// <summary>
    /// Sends the error report to a remote server.
    /// </summary>
    /// <param name="uri">The URI to send the report to.</param>
    /// <returns>The response from the server, if any. May contain instructions how to mitigate the error.</returns>
    [RequiresDynamicCode("XML serialization requires runtime code generation.")]
    public async Task<string> SendAsync(Uri uri)
    {
        // ReSharper disable once ShortLivedHttpClient
        using var httpClient = new HttpClient();
        using var content = new MultipartFormDataContent {{new StringContent(ToXmlStringAnonymized()), "file", "error-report.xml"}};
        using var response = await httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
#endif
}
