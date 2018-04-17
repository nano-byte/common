// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using JetBrains.Annotations;

namespace NanoByte.Common
{
    /// <summary>
    /// Invokes a callback on <see cref="Dispose"/>.
    /// </summary>
    public sealed class Disposable : IDisposable
    {
        [NotNull]
        private readonly Action _callback;

        /// <summary>
        /// Creates a new disposable.
        /// </summary>
        /// <param name="callback">The callback to invoke on <see cref="Dispose"/>.</param>
        public Disposable([NotNull] Action callback)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <summary>
        /// Invokes the callback.
        /// </summary>
        public void Dispose() => _callback();
    }
}
