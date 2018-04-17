// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;
using System.Xml.Serialization;
using NanoByte.Common.Info;

namespace NanoByte.Common.Controls
{
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
        public ExceptionInfo Exception { get; set; }

        /// <summary>
        /// The contents of the <see cref="Log"/> file.
        /// </summary>
        [XmlElement("log"), DefaultValue("")]
        public string Log { get; set; }

        /// <summary>
        /// Comments about the crash entered by the user.
        /// </summary>
        [XmlElement("comments"), DefaultValue("")]
        public string Comments { get; set; }
    }
}
