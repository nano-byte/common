// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Collections;

namespace NanoByte.Common.StructureEditor
{
    /// <summary>
    /// Describes an object that contains properties and/or lists. Provides information about how to edit this content.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container to be described.</typeparam>
    public partial class ContainerDescription<TContainer> where TContainer : class
    {
        private readonly List<DescriptionBase> _descriptions = new List<DescriptionBase>();

        /// <summary>
        /// Returns information about entries found in a specific instance of <typeparamref name="TContainer"/>.
        /// </summary>
        /// <param name="container">The container instance to look in to.</param>
        /// <returns>A list of entry information structures.</returns>
        internal IEnumerable<EntryInfo> GetEntriesIn(TContainer container)
            => _descriptions.SelectMany(description => description.GetEntriesIn(container));

        /// <summary>
        /// Returns information about possible new children for a specific instance of <typeparamref name="TContainer"/>.
        /// </summary>
        /// <param name="container">The container instance to look at.</param>
        /// <returns>A list of child information structures.</returns>
        internal IEnumerable<ChildInfo> GetPossibleChildrenFor(TContainer container)
            => _descriptions.SelectMany(description => description.GetPossibleChildrenFor(container))
                .Append(null); // split marker

        private abstract class DescriptionBase
        {
            public abstract IEnumerable<EntryInfo> GetEntriesIn(TContainer container);
            public abstract IEnumerable<ChildInfo> GetPossibleChildrenFor(TContainer container);
        }
    }
}
