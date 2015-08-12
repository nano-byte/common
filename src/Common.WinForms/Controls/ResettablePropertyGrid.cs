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
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using NanoByte.Common.Collections;
using NanoByte.Common.Properties;
using NanoByte.Common.Values.Design;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A <see cref="PropertyGrid"/> that provides a "reset value" option in its context menu.
    /// </summary>
    public class ResettablePropertyGrid : PropertyGrid
    {
        static ResettablePropertyGrid()
        {
            // Inject custom WinForms type editors
            TypeDescriptor.AddAttributes(typeof(TimeSpan), new EditorAttribute(typeof(TimeSpanEditor), typeof(UITypeEditor)));
            TypeDescriptor.AddAttributes(typeof(LanguageSet), new EditorAttribute(typeof(LanguageSetEditor), typeof(UITypeEditor)));
        }

        private readonly ToolStripMenuItem _menuReset = new ToolStripMenuItem {Text = Resources.ResetValue};

        public ResettablePropertyGrid()
        {
            _menuReset.Click += delegate
            {
                object oldValue = SelectedGridItem.Value;
                ResetSelectedProperty();
                OnPropertyValueChanged(new PropertyValueChangedEventArgs(SelectedGridItem, oldValue));
            };

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            ContextMenuStrip = new ContextMenuStrip {Items = {_menuReset}};
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            #region Sanity checks
            if (e == null) throw new ArgumentNullException(nameof(e));
            #endregion

            _menuReset.Enabled =
                e.NewSelection != null && e.NewSelection.PropertyDescriptor != null && e.NewSelection.Parent != null &&
                e.NewSelection.PropertyDescriptor.CanResetValue(e.NewSelection.Parent.Value ?? SelectedObject);

            base.OnSelectedGridItemChanged(e);
        }
    }
}
