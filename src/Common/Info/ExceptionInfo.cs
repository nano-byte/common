// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Xml.Serialization;

namespace NanoByte.Common.Info
{
    /// <summary>
    /// Wraps information about an exception in a serializer-friendly format.
    /// </summary>
    [XmlType("exception")]
    public class ExceptionInfo
    {
        /// <summary>
        /// The type of exception.
        /// </summary>
        [XmlAttribute("type")]
        public string? ExceptionType { get; set; }

        /// <summary>
        /// The message describing the exception.
        /// </summary>
        [XmlElement("message")]
        public string? Message { get; set; }

        /// <summary>
        /// The name of the application or the object that causes the error.
        /// </summary>
        [XmlElement("source")]
        public string? Source { get; set; }

        /// <summary>
        /// A string representation of the frames on the call stack at the time the exception was thrown.
        /// </summary>
        [XmlElement("stack-trace")]
        public string? StackTrace { get; set; }

        /// <summary>
        /// Information about the exception that originally caused the exception being described here.
        /// </summary>
        [XmlElement("inner-exception")]
        public ExceptionInfo? InnerException { get; set; }

        #region Constructor
        /// <summary>
        /// Base-constructor for XML serialization. Do not call manually!
        /// </summary>
        public ExceptionInfo() {}

        /// <summary>
        /// Creates an exception information based on an exception.
        /// </summary>
        /// <param name="ex">The exception whose information to extract.</param>
        public ExceptionInfo(Exception ex)
            : this()
        {
            ExceptionType = (ex ?? throw new ArgumentNullException(nameof(ex))).GetType().ToString();
            Message = ex.Message;
            Source = ex.Source;
            StackTrace = ex.StackTrace;

            if (ex.InnerException != null)
                InnerException = new ExceptionInfo(ex.InnerException);
        }
        #endregion
    }
}
