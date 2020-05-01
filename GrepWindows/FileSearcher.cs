using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GrepWindows;
using Smithgeek.IO;


namespace Grep
{
    public class FileSearcher
    {
        /// <summary>
        /// Delegate for sending search result events.
        /// </summary>
        /// <param name="results">Search results</param>
        public delegate void SearchResults(String results);

        /// <summary>
        /// Event to send search results.
        /// </summary>
        public event SearchResults HandleResults;

        /// <summary>
        /// List of pending results that need to be sent.
        /// </summary>
        private List<SearchResultList> mPendingResults;

        /// <summary>
        /// Copy of the search parameters this searcher will use.
        /// </summary>
        SearchParameters mSearchParams;

        /// <summary>
        /// The regular expression used to search files.
        /// </summary>
        Regex mRegex;

        /// <summary>
        /// Regular expression used to find newline characters.
        /// </summary>
        private static Regex sNewlineRegex = null;

        /// <summary>
        /// The statistics for the current search.
        /// </summary>
        public SearchStats Stats { get; set; }

        /// <summary>
        /// Timer that runs to periodically send results back up the chain.
        /// </summary>
        private System.Timers.Timer mTimer;
        
        /// <summary>
        /// The thread the periodic timer runs on.
        /// </summary>
        private BackgroundWorker mTimerThread;
        
        /// <summary>
        /// Creates a searcher to search files.
        /// </summary>
        /// <param name="aSearchParams">The parameters that the searcher should use.</param>
        public FileSearcher(SearchParameters aSearchParams)
        {
            if (sNewlineRegex == null)
            {
                sNewlineRegex = new Regex("\n", RegexOptions.Compiled);
            }
            mSearchParams = new SearchParameters(aSearchParams);
            Stats = new SearchStats(mSearchParams);
            mPendingResults = new List<SearchResultList>();
            mTimer = new System.Timers.Timer(200);
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            mTimerThread = new BackgroundWorker();
            mTimerThread.DoWork += new DoWorkEventHandler(mTimerThread_DoWork);
            mTimerThread.RunWorkerAsync();
        }

        /// <summary>
        /// Cancel the search.
        /// </summary>
        public void CancelSearch()
        {
            lock (Stats.CancellationTokenSource)
            {
                Stats.CancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// This function will run the timer thread to run periodic events.
        /// </summary>
        void mTimerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            mTimer.Start();
        }

        /// <summary>
        /// When the periodic event occurs send any pending results.
        /// </summary>
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendPendingResults(false);
        }

        /// <summary>
        /// Start the search.  Creates a background thread to perform the search.
        /// </summary>
        public void search()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Gets the regex options for things like compiled, ignore case, and multiline.
        /// </summary>
        /// <returns>The regex options that should be used.</returns>
        RegexOptions getRegexOptions()
        {
            RegexOptions options = RegexOptions.Compiled;
            if (mSearchParams.IgnoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }
            return options;
        }

        /// <summary>
        /// Runs the background thread that performs the search.
        /// </summary>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Stats.reset();
            mRegex = null;
            try
            {
                mRegex = new Regex(mSearchParams.RegexPattern, getRegexOptions());
            }
            catch(Exception ex)
            {
                SearchResultList results = new SearchResultList("Error invalid search pattern: " + ex.Message);
                mPendingResults.Add(results);
            }
            internalSearch();
        }

        /// <summary>
        /// Converts a string into an array of strings based on line breaks.
        /// </summary>
        /// <param name="text">The text to convert to an array of strings.</param>
        /// <returns>An array of strings representing a line of text.</returns>
        private String[] getLines(String text)
        {
            return text.Split(new char[] { '\n' });
        }

