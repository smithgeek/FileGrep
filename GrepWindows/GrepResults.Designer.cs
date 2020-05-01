using Smithgeek.Windows;
namespace GrepWindows
{
    partial class GrepResults
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
            this.uSkipped = new System.Windows.Forms.LinkLabel();
            this.uStatus = new System.Windows.Forms.Label();
            this.uTime = new System.Windows.Forms.Label();
            this.uPanelFind = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.uTxtReplaceString = new System.Windows.Forms.TextBox();
            this.uBtnReplace = new System.Windows.Forms.Button();
            this.uChFindWholeWord = new System.Windows.Forms.CheckBox();
            this.uChFindMatchCase = new System.Windows.Forms.CheckBox();
            this.uChFindSearchUp = new System.Windows.Forms.CheckBox();
            this.uBtnFindNext = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.uTxtFindString = new System.Windows.Forms.TextBox();
            this.uBtnCloseFind = new System.Windows.Forms.Button();
            this.TextBox = new Smithgeek.Windows.RichEditor();
            this.uPanelFind.SuspendLayout();
            this.SuspendLayout();
            // 
            // uSkipped
            // 
            this.uSkipped.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uSkipped.AutoSize = true;
            this.uSkipped.Location = new System.Drawing.Point(642, 276);
            this.uSkipped.Name = "uSkipped";
            this.uSkipped.Size = new System.Drawing.Size(70, 13);
            this.uSkipped.TabIndex = 20;
            this.uSkipped.TabStop = true;
            this.uSkipped.Text = "Files Skipped";
            this.uSkipped.Visible = false;
            this.uSkipped.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uSkipped_LinkClicked);
            // 
            // uStatus
            // 
            this.uStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uStatus.Location = new System.Drawing.Point(3, 271);
            this.uStatus.Name = "uStatus";
            this.uStatus.Size = new System.Drawing.Size(633, 23);
            this.uStatus.TabIndex = 19;
            this.uStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uTime
            // 
            this.uTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uTime.Location = new System.Drawing.Point(718, 271);
            this.uTime.Name = "uTime";
            this.uTime.Size = new System.Drawing.Size(93, 23);
            this.uTime.TabIndex = 21;
            this.uTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uPanelFind
            // 
            this.uPanelFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uPanelFind.BackColor = System.Drawing.Color.LightBlue;
            this.uPanelFind.Controls.Add(this.label2);
            this.uPanelFind.Controls.Add(this.uTxtReplaceString);
            this.uPanelFind.Controls.Add(this.uBtnReplace);
            this.uPanelFind.Controls.Add(this.uChFindWholeWord);
            this.uPanelFind.Controls.Add(this.uChFindMatchCase);
            this.uPanelFind.Controls.Add(this.uChFindSearchUp);
            this.uPanelFind.Controls.Add(this.uBtnFindNext);
            this.uPanelFind.Controls.Add(this.label1);
            this.uPanelFind.Controls.Add(this.uTxtFindString);
            this.uPanelFind.Controls.Add(this.uBtnCloseFind);
            this.uPanelFind.Location = new System.Drawing.Point(3, 207);
            this.uPanelFind.Name = "uPanelFind";
            this.uPanelFind.Size = new System.Drawing.Size(811, 61);
            this.uPanelFind.TabIndex = 23;
            this.uPanelFind.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Replace:";
            // 
            // uTxtReplaceString
            // 
            this.uTxtReplaceString.Location = new System.Drawing.Point(97, 34);
            this.uTxtReplaceString.Name = "uTxtReplaceString";
            this.uTxtReplaceString.Size = new System.Drawing.Size(145, 20);
            this.uTxtReplaceString.TabIndex = 3;
            // 
            // uBtnReplace
            // 
            this.uBtnReplace.Location = new System.Drawing.Point(248, 32);
            this.uBtnReplace.Name = "uBtnReplace";
            this.uBtnReplace.Size = new System.Drawing.Size(75, 23);
            this.uBtnReplace.TabIndex = 5;
            this.uBtnReplace.Text = "Replace";
            this.uBtnReplace.UseVisualStyleBackColor = true;
            this.uBtnReplace.Click += new System.EventHandler(this.uBtnReplace_Click);
            // 
            // uChFindWholeWord
            // 
            this.uChFindWholeWord.AutoSize = true;
            this.uChFindWholeWord.Location = new System.Drawing.Point(329, 36);
            this.uChFindWholeWord.Name = "uChFindWholeWord";
            this.uChFindWholeWord.Size = new System.Drawing.Size(86, 17);
            this.uChFindWholeWord.TabIndex = 7;
            this.uChFindWholeWord.Text = "Whole Word";
            this.uChFindWholeWord.UseVisualStyleBackColor = true;
            // 
            // uChFindMatchCase
            // 
            this.uChFindMatchCase.AutoSize = true;
            this.uChFindMatchCase.Location = new System.Drawing.Point(329, 5);
            this.uChFindMatchCase.Name = "uChFindMatchCase";
            this.uChFindMatchCase.Size = new System.Drawing.Size(83, 17);
            this.uChFindMatchCase.TabIndex = 6;
            this.uChFindMatchCase.Text = "Match Case";
            this.uChFindMatchCase.UseVisualStyleBackColor = true;
            // 
            // uChFindSearchUp
            // 
            this.uChFindSearchUp.AutoSize = true;
            this.uChFindSearchUp.Location = new System.Drawing.Point(418, 5);
            this.uChFindSearchUp.Name = "uChFindSearchUp";
            this.uChFindSearchUp.Size = new System.Drawing.Size(77, 17);
            this.uChFindSearchUp.TabIndex = 8;
            this.uChFindSearchUp.Text = "Search Up";
            this.uChFindSearchUp.UseVisualStyleBackColor = true;
            // 
            // uBtnFindNext
            // 
            this.uBtnFindNext.Location = new System.Drawing.Point(249, 1);
            this.uBtnFindNext.Name = "uBtnFindNext";
            this.uBtnFindNext.Size = new System.Drawing.Size(74, 23);
            this.uBtnFindNext.TabIndex = 4;
            this.uBtnFindNext.Text = "Next";
            this.uBtnFindNext.UseVisualStyleBackColor = true;
            this.uBtnFindNext.Click += new System.EventHandler(this.uBtnFindNext_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Find:";
            // 
            // uTxtFindString
            // 
            this.uTxtFindString.Location = new System.Drawing.Point(97, 3);
            this.uTxtFindString.Name = "uTxtFindString";
            this.uTxtFindString.Size = new System.Drawing.Size(146, 20);
            this.uTxtFindString.TabIndex = 2;
            this.uTxtFindString.KeyUp += new System.Windows.Forms.KeyEventHandler(this.uTxtFindString_KeyUp);
            this.uTxtFindString.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.uTxtFindString_KeyPress);
            // 
            // uBtnCloseFind
            // 
            this.uBtnCloseFind.FlatAppearance.BorderSize = 0;
            this.uBtnCloseFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uBtnCloseFind.Location = new System.Drawing.Point(5, 4);
            this.uBtnCloseFind.Margin = new System.Windows.Forms.Padding(0);
            this.uBtnCloseFind.Name = "uBtnCloseFind";
            this.uBtnCloseFind.Size = new System.Drawing.Size(20, 20);
            this.uBtnCloseFind.TabIndex = 1;
            this.uBtnCloseFind.Text = "X";
            this.uBtnCloseFind.UseVisualStyleBackColor = true;
            this.uBtnCloseFind.Click += new System.EventHandler(this.uBtnCloseFind_Click);
            // 
            // TextBox
            // 
            this.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox.BackColor = System.Drawing.Color.White;
            this.TextBox.Font = new System.Drawing.Font("Courier New", 5F);
            this.TextBox.HideSelection = false;
            this.TextBox.Location = new System.Drawing.Point(3, 3);
            this.TextBox.Name = "TextBox";
            this.TextBox.ReadOnly = true;
            this.TextBox.Size = new System.Drawing.Size(808, 265);
            this.TextBox.TabIndex = 10;
            this.TextBox.Text = "";
            this.TextBox.WordWrap = false;
            this.TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.TextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBox_MouseDoubleClick_1);
            this.TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            // 
            // GrepResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uPanelFind);
            this.Controls.Add(this.uTime);
            this.Controls.Add(this.uSkipped);
            this.Controls.Add(this.uStatus);
            this.Controls.Add(this.TextBox);
            this.Name = "GrepResults";
            this.Size = new System.Drawing.Size(814, 294);
            this.uPanelFind.ResumeLayout(false);
            this.uPanelFind.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public RichEditor TextBox;
        private System.Windows.Forms.LinkLabel uSkipped;
        private System.Windows.Forms.Label uStatus;
        private System.Windows.Forms.Label uTime;
        private System.Windows.Forms.Panel uPanelFind;
        private System.Windows.Forms.Button uBtnCloseFind;
        private System.Windows.Forms.CheckBox uChFindMatchCase;
        private System.Windows.Forms.CheckBox uChFindSearchUp;
        private System.Windows.Forms.Button uBtnFindNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox uTxtFindString;
        private System.Windows.Forms.CheckBox uChFindWholeWord;
        private System.Windows.Forms.Button uBtnReplace;
        private System.Windows.Forms.TextBox uTxtReplaceString;
        private System.Windows.Forms.Label label2;
    }
}
