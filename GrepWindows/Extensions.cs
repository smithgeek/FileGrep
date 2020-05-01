using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GrepWindows
{
    // This allows us to suspend window painting if we're adding a lot of text to the results window.
    // It should help the speed out some
    public static class Extensions
    {
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        static IntPtr eventMask = IntPtr.Zero;

        public static void SuspendPaint(this RichTextBox box)
        {
            // Stop redrawing:
            SendMessage(box.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
            // Stop sending of events:
            eventMask = SendMessage(box.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);
        }

        public static void ResumePaint(this RichTextBox box)
        {
            // turn on events
            SendMessage(box.Handle, EM_SETEVENTMASK, 0, eventMask);
            // turn on redrawing
            SendMessage(box.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            box.Invalidate();
        }
    }
}
