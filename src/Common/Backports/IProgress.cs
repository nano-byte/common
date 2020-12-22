// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET20 || NET40
namespace System
{
    /// <summary>
    /// Interface for providing progress updates.
    /// </summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    public interface IProgress
#if NET20
        <T>
#else
        <in T>
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
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.IProgress<>))]
#endif
