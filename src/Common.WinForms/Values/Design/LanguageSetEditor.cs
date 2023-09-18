// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;

#if NETFRAMEWORK
using System.Security.Permissions;
#endif

namespace NanoByte.Common.Values.Design;

/// <summary>
/// An editor that can be associated with <see cref="LanguageSet"/> properties. Uses a <see cref="CheckedListBox"/>.
/// </summary>
#if NETFRAMEWORK
[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
#endif
public class LanguageSetEditor : UITypeEditor
{
    /// <inheritdoc/>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;

    /// <inheritdoc/>
    // ReSharper disable once NullnessAnnotationConflictWithJetBrainsAnnotations
    public override object EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        #region Sanity checks
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (provider == null) throw new ArgumentNullException(nameof(provider));
        #endregion

        if (value is not LanguageSet languages) throw new ArgumentNullException(nameof(value));

        var editorService = (IWindowsFormsEditorService?)provider.GetService(typeof(IWindowsFormsEditorService));
        if (editorService == null) return value;

        var listBox = new CheckedListBox {CheckOnClick = true};
        int i = 0;
        foreach (var language in languages)
        {
            listBox.Items.Add(language);
            listBox.SetItemChecked(i++, true);
        }
        foreach (var language in Languages.AllKnown.Except(languages))
            listBox.Items.Add(language);

        editorService.DropDownControl(listBox);

        return new LanguageSet(listBox.CheckedItems.Cast<CultureInfo>());
    }
}
