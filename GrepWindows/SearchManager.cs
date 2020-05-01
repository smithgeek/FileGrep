using System;
using System.IO;
using System.Windows.Forms;
using Grep;

namespace GrepWindows
{
    // This class should handle creating a file searcher and hooking it up to a results window
    class SearchManager
    {
        /// <summary>
        /// The file searcher used to search files.
        /// </summary>
        private FileSearcher mFileSearcher;

        /// <summary>
        /// The memory stream where results are written before they are output to the textbox.
        /// </summary>
        private MemoryStream mMemoryStream;

        /// <summary>
        /// Stream writer used to write to mMemoryStream.
        /// </summary>
        private StreamWriter mStreamWriter;

        /// <summary>
        /// The initial length of the stream after the RTF file header is written.
        /// </summary>
        private long mInitialStreamLength;

        /// <summary>
        /// If results have been written to the memory stream but have not been displayed in the textbox.
        /// </summary>
        private bool mStreamDirty;

        /// <summary>
        /// The results control we should use to write out resutls.
        /// </summary>
        private GrepResults mResults;
        
        /// <summary>
        /// Timer used to throttle UI updates so we don't flood the UI and slow down our program.
        /// </summary>
        Timer mUiUpdateThrottle;

        /// <summary>
        /// Constructor sets up the UI thread and memory streams.
        /// </summary>
        public SearchManager()
        {
            mUiUpdateThrottle = new Timer();
            mUiUpdateThrottle.Enabled = false;
            mUiUpdateThrottle.Tick += new EventHandler(mUiUpdateThrottle_Tick);
            mUiUpdateThrottle.Interval = 50;
        }

        /// <summary>
        /// Initialize everything required before searching
        /// </summary>
        private void searchInit()
        {
            mMemoryStream = new MemoryStream();
            mStreamWriter = new StreamWriter(mMemoryStream);
            mStreamDirty = false;
        }

        /// <summary>
        /// Starts a search.  Gets the search parameters and creates a new file searcher.  Connects
        /// up with the results control.
        /// </summary>
        public void startSearch()
        {
            searchInit();
            mUiUpdateThrottle.Enabled = true;
            RtfUtility.WriteFileHeader(mStreamWriter);
            mStreamWriter.Flush();
            mInitialStreamLength = mMemoryStream.Position;

            if (Logger.get().LoggingEnabled)
            {
                Logger.get().AddData(new SearchParameters(Settings.get().SearchParams), "SearchParameters", "Search Parameters");
            }
            Settings.get().SearchParams.SearchInProgress = true;
            mResults = TabManager.get().getControlForNewResults();
            if (mFileSearcher != null)
            {
                mFileSearcher.HandleResults -= fileSearcher_HandleResults;
                mFileSearcher = null;
            }
            mFileSearcher = new Grep.FileSearcher(Settings.get().SearchParams);
            mFileSearcher.HandleResults += new Grep.FileSearcher.SearchResults(fileSearcher_HandleResults);
            mFileSearcher.search();
        }

        /// <summary>
        /// Handles when results are returned from the file searcher.  Writes the results
        /// out to a memory stream.
        /// </summary>
        /// <param name="results">The results from the file searcher.</param>
        void fileSearcher_HandleResults(String results)
        {
            bool firstUpdate = mMemoryStream.Length == mInitialStreamLength;
            using (MemoryStream memStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(memStream))
                {
                    Logger.get().AddData(results, "Results", "Rich text formatted results");
                    writer.Write(results);
                    writer.Flush();
                    lock (mMemoryStream)
                    {
                        mMemoryStream.Write(memStream.GetBuffer(), 0, (int)memStream.Length);
                        mStreamDirty = true;
                    }
                }
            }
            // If the timer was turned off we thought we were done updating the text box so update now!
            if (firstUpdate || !mUiUpdateThrottle.Enabled)
            {
                writeResultsToTextBox();
            }
        }

        /// <summary>
        /// Handles updating the status of the search and cleans up the memory streams when the
        /// search finishes.
        /// </summary>
        private void mUiUpdateThrottle_Tick(object sender, EventArgs e)
        {
            // Flooding the UI will result in slower performance!
            if (mFileSearcher != null)
            {
                lock (mFileSearcher.Stats)
                {
                    mResults.updateStatus(mFileSearcher.Stats);
                    if (mFileSearcher.Stats.Finished)
                    {
                        writeResultsToTextBox();
                        mUiUpdateThrottle.Enabled = false;
                        mStreamWriter.Dispose();
                        mStreamWriter.Close();
                        mMemoryStream.Dispose();
                        mMemoryStream.Close();
                        Settings.get().SearchParams.SearchInProgress = false;
                        DirectoryCache.get().update(DirectoryCache.UpdateType.Automatic, false);
                        mFileSearcher.HandleResults -= fileSearcher_HandleResults;
                        mFileSearcher = null;
                        GC.Collect();
                    }
                }
            }
        }

        /// <summary>
        /// Writes the results from the memory stream to the textbox.
        /// </summary>
        private void writeResultsToTextBox()
        {
            using (MemoryStream copiedStream = new MemoryStream())
            {
                bool streamDirty = false;
                lock (mMemoryStream)
                {
                    streamDirty = mStreamDirty;
                    if (streamDirty)
                    {
                        mStreamWriter.Flush();
                        mMemoryStream.Position = 0;
                        copiedStream.Write(mMemoryStream.ToArray(), 0, (int)mMemoryStream.Length);
                        mMemoryStream.Position = mMemoryStream.Length;
                        mStreamDirty = false;
                    }
                }
                if (streamDirty)
                {
                    mResults.Invoke(new MethodInvoker(() =>
                    {
                        lock (mResults.TextBox)
                        {
                            mResults.TextBox.SuspendPaint();
                            copiedStream.Flush();
                            copiedStream.Position = 0;
                            mResults.TextBox.LoadFile(copiedStream, RichTextBoxStreamType.RichText);
                            mResults.TextBox.ResumePaint();
                        }
                    }));
                }
            }
        }

        /// <summary>
        /// Cancels the search.
        /// </summary>
        public void cancelSearch()
        {
            if (mFileSearcher != null)
            {
                mFileSearcher.CancelSearch();
            }
        }
    }
}
