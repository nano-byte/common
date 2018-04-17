// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Runtime.Serialization;

namespace NanoByte.Common
{
    /// <summary>
    /// Indicates that the requested operation requires a GUI.
    /// </summary>
    [Serializable]
    public class NeedGuiException : InvalidOperationException
    {
        /// <inheritdoc/>
        public NeedGuiException() {}

        /// <inheritdoc/>
        public NeedGuiException(string message, Exception inner)
            : base(message, inner)
        {}

        /// <inheritdoc/>
        public NeedGuiException(string message)
            : base(message)
        {}

        /// <inheritdoc/>
        protected NeedGuiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}
