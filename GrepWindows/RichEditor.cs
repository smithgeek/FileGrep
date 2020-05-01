using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Smithgeek.Windows
{
    // Override the normal rich text box so we can use some improvements.
    public class RichEditor : RichTextBox
    {
        // Import the kernel dll
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr LoadLibrary(string lpFileName);

        // This makes the rich text box much faster when using a large amount of text.  
        // Why doesn't .NET do this automatically?
        protected override CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
                {
                    cp.ClassName = "RICHEDIT50W";
                }
                return cp;
            }
        }

        //Give us copy using Ctrl+C !!!
        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                if (!String.IsNullOrEmpty(this.SelectedText))
                {
                    Clipboard.SetText(this.SelectedText);
                }
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.C))
            {
                return base.ProcessCmdKey(ref m, (Keys.Control | Keys.C));
            }
            else
            {
                return base.ProcessCmdKey(ref m, keyData);
            }
        }

    }
}
