namespace NanoByte.Common.Controls
{
    partial class TaskControl
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

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelOperation = new System.Windows.Forms.Label();
            this.progressBar = new NanoByte.Common.Controls.TaskProgressBar();
            this.progressLabel = new NanoByte.Common.Controls.TaskLabel();
            this.SuspendLayout();
            // 
            // labelOperation
            // 
            this.labelOperation.AutoEllipsis = true;
            this.labelOperation.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelOperation.Location = new System.Drawing.Point(0, 0);
            this.labelOperation.Name = "labelOperation";
            this.labelOperation.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.labelOperation.Size = new System.Drawing.Size(200, 17);
            this.labelOperation.TabIndex = 0;
            this.labelOperation.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.labelOperation.UseMnemonic = false;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar.Location = new System.Drawing.Point(0, 17);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 18);
            this.progressBar.TabIndex = 1;
            // 
            // progressLabel
            // 
            this.progressLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressLabel.Location = new System.Drawing.Point(0, 35);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.progressLabel.Size = new System.Drawing.Size(200, 17);
            this.progressLabel.TabIndex = 2;
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labelOperation);
            this.Name = "TaskControl";
            this.Size = new System.Drawing.Size(200, 54);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelOperation;
        private Common.Controls.TaskProgressBar progressBar;
        private Common.Controls.TaskLabel progressLabel;
    }
}
