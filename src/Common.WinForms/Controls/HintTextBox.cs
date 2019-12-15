// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A special <see cref="TextBox"/> that displays a <see cref="HintText"/> when <see cref="TextBox.Text"/> is empty and a clear button when it is not.
    /// </summary>
    [Description("A special TextBox that displays a hint text when Text is empty.")]
    public class HintTextBox : TextBox
    {
        #region Events
        /// <inheritdoc/>
        protected override void OnEnter(EventArgs e)
        {
            // Remove the hint when entering the TextBox
            HideHintText();

            OnGotFocus(e);
        }

        /// <inheritdoc/>
        protected override void OnLeave(EventArgs e)
        {
            // Restore hint when leaving the TextBox with an empty Text
            if (string.IsNullOrEmpty(base.Text)) ShowHintText();

            base.OnLeave(e);
        }

        /// <inheritdoc/>
        protected override void OnTextChanged(EventArgs e)
        {
            // Show clear button only if it is enabled and there is text that can be cleared
            _buttonClear.Visible = _showClearButton && !string.IsNullOrEmpty(base.Text) && !IsHintTextVisible;

            // Prevent displaying of hint text from raising events
            if (_suppressTextChangedEvent) return;

            base.OnTextChanged(e);
        }

        /// <inheritdoc/>
        protected override void OnBackColorChanged(EventArgs e)
        {
            // Make sure the clear button has the same background as the TextBox
            _buttonClear.BackColor = BackColor;

            base.OnBackColorChanged(e);
        }

        /// <summary>
        /// Occurs when the clear button is clicked.
        /// </summary>
        [Description("Occurs when the clear button is clicked.")]
        public event EventHandler? ClearButtonClicked;

        /// <summary>
        /// Raises the <see cref="ClearButtonClicked"/> event.
        /// </summary>
        protected virtual void OnClearButtonClicked(EventArgs e) => ClearButtonClicked?.Invoke(this, e);
        #endregion

        #region Variables
        /// <summary>Prevents <see cref="OnTextChanged"/> from raising any events.</summary>
        private bool _suppressTextChangedEvent;

        private static readonly Bitmap _clearButton = ImageResources.ClearButton;

        private readonly PictureBox _buttonClear = new PictureBox
        {
            Visible = false,
            Cursor = Cursors.Default,
            Location = new Point(81, 1),
            Size = new Size(14, 14),
            Dock = DockStyle.Right,
            BackColor = SystemColors.Window,
            Image = _clearButton,
            SizeMode = PictureBoxSizeMode.Zoom
        };
        #endregion

        #region Properties
        private Color _foreColor = SystemColors.ControlText;

        /// <summary>
        /// Gets or sets the color used when displaying text in the control.
        /// </summary>
        [DefaultValue(typeof(Color), "ControlText")]
        public new Color ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = value;
                if (!IsHintTextVisible) base.ForeColor = value;
            }
        }

        /// <inheritdoc/>
        public override string Text
        {
            get => IsHintTextVisible ? "" : base.Text;
            set
            {
                // Update the visibility of the hint text when the text is changed externally
                if (string.IsNullOrEmpty(value) && !Focused) ShowHintText();
                else HideHintText();

                // Inform the underlying TextBox
                base.Text = value;
            }
        }

        private string _hintText = "";

        /// <summary>
        /// A text to be displayed in <see cref="SystemColors.GrayText"/> when <see cref="TextBox.Text"/> is empty.
        /// </summary>
        [Description("A text to be displayed in gray when Text is empty."), Category("Appearance"), Localizable(true)]
        [DefaultValue("")]
        public string HintText
        {
            get => _hintText;
            set
            {
                _hintText = value;

                // Update the hint text on-screen if it is already visible
                if (IsHintTextVisible) ShowHintText();
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="HintText"/> is currently visible.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHintTextVisible { get; private set; }

        private bool _showClearButton;

        /// <summary>
        /// Controls whether the clear button is shown. Remains invisible when the <see cref="HintText"/> is visible.
        /// </summary>
        [DefaultValue(false), Description("Controls whether the clear button is shown. Remains invisible when the HintText is visible."), Category("Appearance")]
        public bool ShowClearButton
        {
            get => _showClearButton;
            set
            {
                _showClearButton = value;

                // Show clear button only if it is enabled and there is text that can be cleared
                _buttonClear.Visible = value && !string.IsNullOrEmpty(base.Text) && !IsHintTextVisible;
            }
        }
        #endregion

        #region Constructor
        public HintTextBox()
        {
            ShowHintText();

            _buttonClear.Click += delegate
            {
                Focus();

                // Only clear the text if focus change was possible (might be prevented by validation)
                if (Focused)
                {
                    Clear();
                    OnClearButtonClicked(EventArgs.Empty);
                }
            };
            Controls.Add(_buttonClear);
        }
        #endregion

        //--------------------//

        #region Hint text helpers
        private void ShowHintText()
        {
            IsHintTextVisible = true;

            // Show the hint text without raising events
            _suppressTextChangedEvent = true;
            base.Text = _hintText;
            _suppressTextChangedEvent = false;
            base.ForeColor = SystemColors.GrayText;
        }

        private void HideHintText()
        {
            // Don't try to clear the TextBox if isn't displaying the hint text - it might contain real data
            if (!IsHintTextVisible) return;

            IsHintTextVisible = false;

            // Remove the hint text without raising events
            _suppressTextChangedEvent = true;
            base.Text = "";
            _suppressTextChangedEvent = false;
            base.ForeColor = _foreColor;
        }
        #endregion
    }
}
