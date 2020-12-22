// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Allows you to specify a <see cref="XmlQualifiedName"/> (namespace short-name) for <see cref="XmlStorage"/> to use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Values set in constructor are available via QualifiedName")]
    public sealed class XmlNamespaceAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="XmlQualifiedName"/>.
        /// </summary>
        public XmlQualifiedName QualifiedName { get; }

        /// <summary>
        /// Specified a <see cref="XmlQualifiedName"/> (namespace short-name) for <see cref="XmlStorage"/> to use.
        /// </summary>
        /// <param name="name">The short-name.</param>
        /// <param name="ns">The full namespace URI.</param>
        public XmlNamespaceAttribute(string name, string ns)
        {
            QualifiedName = new(name, ns);
        }
    }
}
