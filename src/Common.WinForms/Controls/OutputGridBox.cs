// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Native;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Displays tabular data to the user.
    /// </summary>
    public sealed partial class OutputGridBox : Form
    {
        private OutputGridBox()
        {
            InitializeComponent();
            HandleCreated += delegate { WindowsTaskbar.PreventPinning(Handle); };
        }

        /// <summary>
        /// Displays an output dialog with tabular data.
        /// </summary>
        /// <typeparam name="T">The type of the data elements to display.</typeparam>
        /// <param name="owner">The parent window for the dialogs; can be <c>null</c>.</param>
        /// <param name="title">A title for the data.</param>
        /// <param name="data">The data to display.</param>
        public static void Show<T>([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string title, [NotNull, ItemNotNull] IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            using (var dialog = new OutputGridBox())
            {
                dialog.Text = title;
                dialog.SetData(data);

                dialog.ShowDialog(owner);
            }
        }

        /// <summary>
        /// Simple wrapper around objects to make them appear as a single column in a <see cref="DataGridView"/>.
        /// </summary>
        private class SimpleEntry<T>
        {
            public T Value { get; }

            public SimpleEntry(T value)
            {
                Value = value;
            }
        }

        private void SetData<T>(IEnumerable<T> data)
        {
            if (typeof(T) == typeof(string) || typeof(T) == typeof(Uri) || typeof(T).IsSubclassOf(typeof(Uri)))
                dataGrid.DataSource = data.Select(x => new SimpleEntry<T>(x)).ToList();
            else dataGrid.DataSource = data.ToList();
        }
    }
}
