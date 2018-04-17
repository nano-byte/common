// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// An equatable element that can be merged using 3-way merging.
    /// </summary>
    /// <typeparam name="T">The type the interface is being applied to.</typeparam>
    public interface IMergeable<T> : IEquatable<T>
    {
        /// <summary>
        /// A unique identifier used when comparing for merging. Should always remain the same, even when the element is modified.
        /// </summary>
        [DefaultValue(0)]
        [Localizable(false)]
        string MergeID { get; }

        /// <summary>
        /// The time this element was last modified. This is used to determine preceedence with sync conflicts.
        /// </summary>
        /// <remarks>This value is ignored by clone and equality methods.</remarks>
        [DefaultValue(0)]
        DateTime Timestamp { get; set; }
    }
}
