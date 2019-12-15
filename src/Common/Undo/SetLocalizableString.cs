// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using NanoByte.Common.Collections;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// An undo command that sets a <see cref="LocalizableString"/> in a <see cref="LocalizableStringCollection"/>.
    /// </summary>
    public sealed class SetLocalizableString : SimpleCommand
    {
        /// <summary>
        /// The collection to be modified.
        /// </summary>
        private readonly LocalizableStringCollection _collection;

        /// <summary>
        /// The element to be added or removed from <see cref="_collection"/>.
        /// </summary>
        private readonly LocalizableString _entry;

        /// <summary>
        /// Creates a new localizable string command.
        /// </summary>
        /// <param name="collection">The collection to be modified.</param>
        /// <param name="element">The entry to be set in the <paramref name="collection"/>.</param>
        public SetLocalizableString(LocalizableStringCollection collection, LocalizableString element)
        {
            _collection = collection;
            _entry = element;
        }

        private string? _previousValue;

        /// <summary>
        /// Sets the entry in the collection.
        /// </summary>
        protected override void OnExecute()
        {
            try
            {
                _previousValue = _collection.GetExactLanguage(_entry.Language);
            }
            catch (KeyNotFoundException)
            {}
            _collection.Set(_entry.Language, _entry.Value);
        }

        /// <summary>
        /// Restores the original entry in the collection.
        /// </summary>
        protected override void OnUndo() => _collection.Set(_entry.Language, _previousValue);
    }
}
