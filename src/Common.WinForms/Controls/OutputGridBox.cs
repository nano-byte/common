/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
        /// Displays an ouput dialog with tabular data.
        /// </summary>
        /// <typeparam name="T">The type of the data elements to display.</typeparam>
        /// <param name="owner">The parent window for the dialogs; can be <c>null</c>.</param>
        /// <param name="title">A title for the data.</param>
        /// <param name="data">The data to display.</param>
        public static void Show<T>([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string title, [NotNull, ItemNotNull] IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException("title");
            if (data == null) throw new ArgumentNullException("data");
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
            public T Value { get; private set; }

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
