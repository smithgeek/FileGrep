using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

public class CueTextBox : TextBox
{
    private string mCue;
    [Browsable(true)]
    public string Cue
    {
        get { return mCue; }
        set
        {
            mCue = value;
            updateCue();
        }
    }
    private void updateCue()
    {
        if (!this.IsHandleCreated || string.IsNullOrEmpty(mCue)) return;
        IntPtr mem = Marshal.StringToHGlobalUni(mCue);
        SendMessage(this.Handle, 0x1501, (IntPtr)1, mem);
        Marshal.FreeHGlobal(mem);
    }
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        updateCue();
    }
    // P/Invoke
    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
}