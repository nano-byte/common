namespace Common.WinForms.Sample
{
    partial class SampleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taskControl = new NanoByte.Common.Controls.TaskControl();
            this.resettablePropertyGrid = new NanoByte.Common.Controls.ResettablePropertyGrid();
            this.splitButton = new NanoByte.Common.Controls.DropDownButton();
            this.dropDownButton = new NanoByte.Common.Controls.DropDownButton();
            this.timeSpanControl = new NanoByte.Common.Controls.TimeSpanControl();
            this.localizableTextBox = new NanoByte.Common.Controls.LocalizableTextBox();
            this.uriTextBox = new NanoByte.Common.Controls.UriTextBox();
            this.hintTextBox = new NanoByte.Common.Controls.HintTextBox();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(234, 42);
            // 
            // menuItemToolStripMenuItem
            // 
            this.menuItemToolStripMenuItem.Name = "menuItemToolStripMenuItem";
            this.menuItemToolStripMenuItem.Size = new System.Drawing.Size(233, 38);
            this.menuItemToolStripMenuItem.Text = "Menu item";
            // 
            // taskControl
            // 
            this.taskControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.taskControl.Location = new System.Drawing.Point(15, 237);
            this.taskControl.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.taskControl.Name = "taskControl";
            this.taskControl.Size = new System.Drawing.Size(435, 96);
            this.taskControl.TabIndex = 7;
            // 
            // resettablePropertyGrid
            // 
            this.resettablePropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resettablePropertyGrid.Location = new System.Drawing.Point(15, 342);
            this.resettablePropertyGrid.Name = "resettablePropertyGrid";
            this.resettablePropertyGrid.Size = new System.Drawing.Size(435, 220);
            this.resettablePropertyGrid.TabIndex = 6;
            // 
            // splitButton
            // 
            this.splitButton.AutoSize = true;
            this.splitButton.ContextMenuStrip = this.contextMenuStrip;
            this.splitButton.DropDownMenuStrip = this.contextMenuStrip;
            this.splitButton.Location = new System.Drawing.Point(237, 193);
            this.splitButton.Name = "splitButton";
            this.splitButton.ShowSplit = true;
            this.splitButton.Size = new System.Drawing.Size(211, 35);
            this.splitButton.TabIndex = 5;
            this.splitButton.Text = "Split";
            this.splitButton.UseVisualStyleBackColor = true;
            // 
            // dropDownButton
            // 
            this.dropDownButton.AutoSize = true;
            this.dropDownButton.ContextMenuStrip = this.contextMenuStrip;
            this.dropDownButton.DropDownMenuStrip = this.contextMenuStrip;
            this.dropDownButton.Location = new System.Drawing.Point(12, 193);
            this.dropDownButton.Name = "dropDownButton";
            this.dropDownButton.Size = new System.Drawing.Size(211, 35);
            this.dropDownButton.TabIndex = 4;
            this.dropDownButton.Text = "Drop down";
            this.dropDownButton.UseVisualStyleBackColor = true;
            // 
            // timeSpanControl
            // 
            this.timeSpanControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeSpanControl.Location = new System.Drawing.Point(12, 130);
            this.timeSpanControl.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.timeSpanControl.Name = "timeSpanControl";
            this.timeSpanControl.Size = new System.Drawing.Size(446, 54);
            this.timeSpanControl.TabIndex = 3;
            this.timeSpanControl.Value = System.TimeSpan.Parse("1.03:02:04");
            // 
            // localizableTextBox
            // 
            this.localizableTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.localizableTextBox.AutoScroll = true;
            this.localizableTextBox.HintText = "Localizable text";
            this.localizableTextBox.Location = new System.Drawing.Point(12, 86);
            this.localizableTextBox.MinimumSize = new System.Drawing.Size(65, 22);
            this.localizableTextBox.Name = "localizableTextBox";
            this.localizableTextBox.Size = new System.Drawing.Size(435, 35);
            this.localizableTextBox.TabIndex = 2;
            // 
            // uriTextBox
            // 
            this.uriTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uriTextBox.ForeColor = System.Drawing.Color.Red;
            this.uriTextBox.HintText = "Please enter an URI.";
            this.uriTextBox.Location = new System.Drawing.Point(12, 49);
            this.uriTextBox.Name = "uriTextBox";
            this.uriTextBox.Size = new System.Drawing.Size(435, 31);
            this.uriTextBox.TabIndex = 1;
            // 
            // hintTextBox
            // 
            this.hintTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hintTextBox.HintText = "This is a hint.";
            this.hintTextBox.Location = new System.Drawing.Point(12, 12);
            this.hintTextBox.Name = "hintTextBox";
            this.hintTextBox.Size = new System.Drawing.Size(435, 31);
            this.hintTextBox.TabIndex = 0;
            // 
            // SampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 583);
            this.Controls.Add(this.taskControl);
            this.Controls.Add(this.resettablePropertyGrid);
            this.Controls.Add(this.splitButton);
            this.Controls.Add(this.dropDownButton);
            this.Controls.Add(this.timeSpanControl);
            this.Controls.Add(this.localizableTextBox);
            this.Controls.Add(this.uriTextBox);
            this.Controls.Add(this.hintTextBox);
            this.Name = "SampleForm";
            this.Text = "Sample form";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NanoByte.Common.Controls.HintTextBox hintTextBox;
        private NanoByte.Common.Controls.UriTextBox uriTextBox;
        private NanoByte.Common.Controls.TimeSpanControl timeSpanControl;
        private NanoByte.Common.Controls.TaskControl taskControl;
        private NanoByte.Common.Controls.ResettablePropertyGrid resettablePropertyGrid;
        private NanoByte.Common.Controls.DropDownButton splitButton;
        private NanoByte.Common.Controls.DropDownButton dropDownButton;
        private NanoByte.Common.Controls.LocalizableTextBox localizableTextBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemToolStripMenuItem;
    }
}

