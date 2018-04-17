// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Controls;

namespace NanoByte.Common.StructureEditor
{
    /// <summary>
    /// Exposes methods for configuring a list in a <see cref="ContainerDescription{TContainer}"/> in a Fluent API style.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    public interface IListDescription<TContainer, TList>
        where TList : class
    {
        /// <summary>
        /// Adds a list element type to the description.
        /// </summary>
        /// <param name="name">The name of the element type.</param>
        /// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying this type of element.</typeparam>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generics used as type-safe reflection replacement.")]
        IListDescription<TContainer, TList> AddElement<TElement, TEditor>(string name)
            where TElement : class, TList, IEquatable<TElement>, new()
            where TEditor : Control, IEditorControl<TElement>, new();

        /// <summary>
        /// Adds a list element type to the description. Gives the <typeparamref name="TEditor"/> access to the <typeparamref name="TContainer"/>.
        /// </summary>
        /// <param name="name">The name of the element type.</param>
        /// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying this type of element.</typeparam>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generics used as type-safe reflection replacement.")]
        [PublicAPI]
        IListDescription<TContainer, TList> AddElementContainerRef<TElement, TEditor>(string name)
            where TElement : class, TList, IEquatable<TElement>, new()
            where TEditor : Control, IEditorControlContainerRef<TElement, TContainer>, new();

        /// <summary>
        /// Adds a list element type to the description.
        /// </summary>
        /// <param name="name">The name of the element type.</param>
        /// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generics used as type-safe reflection replacement.")]
        [PublicAPI]
        IListDescription<TContainer, TList> AddElement<TElement>(string name)
            where TElement : class, TList, IEquatable<TElement>, new();
    }
}
