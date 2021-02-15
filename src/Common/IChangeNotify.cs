// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;

namespace NanoByte.Common
{
    /// <summary>
    /// An object that can notify interested parties of changes in properties of interest.
    /// </summary>
    /// <typeparam name="TSender">The type of the class implementing this interface.</typeparam>
    public interface IChangeNotify
#if NET20
        <TSender>
#else
        <out TSender>
#endif
    {
        /// <summary>
        /// Occurs when a property of interest has changed.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        event Action<TSender> Changed;

        /// <summary>
        /// Occurs when a property changed that requires visual representations to rebuilt from scratch.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        event Action<TSender> ChangedRebuild;
    }
}
