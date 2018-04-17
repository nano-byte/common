// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using JetBrains.Annotations;
using NanoByte.Common.Values.Design;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Stores the file type describing the kind of data a property stores.
    /// Controls the behaviour of <see cref="CodeEditor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [PublicAPI]
    public sealed class FileTypeAttribute : Attribute
    {
        /// <summary>
        /// The name of the file type (e.g. XML, JavaScript, Lua).
        /// </summary>
        public string FileType { get; }

        /// <summary>
        /// Creates a new file type attribute.
        /// </summary>
        /// <param name="fileType">The name of the file type (e.g. XML, JavaScript, Lua).</param>
        public FileTypeAttribute([NotNull] string fileType)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(fileType)) throw new ArgumentNullException(nameof(fileType));
            #endregion

            FileType = fileType;
        }
    }
}
