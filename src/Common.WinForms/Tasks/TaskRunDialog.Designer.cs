using NanoByte.Common.Controls;

namespace NanoByte.Common.Tasks
{
    partial class TaskRunDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_allowWindowClose", Justification = "Disposing WaitHandle is not necessary since the SafeWaitHandle is never touched")]
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
            this.progressBarTask = new NanoByte.Common.Controls.TaskProgressBar();
            this.labelTask = new NanoByte.Common.Controls.TaskLabel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBarTask
            // 
            this.progressBarTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarTask.Location = new System.Drawing.Point(12, 12);
            this.progressBarTask.Name = "progressBarTask";
            this.progressBarTask.Size = new System.Drawing.Size(270, 23);
            this.progressBarTask.TabIndex = 0;
            // 
            // labelTask
            // 
            this.labelTask.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTask.AutoSize = true;
            this.labelTask.Location = new System.Drawing.Point(12, 38);
            this.labelTask.Name = "labelTask";
            this.labelTask.Size = new System.Drawing.Size(0, 13);
            this.labelTask.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(207, 47);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // TaskRunDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(294, 82);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelTask);
            this.Controls.Add(this.progressBarTask);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(120, 120);
            this.Name = "TaskRunDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TaskRunDialog_FormClosing);
            this.Shown += new System.EventHandler(this.TaskRunDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TaskProgressBar progressBarTask;
        private Common.Controls.TaskLabel labelTask;
        private System.Windows.Forms.Button buttonCancel;
    }
}
