// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;
using NanoByte.Common.Undo;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Provides an interface to a control that edits a single object.
    /// </summary>
    /// <typeparam name="T">The type of object to edit.</typeparam>
    public interface IEditorControl<T>
    {
        /// <summary>
        /// The element to be edited.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        T Target { get; set; }

        /// <summary>
        /// An optional undo system to use for editing.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ICommandExecutor CommandExecutor { get; set; }
    }
}
