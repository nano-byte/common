/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using NanoByte.Common.Values;
using NanoByte.Common.Values.Design;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// A set of languages that can be serialized as a simple space-separated list of ISO language codes.
    /// </summary>
    /// <remarks>Uses Unix-style language codes with an underscore (_) separator.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A Set is a special case of a Collection.")]
    [Serializable]
    [TypeConverter(typeof(StringConstructorConverter<LanguageSet>))]
    public class LanguageSet : SortedSet<CultureInfo>
    {
        /// <summary>
        /// Creates a new empty language collection.
        /// </summary>
        public LanguageSet()
            : base(CultureComparer.Instance)
        {}

        /// <summary>
        /// Creates a new language collection pre-filled with a set of languages.
        /// </summary>
        /// <param name="collection"></param>
        public LanguageSet([NotNull] IEnumerable<CultureInfo> collection)
            : base(collection, CultureComparer.Instance)
        {}

        /// <summary>
        /// Deserializes a space-separated list of languages codes.
        /// </summary>
        /// <param name="value">A space-separated list of languages codes (in the same format as used by the LANG environment variable).</param>
        /// <remarks>Unknown language codes in <paramref name="value"/> are replaced with <see cref="CultureInfo.InvariantCulture"/> rather than raising an <see cref="ArgumentException"/>.</remarks>
        public LanguageSet(string value)
            : this(ParseString(value))
        {}

        protected LanguageSet(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}

        private static IEnumerable<CultureInfo> ParseString(string value)
        {
            if (string.IsNullOrEmpty(value)) yield break;

            // Replace list by parsing input string split by spaces
            foreach (string langCode in value.Split(' '))
            {
                CultureInfo language = null;
                try
                {
                    language = Languages.FromString(langCode);
                }
                catch (ArgumentException)
                {
                    Log.Warn("Unknown language code: " + language);
                    language = CultureInfo.InvariantCulture;
                }
                if (language != null) yield return language;
            }
        }

        /// <summary>
        /// Adds a language identified by a string to the collection.
        /// </summary>
        /// <param name="langCode">The string identifying the language to add.</param>
        /// <returns><c>true</c> if the language could be added, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException"><paramref name="langCode"/> is not a valid language code.</exception>
        public bool Add(string langCode) => Add(Languages.FromString(langCode));

        /// <summary>
        /// Determines whether this language set contains any of a set of target languages.
        /// </summary>
        /// <param name="targets">The language set to match against.</param>
        /// <param name="ignoreCountry"><c>true</c> to compare only the two-letter language name; <c>false</c> to also compare the country code.</param>
        public bool ContainsAny([NotNull, ItemNotNull] ICollection<CultureInfo> targets, bool ignoreCountry = false)
        {
            #region Sanity checks
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            #endregion

            if (ignoreCountry)
            {
                var targetWeakNames = targets.Select(x => x.TwoLetterISOLanguageName);
                var thisWeakNames = this.Select(x => x.TwoLetterISOLanguageName);
                return targetWeakNames.Any(thisWeakNames.Contains);
            }
            else return targets.Any(Contains);
        }

        /// <summary>
        /// Serializes the list as a space-separated list of languages codes.
        /// </summary>
        public override string ToString()
        {
            // Serialize list as string split by spaces
            var output = new StringBuilder();
            foreach (var language in this)
            {
                // .NET uses a hyphen while Unix uses an underscore as a separator
                output.Append(language.ToString().Replace('-', '_') + ' ');
            }

            // Return without trailing whitespaces
            return output.ToString().TrimEnd();
        }
    }
}