        /// <summary>
        /// Get all of the indexes into the file text where newline characters are located.
        /// </summary>
        /// <param name="text">The file text</param>
        /// <returns>Indexes where newline characters are located.</returns>
        private List<int> getLineBreakIndexes(String text)
        {
            List<int> indexes = new List<int>();
            indexes.Add(-1);
            MatchCollection matches = sNewlineRegex.Matches(text);
            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    indexes.Add(matches[i].Index);
                }
            }
            return indexes;
        }

        /// <summary>
        /// Gets text from a file.  If an error occurs the file is marked as skipped.
        /// </summary>
        /// <param name="file">The file to read text from</param>
        /// <returns>The file text as a string.</returns>
        private String getFileText(String file)
        {
            try
            {
                FileInfo info = new FileInfo(file);
                if (info.Length > 500 * 1024 * 1024)
                {
                    throw new Exception("Can't search files 500 MB or larger.");
                }
                return File.ReadAllText(file);
            }
            catch(Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                if (!ex.Message.Contains(file))
                {
                    builder.AppendFormat("{0}: ", file);
                }
                builder.Append(ex.Message);
                lock (Stats.SkippedFiles)
                {
                    Stats.SkippedFiles.Add(builder.ToString());
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Search a specific file for some text.
        /// </summary>
        /// <param name="file">The file to search.</param>
        private void searchFile(String file)
        {
            String fileText = getFileText(file);

            MatchCollection matches = mRegex.Matches(fileText);
            if (matches.Count > 0)
            {
                List<int> indexes = new List<int>();
                for(int i = 0; i < matches.Count; i++)
                {
                    indexes.Add(matches[i].Index);
                }

                SearchResultList results = new SearchResultList(file);
                results.Matches = matches;
                getResults(fileText, indexes, results);
                fileText = String.Empty;
                saveResults(results);
                Logger.get().AddInfoFormat("{0} matches found in {1}", results.Matches.Count, Path.GetFileName(file));
            }
            lock (Stats)
            {
                if (matches.Count > 0)
                {
                    Stats.NumberFound += matches.Count;
                    Stats.FilesWithFound++;
                }
                Stats.FilesSearched++;
            }
        }

        /// <summary>
        /// Gets the results from a file search.
        /// </summary>
        /// <param name="fileText">The text from a file that will be searched.</param>
        /// <param name="indexes">List of indexes into the file text where matches were found.</param>
        /// <param name="results">The results of the search operation.</param>
        private void getResults(String fileText, List<int> indexes, SearchResultList results)
        {
            List<int> lineBreakIndexes = getLineBreakIndexes(fileText);
            lineBreakIndexes.Insert(0, -1);
            int prevStartIndex = -2;
            for (int i = 0; i < indexes.Count; i++)
            {
                int listCOunt = indexes.Count;
                int index = indexes[i];
                int startIndex = fileText.LastIndexOf('\n', Math.Min(fileText.Length - 1, indexes[i]));
                if (prevStartIndex != startIndex)
                {
                    prevStartIndex = startIndex;
                    int endIndex = fileText.IndexOf('\n', indexes[i]);
                    if (endIndex == -1)
                    {
                        endIndex = fileText.Length - 1;
                    }
                    SearchResult result = new SearchResult();
                    result.MatchLine.LineNumber = lineBreakIndexes.IndexOf(startIndex);
                    getContext(result, fileText, lineBreakIndexes);
                    if (result.MatchLine.LineNumber == 0)
                    {
                        result.MatchLine.LineNumber = 1;
                    }
                    result.MatchLine.Text = fileText.Substring(startIndex + 1, endIndex - startIndex);
                    results.Results.Add(result);
                }
            }
        }

        /// <summary>
        /// Gets lines of context around the line that matches the search parameter
        /// </summary>
        /// <param name="result">The match result to add context lines to</param>
        /// <param name="fileText">The file being searched</param>
        /// <param name="lineBreakIndexes">List of indexes to line breaks in the file string</param>
        private void getContext(SearchResult result, String fileText, List<int> lineBreakIndexes)
        {
            if (result.MatchLine.LineNumber > 0)
            {
                int contextLines = 0;
                for (int i = result.MatchLine.LineNumber - 1; i >= 0 && contextLines < Settings.get().ContextLineCount; i--)
                {
                    contextLines++;
                    LineText contextLine = new LineText();
                    contextLine.LineNumber = i;
                    int startIndex = lineBreakIndexes[i] + 1;
                    int endIndex;
                    if (i == lineBreakIndexes.Count - 1)
                    {
                        endIndex = fileText.Length;
                    }
                    else
                    {
                        endIndex = lineBreakIndexes[i + 1];
                    }
                    if (startIndex < endIndex)
                    {
                        contextLine.Text = fileText.Substring(startIndex, endIndex - startIndex);
                        result.BeforeContext.Add(contextLine);
                    }
                }
            }

            if (result.MatchLine.LineNumber < lineBreakIndexes.Count - 1)
            {
                int contextLines = 0;
                for (int i = result.MatchLine.LineNumber + 1; i < lineBreakIndexes.Count && contextLines < Settings.get().ContextLineCount; i++)
                {
                    contextLines++;
                    LineText contextLine = new LineText();
                    contextLine.LineNumber = i;
                    int startIndex = lineBreakIndexes[i] + 1;
                    int endIndex;
                    if(i == lineBreakIndexes.Count-1)
                    {
                        endIndex = fileText.Length;
                    }
                    else
                    {
                        endIndex = lineBreakIndexes[i + 1];
                    }
                    if (startIndex < endIndex)
                    {
                        contextLine.Text = fileText.Substring(startIndex, endIndex - startIndex);
                        result.AfterContext.Add(contextLine);
                    }
                }
            }
        }

        /// <summary>
        /// Save the results to the pending result list so that they can be later sent.
        /// </summary>
        /// <param name="results">The results that should be saved.</param>
        private void saveResults(SearchResultList results)
        {
            if (HandleResults != null)
            {
                lock (mPendingResults)
                {
                    mPendingResults.Add(results);
                }
                lock (Stats)
                {
                    Stats.TotalResultsPending++;
                }
            }
        }

        /// <summary>
        /// Sends the pending results to anyone listening to the event
        /// </summary>
        /// <param name="searchFinished">Has the searched finished.</param>
        void sendPendingResults(bool searchFinished)
        {
            if (HandleResults != null && mSearchParams != null && (!mSearchParams.SortByFile || searchFinished))
            {
                StringBuilder builder = new StringBuilder();
                List<SearchResultList> readyResults = null;
                lock (mPendingResults)
                {
                    if (mPendingResults.Count > 0)
                    {
                        if (mSearchParams.SortByFile)
                        {
                            readyResults = mPendingResults.OrderBy(resultList => resultList.FileName).ToList();
                        }
                        else
                        {
                            readyResults = mPendingResults;
                        }
                        Logger.get().AddDataFormat(mPendingResults, "PendingResults", "Pending results sent for {0} files.", mPendingResults.Count);
                        mPendingResults = new List<SearchResultList>();
                    }
                }
                if (readyResults != null && readyResults.Count > 0)
                {
                    readyResults.ForEach(result =>
                    {
                        String rtf = RtfUtility.createRtf(result, mSearchParams.FileNameSearch, mRegex);
                        builder.AppendLine(rtf);
                    });
                    if (HandleResults != null)
                    {
                        HandleResults(builder.ToString());
                    }
                    lock (Stats)
                    {
                        Stats.ResultsSent += readyResults.Count;
                    }
                    readyResults.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the line numbers for each result in the index list based on the file text.
        /// It is marked as unsafe so that some checking is disabled making this function faster.
        /// That means you shouldn't do anything stupid here because really bad things might happen.
        /// </summary>
        /// <param name="fileText">The text of the file to use.</param>
        /// <param name="indexes">List of indexes where matches have been found.</param>
        /// <returns>List of line numbers where each index can be found in that file.</returns>
        private unsafe List<int> getLineNumbers(String fileText, int[] indexes)
        {
            List<int> lineNumbers = new List<int>();
            int line = 0;
            int currentIndex = 0;
            for (int i = 0; i <= indexes[indexes.Length - 1]; ++i)
            {
                if (fileText[i] == '\n')
                {
                    line++;
                }
                else if (i == indexes[currentIndex])
                {
                    lineNumbers.Add(line);
                    currentIndex++;
                }
            }
            return lineNumbers;
        }

        /// <summary>
        /// This is the function that actually does the searching.  It creates threads and searches and then
        /// waits for all the results to be sent.
        /// </summary>
        public void internalSearch()
        {
            String searchDirectory = mSearchParams.SearchDirectory;

            if (mRegex != null)
            {
                List<String> fileList = DirectoryCache.get().getFileList();

                if (fileList != null)
                {
                    Stats.TotalFilesToSearch = fileList.Count;
                    Logger.get().AddInfoFormat("{0} files to search", Stats.TotalFilesToSearch);
                    try
                    {
                        // Set up the parallel lambda expression.  The parameters were trial and error trying to get the fastest execution
                        fileList.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).WithDegreeOfParallelism(63)
                            .WithCancellation(Stats.CancellationTokenSource.Token)
                            .WithMergeOptions(ParallelMergeOptions.FullyBuffered).ForAll(file =>
                            {
                                // Check for cancellation, this speeds up the cancel after the button is clicked.
                                if (!Stats.CancellationTokenSource.IsCancellationRequested)
                                {
                                    if (mSearchParams.FileNameSearch)
                                    {
                                        if (mRegex.IsMatch(file))
                                        {
                                            SearchResultList list = new SearchResultList(PathExt.GetRelativePath(searchDirectory, file));
                                            saveResults(list);
                                        }
                                    }
                                    else
                                    {
                                        searchFile(file);
                                    }
                                }
                            });
                    }
                    catch (OperationCanceledException)
                    {
                        // Cancelling is not an error, why do they throw an exception?
                    }
                    catch (Exception ex)
                    {
                        Logger.get().AddError(ex.Message);
                    }
                }
                else
                {
                    SearchResultList results = new SearchResultList(DirectoryCache.get().ErrorMessage);
                    mPendingResults.Add(results);
                }
            }
            waitForResultsToBeSent();
        }

        /// <summary>
        /// Wait for all the results to be sent back to the main thread before exiting.
        /// </summary>
        private void waitForResultsToBeSent()
        {
            //Send the results
            sendPendingResults(true);
            bool allResultsSent = false;

            // Check if we think all the results have been sent.
            allResultsSent = Stats.TotalResultsPending <= Stats.ResultsSent;
            int counter = 0;

            // Wait a while for to allow a chance for results to be sent. We really shouldn't have to, but just in case.
            bool canceled = false;
            while (!allResultsSent && counter < 50)
            {
                sendPendingResults(true);
                lock (Stats)
                {
                    allResultsSent = Stats.TotalResultsPending <= Stats.ResultsSent;
                }
                if (!allResultsSent)
                {
                    Thread.Sleep(200);
                }
                counter++;
                if (Stats.CancellationTokenSource.IsCancellationRequested)
                {
                    sendPendingResults(true);
                    canceled = true;
                    break;
                }
            }
            lock (Stats)
            {
                Stats.Finished = true;
                Logger.get().AddInfo(canceled ? "Search canceled" : "Search complete.");
                if (counter >= 50)
                {
                    Logger.get().AddError("Did we actually send all the results?");
                }
                mPendingResults.Clear();
            }
        }

    }

    public static class StringExtensions
    {
        public static IEnumerable<int> AllIndexesOf(this String str, String value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException("the string to find may not be empty", "value");
            }
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                {
                    break;
                }
                yield return index;
            }
        }

        public static IEnumerable<int> AllIndexesOf(this String str, char value)
        {
            for (int index = 0; ; index++)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                {
                    break;
                }
                yield return index;
            }
        }
    }
}
