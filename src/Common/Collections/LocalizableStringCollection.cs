// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// A collection of <see cref="LocalizableString"/>s with language-search methods.
    /// </summary>
    [Serializable]
    public class LocalizableStringCollection : List<LocalizableString>, ICloneable<LocalizableStringCollection>
    {
        /// <summary>
        /// Adds a new string with an associated language to the collection.
        /// </summary>
        /// <param name="language">The language of the <paramref name="value"/>.</param>
        /// <param name="value">The actual string value to store.</param>
        public void Add([Localizable(false)] string language, string? value)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            Add(new LocalizableString {LanguageString = language, Value = value});
        }

        /// <summary>
        /// Adds a new <c>en</c> string to the collection.
        /// </summary>
        /// <param name="value">The actual string value to store.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo")]
        public void Add(string value) => Add(new LocalizableString {Value = value});

        /// <summary>
        /// Checks if the collection contains an entry exactly matching the specified language.
        /// </summary>
        /// <param name="language">The exact language to look for.</param>
        /// <returns><c>true</c> if an element with the specified language exists in the collection; <c>false</c> otherwise.</returns>
        /// <seealso cref="GetExactLanguage"/>
        public bool ContainsExactLanguage(CultureInfo language)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            return this.Any(entry => Equals(language, entry.Language));
        }

        /// <summary>
        /// Returns the first string in the collection exactly matching the specified language.
        /// </summary>
        /// <param name="language">The exact language to look for.</param>
        /// <returns>The string value found in the collection; <c>null</c> if none was found.</returns>
        /// <seealso cref="ContainsExactLanguage"/>
        public string? GetExactLanguage(CultureInfo language)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            var match = this.FirstOrDefault(entry => Equals(language, entry.Language));
            return match?.Value;
        }

        /// <summary>
        /// Returns the best-fitting string in the collection for the specified language.
        /// </summary>
        /// <param name="language">The language to look for.</param>
        /// <returns>The best-fitting string value found in the collection; <c>null</c> if the collection is empty.</returns>
        /// <remarks>
        /// Language preferences in decreasing order:<br/>
        /// 1. exact match<br/>
        /// 2. same language with neutral culture<br/>
        /// 3. en<br/>
        /// 4. en-US<br/>
        /// 5. first entry in collection
        /// </remarks>
        public string? GetBestLanguage(CultureInfo language)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            // Try to find exact match
            foreach (var entry in this.Where(entry => Equals(language, entry.Language)))
                return entry.Value;

            // Try to find same language with neutral culture
            foreach (var entry in this.Where(entry => language.TwoLetterISOLanguageName == entry.Language.TwoLetterISOLanguageName && entry.Language.IsNeutralCulture))
                return entry.Value;

            // Try to find "en"
            var en = LocalizableString.DefaultLanguage;
            foreach (var entry in this.Where(entry => en.Equals(entry.Language)))
                return entry.Value;

            // Try to find "en-US"
            var enUs = new CultureInfo("en-US");
            foreach (var entry in this.Where(entry => enUs.Equals(entry.Language)))
                return entry.Value;

            // Try to find first entry in collection
            return Count == 0 ? null : this[0].Value;
        }

        /// <summary>
        /// Adds a new string with an associated language to the collection. Preexisting entries with the same language are removed.
        /// </summary>
        /// <param name="language">The language of the <paramref name="value"/>.</param>
        /// <param name="value">The actual string value to store; <c>null</c> to remove existing entries.</param>
        public void Set(CultureInfo language, string? value)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            RemoveAll(entry => language.Equals(entry.Language));
            if (value != null) Add(new LocalizableString {Language = language, Value = value});
        }

        #region Clone
        /// <summary>
        /// Creates a deep copy of this <see cref="LocalizableStringCollection"/> (elements are cloned).
        /// </summary>
        /// <returns>The cloned <see cref="LocalizableStringCollection"/>.</returns>
        public LocalizableStringCollection Clone()
        {
            var newDict = new LocalizableStringCollection();
            newDict.AddRange(this.Select(entry => entry.Clone()));
            return newDict;
        }
        #endregion
    }
}
