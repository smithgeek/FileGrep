using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Smithgeek.Extensions;

namespace Smithgeek.IO
{
    /// <summary>
    /// PInvoke wrapper for CopyEx
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa363852.aspx
    /// http://stackoverflow.com/questions/187768/can-i-show-file-copy-progress-using-fileinfo-copyto-in-net
    /// </summary>
    internal class XCopy
    {
        public static void Copy(string source, string destination, bool overwrite, bool nobuffering, ref int cancel)
        {
            new XCopy().CopyInternal(source, destination, overwrite, nobuffering, null, ref cancel);
        }

        public static void Copy(string source, string destination, bool overwrite, bool nobuffering, Action<ProcessProgress> handler, ref int cancel)
        {
            new XCopy().CopyInternal(source, destination, overwrite, nobuffering, handler, ref cancel);
        }

        private event EventHandler mCompleted;

        private int mIsCancelled;
        private int mFilePercentCompleted;
        private string mSource;
        private string mDestination;
        private ProcessProgress mProgress;
        private bool mReportProgress;

        private XCopy()
        {
            mReportProgress = false;
            mIsCancelled = 0;
        }

        private void CopyInternal(string source, string destination, bool overwrite, bool nobuffering, Action<ProcessProgress> handler)
        {
            CopyInternal(source, destination, overwrite, nobuffering, handler, ref mIsCancelled);
        }

        private void CopyInternal(string source, string destination, bool overwrite, bool nobuffering, Action<ProcessProgress> handler, ref int cancel)
        {
            try
            {
                mReportProgress = !handler.isNull();
                if (mReportProgress)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(source);
                    mProgress = new ProcessProgress((int)info.Length, handler);
                    mProgress.Text = destination;
                }

                CopyFileFlags copyFileFlags = CopyFileFlags.COPY_FILE_RESTARTABLE;
                if (!overwrite)
                {
                    copyFileFlags |= CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;
                }

                OperatingSystem os = Environment.OSVersion;
                if (nobuffering && os.Platform == PlatformID.Win32NT && os.Version.Major >= 6)
                    copyFileFlags |= CopyFileFlags.COPY_FILE_NO_BUFFERING;

                mSource = source;
                mDestination = destination;

                bool result = CopyFileEx(mSource, mDestination, new CopyProgressRoutine(CopyProgressHandler), IntPtr.Zero, ref cancel, copyFileFlags);
                if (!result)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            catch (Exception ex)
            {
                // If the user canceled the copy we throw an exception, how nice is that.  Just ignore it.
                if (ex.Message != "The request was aborted")
                {
                    throw ex;
                }
            }
            if (mReportProgress)
            {
                if (cancel == 1)
                {
                    mProgress.Canceled = true;
                }
                if (!mProgress.Canceled)
                {
                    mProgress.Completed = true;
                }
            }
        }

        private void OnProgressChanged(double transferred)
        {
            // only raise an event when progress has changed
            if (transferred > mFilePercentCompleted)
            {
                mFilePercentCompleted = (int)transferred;

                if (mReportProgress)
                {
                    mProgress.Processed = (long)transferred;
                }
            }
        }

        private void OnCompleted()
        {
            var handler = mCompleted;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #region PInvoke

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel, CopyFileFlags dwCopyFlags);

        private delegate CopyProgressResult CopyProgressRoutine(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, uint dwStreamNumber, CopyProgressCallbackReason dwCallbackReason,
                                                        IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

        private enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        private enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        [Flags]
        private enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_NO_BUFFERING = 0x00001000,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
        }

        private CopyProgressResult CopyProgressHandler(long total, long transferred, long streamSize, long streamByteTrans, uint dwStreamNumber,
                                                       CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
        {
            if (reason == CopyProgressCallbackReason.CALLBACK_CHUNK_FINISHED)
                OnProgressChanged(transferred);

            if (transferred >= total)
                OnCompleted();

            return CopyProgressResult.PROGRESS_CONTINUE;
        }

        #endregion

    }
}
