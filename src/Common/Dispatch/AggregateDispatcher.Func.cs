// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NanoByte.Common.Collections;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Calls different function delegates (with enumerable return values) based on the runtime types of objects.
    /// Aggregates results when multiple delegates match a type (through inheritance).
    /// </summary>
    /// <typeparam name="TBase">The common base type of all objects to be dispatched.</typeparam>
    /// <typeparam name="TResult">The enumerable return values of the delegates.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class AggregateDispatcher<TBase, TResult> : IEnumerable<Func<TBase, IEnumerable<TResult>>>
        where TBase : class
    {
        private readonly List<Func<TBase, IEnumerable<TResult>?>> _delegates = new List<Func<TBase, IEnumerable<TResult>?>>();

        /// <summary>
        /// Adds a dispatch delegate.
        /// </summary>
        /// <typeparam name="TSpecific">The specific type to call the delegate for. Matches all subtypes as well.</typeparam>
        /// <param name="function">The delegate to call.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public AggregateDispatcher<TBase, TResult> Add<TSpecific>(Func<TSpecific, IEnumerable<TResult>> function) where TSpecific : class, TBase
        {
            #region Sanity checks
            if (function == null) throw new ArgumentNullException(nameof(function));
            #endregion

            _delegates.Add(value => !(value is TSpecific specificValue) ? null : function(specificValue));

            return this;
        }

        /// <summary>
        /// Dispatches an element to all delegates matching the type. Set up with <see cref="Add{TSpecific}"/> first.
        /// </summary>
        /// <param name="element">The element to be dispatched.</param>
        /// <returns>The values returned by all matching delegates aggregated.</returns>
        public IEnumerable<TResult> Dispatch(TBase element)
        {
            #region Sanity checks
            if (element == null) throw new ArgumentNullException(nameof(element));
            #endregion

            return _delegates.Select(del => del(element)).WhereNotNull().Flatten();
        }

        /// <summary>
        /// Dispatches for each element in a collection. Set up with <see cref="Add{TSpecific}"/> first.
        /// </summary>
        /// <param name="elements">The elements to be dispatched.</param>
        /// <returns>The values returned by the matching delegates.</returns>
        public IEnumerable<TResult> Dispatch(IEnumerable<TBase> elements)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            return elements.SelectMany(Dispatch);
        }

        #region IEnumerable
        public IEnumerator<Func<TBase, IEnumerable<TResult>>> GetEnumerator() => _delegates.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _delegates.GetEnumerator();
        #endregion
    }
}
