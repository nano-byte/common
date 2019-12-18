// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Defines a provider for progress updates.
    /// </summary>
    /// <remarks>Implementations should derive from <see cref="MarshalByRefObject"/>.</remarks>
    /// <remarks>Unlike the built-in Progress type of .NET implementations of the NanoByte.Common variant should derive from <see cref="MarshalByRefObject"/> to support remoting.</remarks>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    public interface IProgress
#if NET40 || NET45 || NET461 || NETSTANDARD
        <in T>
#else
        <T>
#endif
    {
        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        /// <remarks>May be called from background/worker threads. Callee must perform thread marshaling as needed.</remarks>
        void Report(T value);
    }
}
