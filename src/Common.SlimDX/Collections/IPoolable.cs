// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// An interface items must implement to be addable to <see cref="Pool{T}"/>. Poolable items directly store a reference to their successor.
    /// </summary>
    /// <typeparam name="T">The type of items to store in <see cref="Pool{T}"/>.</typeparam>
    public interface IPoolable<T> where T : class, IPoolable<T>
    {
        /// <summary>
        /// A reference to the next element in the <see cref="Pool{T}"/> chain.
        /// </summary>
        T NextElement { get; set; }
    }
}
