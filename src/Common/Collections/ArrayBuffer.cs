// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Buffers;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// An array rented from the <see cref="ArrayPool{T}"/>.
    /// </summary>
    public sealed class ArrayBuffer<T> : IDisposable
    {
        /// <summary>
        /// The requested length of the array.
        /// </summary>
        public int Length { get; }

        private T[]? _buffer;

        /// <summary>
        /// Rents an array from the <see cref="ArrayPool{T}"/>.
        /// </summary>
        /// <param name="length">The desired array length.</param>
        public ArrayBuffer(int length)
        {
            Length = length;
            _buffer = ArrayPool<T>.Shared.Rent(length);
        }

        /// <summary>
        /// Returns the array to the <see cref="ArrayPool{T}"/>.
        /// </summary>
        public void Dispose()
        {
            if (_buffer != null)
            {
                ArrayPool<T>.Shared.Return(_buffer);
                _buffer = null;
            }
        }

        /// <summary>
        /// The array.
        /// The length may be equal to or greater than the requested <see cref="Length"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Dispose"/> has been called.</exception>
        public T[] Array
            => _buffer ?? throw new ObjectDisposedException(GetType().Name);

        /// <summary>
        /// A view of the array with exactly the requested <see cref="Length"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Dispose"/> has been called.</exception>
        public ArraySegment<T> Segment
            => new(Array, 0, Length);

        /// <summary>
        /// A view of the array with exactly the requested <see cref="Length"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Dispose"/> has been called.</exception>
        public Span<T> Span
            => new(Array, 0, Length);
    }
}
#endif
