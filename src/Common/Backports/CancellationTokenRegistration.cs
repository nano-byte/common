// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET20
using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Threading
{
    /// <summary>
    /// Represents a callback delegate that has been registered with a <see cref="CancellationToken"/>.
    /// </summary>
    public struct CancellationTokenRegistration : IDisposable
    {
        private readonly CancellationTokenSource? _source;

        private readonly Action _callback;

        internal CancellationTokenRegistration(CancellationTokenSource? source, Action callback)
        {
            _source = source;
            _callback = callback;

            if (_source != null) _source.CancellationRequested += _callback;
        }

        /// <summary>
        /// Unregisters the callback.
        /// </summary>
        public void Dispose()
        {
            if (_source != null) _source.CancellationRequested -= _callback;
        }
    }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Threading.CancellationTokenRegistration))]
#endif
