// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// Provides an interface to a dialog that edits a single object.
/// </summary>
/// <typeparam name="T">The type of object to edit.</typeparam>
public interface IEditorDialog<T> : IDisposable
{
    /// <summary>
    /// Displays a modal dialog for editing an element.
    /// </summary>
    /// <param name="owner">The parent window used to make the dialog modal.</param>
    /// <param name="element">The element to be edited.</param>
    /// <returns>Save changes if <see cref="DialogResult.OK"/>; discard changes if  <see cref="DialogResult.Cancel"/>.</returns>
    DialogResult ShowDialog(IWin32Window owner, T element);
}