namespace GrepWindows
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.uGrepResults = new GrepWindows.GrepResults();
            this.addTab = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regularExpressionTipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikiPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportIssueMakeSuggestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uUpdaterCheck = new System.Windows.Forms.Timer(this.components);
            this.uGrepControl = new GrepWindows.GrepControl();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.addTab);
            this.tabControl1.Location = new System.Drawing.Point(10, 275);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(996, 153);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.uGrepResults);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(988, 127);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Results";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // uGrepResults
            // 
            this.uGrepResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uGrepResults.Location = new System.Drawing.Point(0, 0);
            this.uGrepResults.Name = "uGrepResults";
            this.uGrepResults.SearchedDirectory = null;
            this.uGrepResults.Size = new System.Drawing.Size(988, 127);
            this.uGrepResults.TabIndex = 0;
            // 
            // addTab
            // 
            this.addTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addTab.Location = new System.Drawing.Point(4, 22);
            this.addTab.Name = "addTab";
            this.addTab.Padding = new System.Windows.Forms.Padding(3);
            this.addTab.Size = new System.Drawing.Size(988, 127);
            this.addTab.TabIndex = 1;
            this.addTab.Text = "    +   ";
            this.addTab.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1018, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.loadSettingsToolStripMenuItem,
            this.saveSettingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regularExpressionTipsToolStripMenuItem,
            this.wikiPageToolStripMenuItem,
            this.reportIssueMakeSuggestionToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // regularExpressionTipsToolStripMenuItem
            // 
            this.regularExpressionTipsToolStripMenuItem.Name = "regularExpressionTipsToolStripMenuItem";
            this.regularExpressionTipsToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.regularExpressionTipsToolStripMenuItem.Text = "Regular Expression Tips";
            this.regularExpressionTipsToolStripMenuItem.Click += new System.EventHandler(this.regularExpressionTipsToolStripMenuItem_Click);
            // 
            // wikiPageToolStripMenuItem
            // 
            this.wikiPageToolStripMenuItem.Name = "wikiPageToolStripMenuItem";
            this.wikiPageToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.wikiPageToolStripMenuItem.Text = "Wiki Page";
            this.wikiPageToolStripMenuItem.Click += new System.EventHandler(this.wikiPageToolStripMenuItem_Click);
            // 
            // reportIssueMakeSuggestionToolStripMenuItem
            // 
            this.reportIssueMakeSuggestionToolStripMenuItem.Name = "reportIssueMakeSuggestionToolStripMenuItem";
            this.reportIssueMakeSuggestionToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.reportIssueMakeSuggestionToolStripMenuItem.Text = "Report an issue or make a suggestion.";
            this.reportIssueMakeSuggestionToolStripMenuItem.Click += new System.EventHandler(this.reportIssueMakeSuggestionToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // uUpdaterCheck
            // 
            this.uUpdaterCheck.Enabled = true;
            this.uUpdaterCheck.Interval = 18000000;
            this.uUpdaterCheck.Tick += new System.EventHandler(this.uUpdaterCheck_Tick);
            // 
            // uGrepControl
            // 
            this.uGrepControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uGrepControl.Location = new System.Drawing.Point(10, 24);
            this.uGrepControl.Name = "uGrepControl";
            this.uGrepControl.Size = new System.Drawing.Size(992, 247);
            this.uGrepControl.TabIndex = 1;
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveSettingsToolStripMenuItem.Text = "Save Settings...";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.loadSettingsToolStripMenuItem.Text = "Load Settings...";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 440);
            this.Controls.Add(this.uGrepControl);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(593, 464);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Grep";
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage addTab;
        private GrepControl uGrepControl;
        private System.Windows.Forms.TabPage tabPage1;
        private GrepResults uGrepResults;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer uUpdaterCheck;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportIssueMakeSuggestionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wikiPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regularExpressionTipsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
    }
}

