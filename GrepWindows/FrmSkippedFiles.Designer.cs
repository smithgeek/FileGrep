namespace GrepWindows
{
    partial class FrmSkippedFiles
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
            this.uTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // uTextBox
            // 
            this.uTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uTextBox.Location = new System.Drawing.Point(0, 0);
            this.uTextBox.Multiline = true;
            this.uTextBox.Name = "uTextBox";
            this.uTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uTextBox.Size = new System.Drawing.Size(779, 262);
            this.uTextBox.TabIndex = 0;
            this.uTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.uTextBox_KeyUp);
            // 
            // FrmSkippedFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 262);
            this.Controls.Add(this.uTextBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSkippedFiles";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Skipped Files";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox uTextBox;
    }
}