// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;

namespace NanoByte.Common.Dispatch;

/// <summary>
/// Calls different action delegates based on the runtime types of objects.
/// Types must be exact matches. Inheritance is not considered.
/// </summary>
/// <param name="ignoreMissing"><c>true</c> to silently ignore dispatch attempts on unknown types; <c>false</c> to throw exceptions.</param>
/// <typeparam name="TBase">The common base type of all objects to be dispatched.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
public class PerTypeDispatcher<TBase>(bool ignoreMissing) : IEnumerable<KeyValuePair<Type, Action<object>>>
    where TBase : class
{
    private readonly Dictionary<Type, Action<object>> _map = [];

    /// <summary>
    /// Adds a dispatch delegate.
    /// </summary>
    /// <typeparam name="TSpecific">The specific type to call the delegate for. Does not match subtypes</typeparam>
    /// <param name="action">The delegate to call.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public PerTypeDispatcher<TBase> Add<TSpecific>(Action<TSpecific> action) where TSpecific : TBase
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
    /// <exception cref="KeyNotFoundException">No delegate matching the <paramref name="element"/> type was <see cref="Add{TSpecific}"/>ed.</exception>
    public void Dispatch(TBase element)
    {
        #region Sanity checks
        if (element == null) throw new ArgumentNullException(nameof(element));
        #endregion

        var type = element.GetType();
        if (_map.TryGetValue(type, out var action)) action(element);
        else if (!ignoreMissing) throw new KeyNotFoundException(string.Format(Resources.MissingDispatchAction, type.Name));
    }

    /// <summary>
    /// Dispatches for each element in a collection. Set up with <see cref="Add{TSpecific}"/> first.
    /// </summary>
    /// <param name="elements">The elements to be dispatched.</param>
    /// <exception cref="KeyNotFoundException">No delegate matching one of the element types was <see cref="Add{TSpecific}"/>ed.</exception>
    public void Dispatch([InstantHandle] IEnumerable<TBase> elements)
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        #endregion

        foreach (var element in elements)
            Dispatch(element);
    }

    #region IEnumerable
    public IEnumerator<KeyValuePair<Type, Action<object>>> GetEnumerator() => _map.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _map.GetEnumerator();
    #endregion
}
