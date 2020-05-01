namespace GrepWindows
{
    partial class GrepControl
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.uPatternList = new System.Windows.Forms.ComboBox();
            this.uScopeList = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.uNumericContext = new System.Windows.Forms.NumericUpDown();
            this.uChFileFind = new System.Windows.Forms.CheckBox();
            this.uOrderFiles = new System.Windows.Forms.CheckBox();
            this.uWholeWord = new System.Windows.Forms.CheckBox();
            this.uRegex = new System.Windows.Forms.CheckBox();
            this.uSubdirectories = new System.Windows.Forms.CheckBox();
            this.uIgnoreCase = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.uQuickScopePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.uDirectoryList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.uBtnBrowse = new System.Windows.Forms.Button();
            this.uBtnSearch = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.uChListExclusions = new GrepWindows.CheckedListBoxBindable();
            this.uBtnRemoveExlcusion = new System.Windows.Forms.Button();
            this.uBtnAddExclusion = new System.Windows.Forms.Button();
            this.uTxtExclusions = new CueTextBox();
            this.uBtnCancel = new System.Windows.Forms.Button();
            this.uTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uNumericContext)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(220, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Pattern";
            // 
            // uPatternList
            // 
            this.uPatternList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uPatternList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.uPatternList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uPatternList.Location = new System.Drawing.Point(276, 3);
            this.uPatternList.Name = "uPatternList";
            this.uPatternList.Size = new System.Drawing.Size(674, 24);
            this.uPatternList.TabIndex = 1;
            this.uPatternList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GrepControl_KeyPress);
            this.uPatternList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            // 
            // uScopeList
            // 
            this.uScopeList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uScopeList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.uScopeList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.uScopeList.Location = new System.Drawing.Point(6, 21);
            this.uScopeList.Name = "uScopeList";
            this.uScopeList.Size = new System.Drawing.Size(716, 24);
            this.uScopeList.TabIndex = 0;
            this.uScopeList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GrepControl_KeyPress);
            this.uScopeList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            this.uScopeList.TextChanged += new System.EventHandler(this.uScopeList_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.uNumericContext);
            this.groupBox1.Controls.Add(this.uChFileFind);
            this.groupBox1.Controls.Add(this.uOrderFiles);
            this.groupBox1.Controls.Add(this.uWholeWord);
            this.groupBox1.Controls.Add(this.uRegex);
            this.groupBox1.Controls.Add(this.uSubdirectories);
            this.groupBox1.Controls.Add(this.uIgnoreCase);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(217, 125);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(733, 52);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(671, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Context";
            // 
            // uNumericContext
            // 
            this.uNumericContext.Location = new System.Drawing.Point(636, 19);
            this.uNumericContext.Name = "uNumericContext";
            this.uNumericContext.Size = new System.Drawing.Size(33, 22);
            this.uNumericContext.TabIndex = 6;
            // 
            // uChFileFind
            // 
            this.uChFileFind.AutoSize = true;
            this.uChFileFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uChFileFind.Location = new System.Drawing.Point(554, 20);
            this.uChFileFind.Name = "uChFileFind";
            this.uChFileFind.Size = new System.Drawing.Size(78, 20);
            this.uChFileFind.TabIndex = 5;
            this.uChFileFind.Text = "File Find";
            this.uTooltip.SetToolTip(this.uChFileFind, "Search for file names instead of text in a file.");
            this.uChFileFind.UseVisualStyleBackColor = true;
            // 
            // uOrderFiles
            // 
            this.uOrderFiles.AutoSize = true;
            this.uOrderFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uOrderFiles.Location = new System.Drawing.Point(412, 20);
            this.uOrderFiles.Name = "uOrderFiles";
            this.uOrderFiles.Size = new System.Drawing.Size(136, 20);
            this.uOrderFiles.TabIndex = 4;
            this.uOrderFiles.Text = "Sort Output By File";
            this.uTooltip.SetToolTip(this.uOrderFiles, "Sorts the results by filename, no results will be shown until the search is finis" +
                    "hed.");
            this.uOrderFiles.UseVisualStyleBackColor = true;
            // 
            // uWholeWord
            // 
            this.uWholeWord.AutoSize = true;
            this.uWholeWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uWholeWord.Location = new System.Drawing.Point(304, 20);
            this.uWholeWord.Name = "uWholeWord";
            this.uWholeWord.Size = new System.Drawing.Size(102, 20);
            this.uWholeWord.TabIndex = 3;
            this.uWholeWord.Text = "&Whole Word";
            this.uWholeWord.UseVisualStyleBackColor = true;
            // 
            // uRegex
            // 
            this.uRegex.AutoSize = true;
            this.uRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uRegex.Location = new System.Drawing.Point(111, 20);
            this.uRegex.Name = "uRegex";
            this.uRegex.Size = new System.Drawing.Size(67, 20);
            this.uRegex.TabIndex = 1;
            this.uRegex.Text = "&Regex";
            this.uTooltip.SetToolTip(this.uRegex, "Allows entering regular expressions into the pattern.  If this is not selected wi" +
                    "ldcards(* and ?) are accepted.");
            this.uRegex.UseVisualStyleBackColor = true;
            this.uRegex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            // 
            // uSubdirectories
            // 
            this.uSubdirectories.AutoSize = true;
            this.uSubdirectories.Checked = true;
            this.uSubdirectories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uSubdirectories.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uSubdirectories.Location = new System.Drawing.Point(184, 20);
            this.uSubdirectories.Name = "uSubdirectories";
            this.uSubdirectories.Size = new System.Drawing.Size(114, 20);
            this.uSubdirectories.TabIndex = 2;
            this.uSubdirectories.Text = "S&ubdirectories";
            this.uSubdirectories.UseVisualStyleBackColor = true;
            this.uSubdirectories.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            // 
            // uIgnoreCase
            // 
            this.uIgnoreCase.AutoSize = true;
            this.uIgnoreCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uIgnoreCase.Location = new System.Drawing.Point(7, 20);
            this.uIgnoreCase.Name = "uIgnoreCase";
            this.uIgnoreCase.Size = new System.Drawing.Size(98, 20);
            this.uIgnoreCase.TabIndex = 0;
            this.uIgnoreCase.Text = "&Ignore case";
            this.uIgnoreCase.UseVisualStyleBackColor = true;
            this.uIgnoreCase.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.uQuickScopePanel);
            this.groupBox2.Controls.Add(this.uScopeList);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(217, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(733, 89);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "S&cope(s) separated by semicolons";
            // 
            // uQuickScopePanel
            // 
            this.uQuickScopePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uQuickScopePanel.Location = new System.Drawing.Point(7, 48);
            this.uQuickScopePanel.Name = "uQuickScopePanel";
            this.uQuickScopePanel.Size = new System.Drawing.Size(716, 35);
            this.uQuickScopePanel.TabIndex = 12;
            // 
            // uDirectoryList
            // 
            this.uDirectoryList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uDirectoryList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.uDirectoryList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.uDirectoryList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uDirectoryList.FormattingEnabled = true;
            this.uDirectoryList.Location = new System.Drawing.Point(288, 183);
            this.uDirectoryList.Name = "uDirectoryList";
            this.uDirectoryList.Size = new System.Drawing.Size(581, 24);
            this.uDirectoryList.TabIndex = 5;
            this.uDirectoryList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GrepControl_KeyPress);
            this.uDirectoryList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(220, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Directory";
            // 
            // uBtnBrowse
            // 
            this.uBtnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uBtnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uBtnBrowse.Location = new System.Drawing.Point(875, 181);
            this.uBtnBrowse.Name = "uBtnBrowse";
            this.uBtnBrowse.Size = new System.Drawing.Size(75, 27);
            this.uBtnBrowse.TabIndex = 10;
            this.uBtnBrowse.TabStop = false;
            this.uBtnBrowse.Text = "&Browse...";
            this.uBtnBrowse.UseVisualStyleBackColor = true;
            this.uBtnBrowse.Click += new System.EventHandler(this.uBtnBrowse_Click);
            this.uBtnBrowse.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            // 
            // uBtnSearch
            // 
            this.uBtnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uBtnSearch.Location = new System.Drawing.Point(217, 213);
            this.uBtnSearch.Name = "uBtnSearch";
            this.uBtnSearch.Size = new System.Drawing.Size(138, 27);
            this.uBtnSearch.TabIndex = 6;
            this.uBtnSearch.Text = "&Search";
            this.uBtnSearch.UseVisualStyleBackColor = true;
            this.uBtnSearch.Click += new System.EventHandler(this.uBtnSearch_Click);
            this.uBtnSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.uChListExclusions);
            this.groupBox3.Controls.Add(this.uBtnRemoveExlcusion);
            this.groupBox3.Controls.Add(this.uBtnAddExclusion);
            this.groupBox3.Controls.Add(this.uTxtExclusions);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(202, 237);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Exclusions (Wildcards Supported)";
            // 
            // uChListExclusions
            // 
            this.uChListExclusions.BindingList = null;
            this.uChListExclusions.CheckOnCheckboxOnly = true;
            this.uChListExclusions.CheckOnClick = true;
            this.uChListExclusions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uChListExclusions.FormattingEnabled = true;
            this.uChListExclusions.Location = new System.Drawing.Point(7, 21);
            this.uChListExclusions.Name = "uChListExclusions";
            this.uChListExclusions.Size = new System.Drawing.Size(188, 164);
            this.uChListExclusions.TabIndex = 15;
            this.uChListExclusions.KeyUp += new System.Windows.Forms.KeyEventHandler(this.uChListExclusions_KeyUp);
            // 
            // uBtnRemoveExlcusion
            // 
            this.uBtnRemoveExlcusion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uBtnRemoveExlcusion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uBtnRemoveExlcusion.Location = new System.Drawing.Point(104, 187);
            this.uBtnRemoveExlcusion.Name = "uBtnRemoveExlcusion";
            this.uBtnRemoveExlcusion.Size = new System.Drawing.Size(91, 24);
            this.uBtnRemoveExlcusion.TabIndex = 14;
            this.uBtnRemoveExlcusion.Text = "Remove";
            this.uBtnRemoveExlcusion.UseVisualStyleBackColor = true;
            this.uBtnRemoveExlcusion.Click += new System.EventHandler(this.uBtnRemoveExlcusion_Click);
            // 
            // uBtnAddExclusion
            // 
            this.uBtnAddExclusion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uBtnAddExclusion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uBtnAddExclusion.Location = new System.Drawing.Point(7, 187);
            this.uBtnAddExclusion.Name = "uBtnAddExclusion";
            this.uBtnAddExclusion.Size = new System.Drawing.Size(91, 24);
            this.uBtnAddExclusion.TabIndex = 13;
            this.uBtnAddExclusion.Text = "Add";
            this.uBtnAddExclusion.UseVisualStyleBackColor = true;
            this.uBtnAddExclusion.Click += new System.EventHandler(this.uBtnAddExclusion_Click);
            // 
            // uTxtExclusions
            // 
            this.uTxtExclusions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uTxtExclusions.Cue = "Type exclusion format here";
            this.uTxtExclusions.Location = new System.Drawing.Point(6, 213);
            this.uTxtExclusions.Name = "uTxtExclusions";
            this.uTxtExclusions.Size = new System.Drawing.Size(189, 20);
            this.uTxtExclusions.TabIndex = 12;
            this.uTxtExclusions.KeyUp += new System.Windows.Forms.KeyEventHandler(this.uTxtExclusions_KeyUp);
            // 
            // uBtnCancel
            // 
            this.uBtnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uBtnCancel.Location = new System.Drawing.Point(361, 213);
            this.uBtnCancel.Name = "uBtnCancel";
            this.uBtnCancel.Size = new System.Drawing.Size(138, 27);
            this.uBtnCancel.TabIndex = 13;
            this.uBtnCancel.Text = "Cancel";
            this.uBtnCancel.UseVisualStyleBackColor = true;
            this.uBtnCancel.Click += new System.EventHandler(this.uBtnCancel_Click);
            // 
            // uTooltip
            // 
            this.uTooltip.AutoPopDelay = 10000;
            this.uTooltip.InitialDelay = 500;
            this.uTooltip.ReshowDelay = 100;
            this.uTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.uTooltip.ToolTipTitle = "Explanation";
            // 
            // GrepControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uBtnCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.uBtnSearch);
            this.Controls.Add(this.uBtnBrowse);
            this.Controls.Add(this.uDirectoryList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.uPatternList);
            this.Controls.Add(this.label1);
            this.Name = "GrepControl";
            this.Size = new System.Drawing.Size(953, 247);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrepControl_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uNumericContext)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox uPatternList;
        private System.Windows.Forms.ComboBox uScopeList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox uIgnoreCase;
        private System.Windows.Forms.CheckBox uSubdirectories;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox uDirectoryList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button uBtnBrowse;
        private System.Windows.Forms.CheckBox uRegex;
        private System.Windows.Forms.Button uBtnSearch;
        private System.Windows.Forms.FlowLayoutPanel uQuickScopePanel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button uBtnRemoveExlcusion;
        private System.Windows.Forms.Button uBtnAddExclusion;
        private CueTextBox uTxtExclusions;
        private System.Windows.Forms.Button uBtnCancel;
        private System.Windows.Forms.CheckBox uWholeWord;
        private System.Windows.Forms.CheckBox uOrderFiles;
        private CheckedListBoxBindable uChListExclusions;
        private System.Windows.Forms.CheckBox uChFileFind;
        private System.Windows.Forms.ToolTip uTooltip;
        private System.Windows.Forms.NumericUpDown uNumericContext;
        private System.Windows.Forms.Label label3;
    }
}
