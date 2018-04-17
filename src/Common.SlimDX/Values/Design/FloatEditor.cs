// Copyright Bastian Eicher
// Licensed under the MIT License

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
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

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

            var range = context.PropertyDescriptor?.Attributes.OfType<FloatRangeAttribute>().FirstOrDefault();
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
