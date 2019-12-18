// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET45 || NET461 || NETSTANDARD
namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Provides extension methods for <see cref="IProgress{T}"/>s.
    /// </summary>
    public static class ProgressExtensions
    {
        /// <summary>
        /// Converts a NanoByte.Common progress handler to a regular .NET progress handler.
        /// </summary>
        public static System.IProgress<T> ToSystem<T>(IProgress<T> progress)
        {
            return new System.Progress<T>(progress.Report);
        }

        /// <summary>
        /// Converts a regular .NET progress handler to a NanoByte.Common progress handler.
        /// </summary>
        public static IProgress<T> ToNanoByte<T>(System.IProgress<T> progress)
        {
            return new Progress<T>(progress.Report);
        }
    }
}
#endif
