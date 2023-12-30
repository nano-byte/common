// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;

namespace NanoByte.Common.Dispatch;

/// <summary>
/// Calls different action delegates based on the runtime types of objects.
/// Calls multiple delegates when they all match a type (through inheritance).
/// </summary>
/// <typeparam name="TBase">The common base type of all objects to be dispatched.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
public class AggregateDispatcher<TBase> : IEnumerable<Action<TBase>>
    where TBase : class
{
    private readonly List<Action<TBase>> _delegates = [];

    /// <summary>
    /// Adds a dispatch delegate.
    /// </summary>
    /// <typeparam name="TSpecific">The specific type to call the delegate for. Matches all subtypes as well.</typeparam>
    /// <param name="action">The delegate to call.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public AggregateDispatcher<TBase> Add<TSpecific>(Action<TSpecific> action) where TSpecific : class, TBase
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        _delegates.Add(value =>
        {
            if (value is TSpecific specificValue) action(specificValue);
        });

        return this;
    }

    /// <summary>
    /// Dispatches an element to all delegates matching the type. Set up with <see cref="Add{TSpecific}"/> first.
    /// </summary>
    /// <param name="element">The element to be dispatched.</param>
    public void Dispatch(TBase element)
    {
        #region Sanity checks
        if (element == null) throw new ArgumentNullException(nameof(element));
        #endregion

        foreach (var del in _delegates) del(element);
    }

    /// <summary>
    /// Dispatches for each element in a collection. Set up with <see cref="Add{TSpecific}"/> first.
    /// </summary>
    /// <param name="elements">The elements to be dispatched.</param>
    public void Dispatch([InstantHandle] IEnumerable<TBase> elements)
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        #endregion

        foreach (var element in elements)
            Dispatch(element);
    }

    #region IEnumerable
    public IEnumerator<Action<TBase>> GetEnumerator() => _delegates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _delegates.GetEnumerator();
    #endregion
}
