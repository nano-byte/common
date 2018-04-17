// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Edits arbitrary types of elements using an <see cref="GenericEditorControl{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of element to edit.</typeparam>
    public partial class EditorDialog<T> : OKCancelDialog, IEditorDialog<T> where T : class
    {
        // Don't use WinForms designer for this, since it doesn't understand generics
        private readonly GenericEditorControl<T> _editor = new GenericEditorControl<T>
        {
            TabIndex = 0,
            Location = new Point(12, 12),
            Size = new Size(289, 278),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        public EditorDialog()
        {
            InitializeComponent();
            Controls.Add(_editor);

            Load += delegate { Text = typeof(T).Name; };
        }

        /// <inheritdoc/>
        public DialogResult ShowDialog([NotNull] IWin32Window owner, [NotNull] T element)
        {
            #region Sanity checks
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (element == null) throw new ArgumentNullException(nameof(element));
            #endregion

            _editor.Target = element;

            return ShowDialog(owner);
        }

        /// <summary>
        /// Displays a modal dialog without a parent (listed in the taskbar) for editing an element.
        /// </summary>
        /// <param name="element">The element to be edited.</param>
        /// <returns>Save changes if <see cref="DialogResult.OK"/>; discard changes if <see cref="DialogResult.Cancel"/>.</returns>
        public DialogResult ShowDialog([NotNull] T element)
        {
            #region Sanity checks
            if (element == null) throw new ArgumentNullException(nameof(element));
            #endregion

            _editor.Target = element;

            ShowInTaskbar = true;
            return ShowDialog();
        }
    }
}
