// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing.Design;
using NanoByte.Common.Values.Design;

#if !NET
using System.Security.Permissions;
#endif

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

        private readonly ToolStripMenuItem _menuReset = new() {Text = Resources.ResetValue};

        public ResettablePropertyGrid()
        {
            _menuReset.Click += delegate
            {
                var oldValue = SelectedGridItem.Value;
                ResetSelectedProperty();
                OnPropertyValueChanged(new PropertyValueChangedEventArgs(SelectedGridItem, oldValue));
            };

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            ContextMenuStrip = new ContextMenuStrip {Items = {_menuReset}};
        }

#if !NET
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
#endif
        protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            #region Sanity checks
            if (e == null) throw new ArgumentNullException(nameof(e));
            #endregion

            _menuReset.Enabled = e.NewSelection?.PropertyDescriptor != null && e.NewSelection.Parent != null && e.NewSelection.PropertyDescriptor.CanResetValue(e.NewSelection.Parent.Value ?? SelectedObject);

            base.OnSelectedGridItemChanged(e);
        }
    }
}
