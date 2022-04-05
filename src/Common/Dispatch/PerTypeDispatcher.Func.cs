// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;

namespace NanoByte.Common.Dispatch;

/// <summary>
/// Calls different function delegates (with return values) based on the runtime types of objects.
/// Types must be exact matches. Inheritance is not considered.
/// </summary>
/// <typeparam name="TBase">The common base type of all objects to be dispatched.</typeparam>
/// <typeparam name="TResult">The return value of the delegates.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
public class PerTypeDispatcher<TBase, TResult> : IEnumerable<KeyValuePair<Type, Func<TBase, TResult>>> where TBase : class
{
    private readonly Dictionary<Type, Func<TBase, TResult>> _map = new();

    /// <summary>
    /// Adds a dispatch delegate.
    /// </summary>
    /// <typeparam name="TSpecific">The specific type to call the delegate for. Does not match subtypes.</typeparam>
    /// <param name="function">The delegate to call.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public PerTypeDispatcher<TBase, TResult> Add<TSpecific>(Func<TSpecific, TResult> function) where TSpecific : TBase
    {
        #region Sanity checks
        if (function == null) throw new ArgumentNullException(nameof(function));
        #endregion

        _map.Add(typeof(TSpecific), obj => function((TSpecific)obj));

        return this;
    }

    /// <summary>
    /// Dispatches an element to the delegate matching the type. Set up with <see cref="Add{TSpecific}"/> first.
    /// </summary>
    /// <param name="element">The element to be dispatched.</param>
    /// <returns>The value returned by the matching delegate.</returns>
    /// <exception cref="KeyNotFoundException">No delegate matching the <paramref name="element"/> type was <see cref="Add{TSpecific}"/>ed.</exception>
    public TResult Dispatch(TBase element)
    {
        #region Sanity checks
        if (element == null) throw new ArgumentNullException(nameof(element));
        #endregion

        var type = element.GetType();
        if (_map.TryGetValue(type, out var function)) return function(element);
        else throw new KeyNotFoundException(string.Format(Resources.MissingDispatchAction, type.Name));
    }

    /// <summary>
    /// Dispatches for each element in a collection. Set up with <see cref="Add{TSpecific}"/> first.
    /// </summary>
    /// <param name="elements">The elements to be dispatched.</param>
    /// <returns>The values returned by the matching delegates.</returns>
    /// <exception cref="KeyNotFoundException">No delegate matching one of the element types was <see cref="Add{TSpecific}"/>ed.</exception>
    public IEnumerable<TResult> Dispatch([InstantHandle] IEnumerable<TBase> elements)
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        #endregion

        return elements.Select(Dispatch);
    }

    #region IEnumerable
    public IEnumerator<KeyValuePair<Type, Func<TBase, TResult>>> GetEnumerator() => _map.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _map.GetEnumerator();
    #endregion
}
