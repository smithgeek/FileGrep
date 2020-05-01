using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace GrepWindows
{
    /// <summary>
    /// Statistics for the current search process.
    /// </summary>
    public class SearchStats : BasePropertyChanged
    {
        /// <summary>
        /// Number of results found.
        /// </summary>
        public int NumberFound 
        {
            get { return mNumberFound; }
            set { SetField(ref mNumberFound, value, "NumberFound"); }
        }
        private int mNumberFound;

        /// <summary>
        /// Number of files that have at least 1 match.
        /// </summary>
        public int FilesWithFound 
        {
            get { return mFilesWithFound; }
            set { SetField(ref mFilesWithFound, value, "FilesWithFound"); }
        }
        private int mFilesWithFound;

        /// <summary>
        /// The total number of files that have already been searched.
        /// </summary>
        public int FilesSearched 
        {
            get { return mFilesSearched; }
            set { SetField(ref mFilesSearched, value, "FilesSearched"); }
        }
        private int mFilesSearched;

        /// <summary>
        /// The total number of files that will be searched.
        /// </summary>
        public int TotalFilesToSearch 
        {
            get { return mTotalFilesToSearch; }
            set { SetField(ref mTotalFilesToSearch, value, "TotalFilesToSearch"); }
        }
        private int mTotalFilesToSearch;

        /// <summary>
        /// Are we done searching?
        /// </summary>
        public bool Finished 
        {
            get { return mFinished; }
            set 
            { 
                SetField(ref mFinished, value, "Finished");
                if (mFinished)
                {
                    mTimer.Stop();
                }
            }
        }
        private bool mFinished;

        /// <summary>
        /// The number of files skipped, usually do to access restrictions.
        /// </summary>
        public List<String> SkippedFiles 
        {
            get { return mSkippedFiles; }
            set { SetField(ref mSkippedFiles, value, "SkippedFiles"); }
        }
        private List<String> mSkippedFiles;

        /// <summary>
        /// The number of results that need to be written to the textbox
        /// </summary>
        public int TotalResultsPending 
        {
            get { return mTotalResultsPending; }
            set { SetField(ref mTotalResultsPending, value, "TotalResultsPending"); }
        }
        private int mTotalResultsPending;

        /// <summary>
        /// The number of results that have been sent to the UI thread.
        /// </summary>
        public int ResultsSent 
        {
            get { return mResultsSent; }
            set { SetField(ref mResultsSent, value, "ResultsSent"); }
        }
        private int mResultsSent;

        /// <summary>
        /// Handles cancelling the search with LINQ.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource
        {
            get { return mCts; }
            set { SetField(ref mCts, value, "CancellationTokenSource"); }
        }
        private CancellationTokenSource mCts;

        public String ElapsedTime
        {
            get { return (mTimer.ElapsedMilliseconds / 1000.0).ToString("F2"); }
        }
        private Stopwatch mTimer;

        /// <summary>
        /// Creates status text that is suitable for humans to read and reasonably understand.
        /// </summary>
        public String StatusText
        {
            get 
            {
                String fileSearchStatus = String.Empty;
                if (CancellationTokenSource.IsCancellationRequested)
                    fileSearchStatus = Finished ? "Search cancelled. " : "Cancelling search ";
                else if (TotalFilesToSearch == 0 && !Finished)
                    fileSearchStatus = "Obtaining file list.";
                else if (!mSearchParams.FileNameSearch)
                    fileSearchStatus = "Searched " + FilesSearched + "/" +TotalFilesToSearch + " files.";
                String endString;
                if (mSearchParams.FileNameSearch)
                    endString = ResultsSent + " files found.";
                else
                    endString = "  Found " + NumberFound + " matches in " + FilesWithFound + " files.";
                String result = (Finished ? "Finished: " : "In Progress: ") + fileSearchStatus + endString;
                return result; 
            }
        }

        private SearchParameters mSearchParams;

        /// <summary>
        /// Create the statistics class and init to defaults.
        /// </summary>
        public SearchStats(SearchParameters aSearchParams)
        {
            mSearchParams = aSearchParams;
            mTimer = new Stopwatch();
            reset();
        }

        /// <summary>
        /// Resets the statistics to default values.
        /// </summary>
        public void reset()
        {
            CancellationTokenSource = new CancellationTokenSource();
            NumberFound = 0;
            FilesWithFound = 0;
            FilesSearched = 0;
            TotalFilesToSearch = 0;
            Finished = false;
            TotalResultsPending = 0;
            ResultsSent = 0;
            SkippedFiles = new List<string>();
            mTimer.Reset();
            mTimer.Start();
        }
    }
}
