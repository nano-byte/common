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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Calls different action delegates based on the runtime types of objects.
    /// Types must be exact matches. Inheritance is not considered.
    /// </summary>
    /// <typeparam name="TBase">The common base type of all objects to be dispatched.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class PerTypeDispatcher<TBase> : IEnumerable<KeyValuePair<Type, Action<object>>> where TBase : class
    {
        private readonly Dictionary<Type, Action<object>> _map = new Dictionary<Type, Action<object>>();

        /// <summary><c>true</c> to silently ignore dispatch attempts on unknown types; <c>false</c> to throw exceptions.</summary>
        private readonly bool _ignoreMissing;

        /// <summary>
        /// Creates a new dispatcher.
        /// </summary>
        /// <param name="ignoreMissing"><c>true</c> to silently ignore dispatch attempts on unknown types; <c>false</c> to throw exceptions.</param>
        public PerTypeDispatcher(bool ignoreMissing)
        {
            _ignoreMissing = ignoreMissing;
        }

        /// <summary>
        /// Adds a dispatch delegate.
        /// </summary>
        /// <typeparam name="TSpecific">The specific type to call the delegate for. Does not match subtypes</typeparam>
        /// <param name="action">The delegate to call.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [PublicAPI]
        public PerTypeDispatcher<TBase> Add<TSpecific>([NotNull] Action<TSpecific> action) where TSpecific : TBase
        {
            #region Sanity checks
            if (action == null) throw new ArgumentNullException(nameof(action));
            #endregion

            _map.Add(typeof(TSpecific), obj => action((TSpecific)obj));

            return this;
        }

        /// <summary>
        /// Dispatches an element to the delegate matching the type. Set up with <see cref="Add{TSpecific}"/> first.
        /// </summary>
        /// <param name="element">The element to be dispatched.</param>
        /// <exception cref="KeyNotFoundException">No delegate matching the <paramref name="element"/> type was <see cref="Add{TSpecific}"/>ed and <see cref="_ignoreMissing"/> is <c>false</c>.</exception>
        public void Dispatch([NotNull] TBase element)
        {
            #region Sanity checks
            if (element == null) throw new ArgumentNullException(nameof(element));
            #endregion

            var type = element.GetType();
            if (_map.TryGetValue(type, out var action)) action(element);
            else if (!_ignoreMissing) throw new KeyNotFoundException(string.Format(Resources.MissingDispatchAction, type.Name));
        }

        /// <summary>
        /// Dispatches for each element in a collection. Set up with <see cref="Add{TSpecific}"/> first.
        /// </summary>
        /// <param name="elements">The elements to be dispatched.</param>
        /// <exception cref="KeyNotFoundException">No delegate matching one of the element types was <see cref="Add{TSpecific}"/>ed and <see cref="_ignoreMissing"/> is <c>false</c>.</exception>
        public void Dispatch([NotNull, ItemNotNull] IEnumerable<TBase> elements)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            foreach (var element in elements)
                Dispatch(element);
        }

        #region IEnumerable
        public IEnumerator<KeyValuePair<Type, Action<object>>> GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetEnumerator();
        }
        #endregion
    }
}
