// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Represents a callback delegate that has been registered with a <see cref="CancellationToken"/>.
    /// </summary>
    /// <remarks>Unlike the built-in CancellationToken type of .NET the NanoByte.Common variant supports remoting.</remarks>
    [Serializable]
    public struct CancellationTokenRegistration : IDisposable
    {
        [SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Justification = "Access to this field is remoted.")]
        [CanBeNull]
        private readonly CancellationTokenSource _source;

        private readonly Action _callback;

        internal CancellationTokenRegistration([CanBeNull] CancellationTokenSource source, [NotNull] Action callback)
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
