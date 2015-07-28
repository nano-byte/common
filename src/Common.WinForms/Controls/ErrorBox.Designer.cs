namespace NanoByte.Common.Controls
{
    partial class ErrorBox
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.textLog = new System.Windows.Forms.RichTextBox();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelInnerMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(384, 216);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(88, 24);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoEllipsis = true;
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.Location = new System.Drawing.Point(3, 0);
            this.labelMessage.MinimumSize = new System.Drawing.Size(48, 48);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Padding = new System.Windows.Forms.Padding(52, 0, 0, 0);
            this.labelMessage.Size = new System.Drawing.Size(136, 48);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "(Message)";
            this.labelMessage.Click += new System.EventHandler(this.label_Click);
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textLog.Location = new System.Drawing.Point(3, 70);
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.Size = new System.Drawing.Size(454, 125);
            this.textLog.TabIndex = 2;
            this.textLog.Text = "(Log)";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Image = global::NanoByte.Common.Properties.ImageResources.Error;
            this.pictureBoxIcon.Location = new System.Drawing.Point(16, 9);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(48, 48);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxIcon.TabIndex = 5;
            this.pictureBoxIcon.TabStop = false;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelMessage, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textLog, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelInnerMessage, 0, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(460, 198);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // labelInnerMessage
            // 
            this.labelInnerMessage.AutoSize = true;
            this.labelInnerMessage.Location = new System.Drawing.Point(3, 48);
            this.labelInnerMessage.Name = "labelInnerMessage";
            this.labelInnerMessage.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.labelInnerMessage.Size = new System.Drawing.Size(82, 19);
            this.labelInnerMessage.TabIndex = 1;
            this.labelInnerMessage.Text = "(Inner message)";
            this.labelInnerMessage.Click += new System.EventHandler(this.label_Click);
            // 
            // ErrorBox
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(484, 252);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.buttonOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 160);
            this.Name = "ErrorBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error box";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.RichTextBox textLog;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelInnerMessage;
    }
}