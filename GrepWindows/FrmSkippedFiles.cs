using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepWindows
{
    public partial class FrmSkippedFiles : Form
    {
        public FrmSkippedFiles(List<String> strings)
        {
            InitializeComponent();
            StringBuilder builder = new StringBuilder();
            strings.ForEach(s => builder.AppendLine(s));
            uTextBox.Text = builder.ToString();
            uTextBox.Select(0, 0);
        }

        private void uTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                uTextBox.SelectAll();
            }
        }
    }
}
