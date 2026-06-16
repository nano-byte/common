// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// A keyed collection (pseudo-dictionary) of <see cref="INamed"/> objects that are also <see cref="ICloneable{T}"/>.
/// </summary>
public class ClonableNamedCollection<T> : NamedCollection<T>, ICloneable<ClonableNamedCollection<T>>
    where T : INamed, ICloneable<T>
{
    /// <summary>
    /// Creates a new clonable named collection.
    /// </summary>
    public ClonableNamedCollection()
    {}

    /// <summary>
    /// Creates a new clonable named collection pre-filled with elements.
    /// </summary>
    /// <param name="elements">The elements to pre-fill the collection with. Must all have unique <see cref="INamed.Name"/>s!</param>
    public ClonableNamedCollection([InstantHandle] IEnumerable<T> elements)
        : base(elements)
    {}

    /// <summary>
    /// Creates a deep clone of this collection.
    /// </summary>
    public ClonableNamedCollection<T> Clone() => new(this.CloneElements());
}
