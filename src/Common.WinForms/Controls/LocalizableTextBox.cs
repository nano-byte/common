// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using NanoByte.Common.Collections;
using NanoByte.Common.Undo;
using NanoByte.Common.Values;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A control for editing a <see cref="LocalizableStringCollection"/>.
    /// </summary>
    public partial class LocalizableTextBox : EditorControlBase<LocalizableStringCollection>
    {
        #region Variables
        private CultureInfo _selectedLanguage;
        private bool _textBoxDirty;
        #endregion

        #region Properties
        /// <summary>
        /// Controls whether the text can span more than one line.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior"), Description("Controls whether the text can span more than one line.")]
        public bool Multiline { get => textBox.Multiline; set => textBox.Multiline = value; }

        /// <summary>
        /// A text to be displayed in <see cref="SystemColors.GrayText"/> when the text box is empty.
        /// </summary>
        [Description("A text to be displayed in gray when Text is empty."), Category("Appearance"), Localizable(true)]
        public string HintText { get => textBox.HintText; set => textBox.HintText = value; }
        #endregion

        #region Constructor
        public LocalizableTextBox()
            : base(showDescriptionBox: false)
        {
            InitializeComponent();
        }
        #endregion

        //--------------------//

        #region Fill
        private void FillComboBox()
        {
            var setLanguages = new List<CultureInfo>();
            var unsetLanguages = new List<CultureInfo>();
            foreach (var language in Languages.AllKnown)
            {
                if (Target.ContainsExactLanguage(language)) setLanguages.Add(language);
                else unsetLanguages.Add(language);
            }

            if (_selectedLanguage == null)
                _selectedLanguage = setLanguages.Count == 0 ? LocalizableString.DefaultLanguage : setLanguages[0];

            comboBoxLanguage.BeginUpdate();
            comboBoxLanguage.Items.Clear();
            foreach (var language in setLanguages) comboBoxLanguage.Items.Add(language);
            foreach (var language in unsetLanguages) comboBoxLanguage.Items.Add(language);
            comboBoxLanguage.SelectedItem = _selectedLanguage;
            comboBoxLanguage.EndUpdate();
        }

        private void FillTextBox()
        {
            try
            {
                textBox.Text = Target.GetExactLanguage(_selectedLanguage);
            }
            catch (KeyNotFoundException)
            {
                textBox.Text = "";
            }
            _textBoxDirty = false;
        }
        #endregion

        #region Apply
        private void ApplyValue()
        {
            string newValue = string.IsNullOrEmpty(textBox.Text) ? null : textBox.Text;

            if (Target.GetExactLanguage(_selectedLanguage) == newValue) return;

            if (CommandExecutor == null) Target.Set(_selectedLanguage, newValue);
            else CommandExecutor.Execute(new SetLocalizableString(Target, new LocalizableString {Language = _selectedLanguage, Value = newValue}));
        }
        #endregion

        #region Event handlers
        private void comboBoxLanguage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _selectedLanguage = (CultureInfo)comboBoxLanguage.SelectedItem;
            FillTextBox();
        }

        private void textBox_TextChanged(object sender, EventArgs e) => _textBoxDirty = true;

        private void textBox_Validating(object sender, CancelEventArgs e)
        {
            if (_selectedLanguage == null) return;

            if (_textBoxDirty)
            {
                ApplyValue();
                _textBoxDirty = false;
            }
        }

        public override void Refresh()
        {
            FillComboBox();
            FillTextBox();

            base.Refresh();
        }
        #endregion
    }
}
