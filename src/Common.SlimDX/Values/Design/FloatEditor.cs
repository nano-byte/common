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
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NanoByte.Common.Values.Design
{
    /// <summary>
    /// Abstract base class for drop-down <see cref="PropertyGrid"/> editors that can be associated with <c>float</c> properties.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public abstract class FloatEditor : UITypeEditor
    {
        /// <inheritdoc/>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <inheritdoc/>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            #region Sanity checks
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (value == null) throw new ArgumentNullException(nameof(value));
            #endregion

            if (value.GetType() != typeof(float)) return value;

            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService == null) return value;

            var range = context.PropertyDescriptor.Attributes.OfType<FloatRangeAttribute>().FirstOrDefault();
            return (range == null)
                ? EditValue((float)value, editorService)
                : EditValue((float)value, range, editorService);
        }

        /// <summary>
        /// Displays the UI to edit the <c>float</c> value.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="editorService">The editor service used to display the dropdown control.</param>
        /// <returns>The value set by the user.</returns>
        protected abstract float EditValue(float value, IWindowsFormsEditorService editorService);

        /// <summary>
        /// Displays the UI to edit the <c>float</c> value.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="range">The range of valid values the user can select.</param>
        /// <param name="editorService">The editor service used to display the dropdown control.</param>
        /// <returns>The value set by the user.</returns>
        protected abstract float EditValue(float value, FloatRangeAttribute range, IWindowsFormsEditorService editorService);
    }
}
