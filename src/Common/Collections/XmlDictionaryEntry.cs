// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// A key-value string pair for <see cref="XmlDictionary"/>.
    /// </summary>
    [Serializable]
    public sealed class XmlDictionaryEntry : IEquatable<XmlDictionaryEntry>, ICloneable<XmlDictionaryEntry>
    {
        /// <summary>
        /// The collection that owns this entry - set to enable automatic duplicate detection!
        /// </summary>
        internal XmlDictionary? Parent;

        private string? _key;

        /// <summary>
        /// The unique text key. Warning: If this is changed the <see cref="XmlDictionary"/> must be rebuilt in order to update its internal hash table.
        /// </summary>
        /// <exception cref="InvalidOperationException">The new key value already exists in the <see cref="Parent"/> dictionary.</exception>
        [XmlAttribute("key")]
        public string Key
        {
            get => _key ?? "";
            set
            {
                if (Parent != null && Parent.ContainsKey(value))
                    throw new InvalidOperationException(Resources.KeyAlreadyPresent);
                _key = value;
            }
        }

        /// <summary>
        /// The text value.
        /// </summary>
        [XmlText]
        public string Value { get; set; } = "";

        /// <summary>
        /// Base-constructor for XML serialization. Do not call manually!
        /// </summary>
        public XmlDictionaryEntry() {}

        /// <summary>
        /// Creates a new entry for <see cref="XmlDictionary"/>.
        /// </summary>
        /// <param name="key">The unique text key.</param>
        /// <param name="value">The text value.</param>
        public XmlDictionaryEntry(string key, string value)
        {
            _key = key;
            Value = value;
        }

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => Key + ": " + Value;
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(XmlDictionaryEntry? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Value == Value && other.Key == Key;
        }

        public static bool operator ==(XmlDictionaryEntry left, XmlDictionaryEntry right) => Equals(left, right);
        public static bool operator !=(XmlDictionaryEntry left, XmlDictionaryEntry right) => !Equals(left, right);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(XmlDictionaryEntry)) return false;
            return Equals((XmlDictionaryEntry)obj);
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
            => HashCode.Combine(Value, _key);
        #endregion

        #region Clone
        /// <summary>
        /// Creates a plain copy of this entry.
        /// </summary>
        /// <returns>The cloned entry.</returns>
        public XmlDictionaryEntry Clone() => new(Key, Value);
        #endregion
    }
}
