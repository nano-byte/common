// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// A keyed collection (pseudo-dictionary) of <see cref="INamed{T}"/> objects. Case-insensitive!
    /// </summary>
    /// <remarks>Elements are automatically maintained in an alphabetically sorted order. Suitable for XML serialization.</remarks>
    public class NamedCollection<T> : KeyedCollection<string, T>, ICloneable<NamedCollection<T>> where T : INamed<T>
    {
        #region Events
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        public event Action<object>? CollectionChanged;

        private void OnCollectionChanged() => CollectionChanged?.Invoke(this);
        #endregion

        /// <summary>
        /// Creates a new named collection.
        /// </summary>
        public NamedCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {}

        /// <summary>
        /// Creates a new named collection pre-filled with elements.
        /// </summary>
        /// <param name="elements">The elements to pre-fill the collection with. Must all have unique <see cref="INamed{T}.Name"/>s!</param>
        public NamedCollection(IEnumerable<T> elements)
            : this()
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            foreach (var element in elements) Add(element);
        }

        /// <summary>
        /// Renames an element in the list. Renaming an element in the list directly (without using this method) will prevent lookups from working properly!
        /// </summary>
        /// <param name="element">The element to rename.</param>
        /// <param name="newName">The new <see cref="INamed{T}.Name"/> for the element.</param>
        /// <exception cref="KeyNotFoundException">The <paramref name="element"/> is not in the collection.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="newName"/> is already taken by another element in the collection.</exception>
        public void Rename(T element, [Localizable(false)] string newName)
        {
            #region Sanity checks
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (string.IsNullOrEmpty(newName)) throw new ArgumentNullException(nameof(newName));
            #endregion

            if (!Contains(element)) throw new KeyNotFoundException();
            if (element.Name == newName) return;
            if (Contains(newName)) throw new InvalidOperationException(Resources.KeyAlreadyPresent);

            ChangeItemKey(element, newName);
            element.Name = newName;
            Sort();
            OnCollectionChanged();
        }

        /// <summary>
        /// Sorts all elements alphabetically by their <see cref="INamed{T}.Name"/>.
        /// </summary>
        private void Sort()
        {
            var items = Items as List<T>;
            items?.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        }

        #region Hooks
        /// <inheritdoc/>
        protected override string GetKeyForItem(T item) => item.Name;

        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            Sort();
            OnCollectionChanged();
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            Sort();
            OnCollectionChanged();
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            OnCollectionChanged();
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged();
        }
        #endregion

        //--------------------//

        #region Clone
        /// <summary>
        /// Creates a shallow copy of this collection (elements are not cloned).
        /// </summary>
        /// <returns>The cloned collection.</returns>
        public virtual NamedCollection<T> Clone() => new NamedCollection<T>(this);
        #endregion
    }
}
