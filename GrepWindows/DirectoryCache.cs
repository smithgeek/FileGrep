using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Smithgeek.Text.RegularExpressions;
using Smithgeek.Extensions;

namespace GrepWindows
{
    public class DirectoryCache
    {
        /// <summary>
        /// Mutex to protect the file list
        /// </summary>
        private Mutex mMutex;

        /// <summary>
        /// The search parameters to use when caching the directory file list
        /// </summary>
        private SearchParameters mSearchParams;

        private String mCachedDir;

        /// <summary>
        /// The current file list cache that anyone else can use.
        /// </summary>
        private List<String> mFileList;

        /// <summary>
        /// The full file list before any exclusions or scope is applied
        /// </summary>
        private List<String> mFullFileList;

        /// <summary>
        /// The last time a full file list cache update was performed.
        /// </summary>
        private DateTime mLastUpdate;

        /// <summary>
        /// An error message that can be passed on if something bad happens.
        /// </summary>
        public String ErrorMessage { get; set; }

        /// <summary>
        /// The type of cache updates that can be performed.
        /// </summary>
        public enum UpdateType
        {
            Full,
            Fast,
            Automatic
        };

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static DirectoryCache sDirCache = null;
        private static Mutex sInstanceMutex = new Mutex(false);


        /// <summary>
        /// Gets the single instance of the directory cache
        /// </summary>
        /// <returns>The single directory cache instance.</returns>
        public static DirectoryCache get()
        {
            if (sDirCache == null)
            {
                sInstanceMutex.WaitOne();
                if (sDirCache == null)
                {
                    sDirCache = new DirectoryCache(Settings.get().SearchParams);
                }
                sInstanceMutex.ReleaseMutex();
            }
            return sDirCache;
        }

        public List<String> getFileList()
        {
            mMutex.WaitOne();
            bool shouldUpdate = mFileList == null || mFileList.Count == 0;
            if (!shouldUpdate)
            {
                shouldUpdate = mCachedDir != mSearchParams.SearchDirectory || !mFileList[0].StartsWith(mSearchParams.SearchDirectory);
            }
            if (shouldUpdate)
            {
                mFileList = new List<string>();
                update(UpdateType.Full, true);
            }
            else
            {
                update(UpdateType.Fast, true);
            }
            List<String> newList = new List<string>(mFileList);
            mMutex.ReleaseMutex();
            return newList;
        }

        /// <summary>
        /// Caches all files in the given directory and applies any exclusion or scope filters.
        /// </summary>
        /// <param name="directory">The directory to cache</param>
        /// <param name="scope">The scope to apply to the file list.</param>
        /// <param name="exclusions">The exclusions to apply to the file list.</param>
        /// <param name="searchSubdirectories">If subdirectories should be searched.</param>
        /// <param name="fileNameSearch">If this is a filename search instead of a search in file search</param>
        private DirectoryCache(SearchParameters aSearchParams)
        {
            mSearchParams = aSearchParams;
            mSearchParams.PropertyChanged += new PropertyChangedEventHandler(mSearchParams_PropertyChanged);
            mMutex = new Mutex(false, DateTime.Now.Ticks.ToString());
            mMutex.WaitOne();
            mFullFileList = new List<string>();
            update(UpdateType.Full, false);
            mMutex.ReleaseMutex();
        }

        /// <summary>
        /// Triggered when a search parameter is changed, this will update the file cache
        /// </summary>
        /// <param name="sender">The object that sent the signal.</param>
        /// <param name="e">The property changed event arguments</param>
        void mSearchParams_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Do fast and slow updates based on what property changed.
            switch (e.PropertyName)
            {
                case "Exclusions":
                case "Scope":
                case "FileNameSearch":
                case "SearchSubdirectories":
                    update(UpdateType.Fast, false);
                    break;

                case "SearchDirectory":
                    update(UpdateType.Full, false);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Converts the exclusions into regular expressions.
        /// </summary>
        /// <returns>List of regular expressions describing the list of exclusions.</returns>
        private List<Regex> getExclusionList()
        {
            List<Regex> exclusions = new List<Regex>();
            if (mSearchParams.Exclusions != null)
            {
                foreach (CheckBoxModel exclusion in mSearchParams.Exclusions)
                {
                    if (exclusion.Checked)
                    {
                        exclusions.Add(new Wildcard(exclusion.Text, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                    }
                }
            }
            return exclusions;
        }

        delegate void workFunction(object sender, DoWorkEventArgs e);
        /// <summary>
        /// Updates the directory cache
        /// </summary>
        /// <param name="force">Force a full update instead of a fast update</param>
        public void update(UpdateType updateMethod, bool wait)
        {
            mMutex.WaitOne();
            // If the update method is automatic just check if it's been a while since we last updated.  
            // We don't think anything has changed.
            BackgroundWorker worker = new BackgroundWorker();
            bool workToDo = true;
            workFunction func = null;
            if ( ( updateMethod == UpdateType.Full || mFullFileList == null || (DateTime.Now - mLastUpdate).TotalSeconds > 45) )
            {
                mLastUpdate = DateTime.Now;
                func = worker_DoWork;
            }
            else if (updateMethod == UpdateType.Fast)
            {
                func = worker_DoFastWork;
            }
            else
            {
                workToDo = false;
            }
            if (workToDo)
            {
                if (wait)
                {
                    DoWorkEventArgs args = new DoWorkEventArgs(mSearchParams.SearchDirectory);
                    func(this, args);
                    RunWorkerCompletedEventArgs result = new RunWorkerCompletedEventArgs(args.Result, null, false);
                    worker_RunWorkerCompleted(this, result);
                }
                else
                {
                    worker.DoWork += new DoWorkEventHandler(func);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                    worker.RunWorkerAsync(mSearchParams.SearchDirectory);
                }
            }
            else
            {
                mMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Creates regular expressions from the scope string
        /// </summary>
        /// <param name="scopeString">The scope string provided from the user</param>
        /// <param name="scopeRegex">OUT: The positive scope regex to apply</param>
        /// <param name="negativeScopeRegex">OUT: The negative scope regex to apply</param>
        private void getScopeRegex(String scopeString, out Regex scopeRegex, out Regex negativeScopeRegex)
        {
            scopeRegex = null;
            negativeScopeRegex = null;
            StringBuilder combinedRegexString = new StringBuilder();
            StringBuilder negativeRegexString = new StringBuilder();
            if (!String.IsNullOrEmpty(scopeString))
            {
                foreach (String scope in scopeString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        if (scope.Trim()[0] != '-')
                        {
                            Wildcard wildcard = new Wildcard(scope.Trim());
                            combinedRegexString.AppendFormat("({0})|", wildcard.ToString());
                        }
                        else
                        {
                            Wildcard wildcard = new Wildcard(scope.Trim().Substring(1).Trim());
                            negativeRegexString.AppendFormat("({0})|", wildcard.ToString().Substring(1).Trim());
                        }
                    }
                    catch { }
                }
                try
                {
                    if (combinedRegexString.Length > 0)
                    {
                        scopeRegex = new Regex(combinedRegexString.ToString(0, combinedRegexString.Length - 1), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    }
                    if (negativeRegexString.Length > 0)
                    {
                        negativeScopeRegex = new Regex(negativeRegexString.ToString(0, negativeRegexString.Length - 1), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets all files in the directory (and possible subdirectory) and then filters out and exclusions or scope restrictions.
        /// </summary>
        /// <param name="sender">The object that started this function</param>
        /// <param name="e">The background worker event argumes</param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ErrorMessage = String.Empty;
            String directory = (String)e.Argument;
            mCachedDir = directory;
            if (Directory.Exists(directory))
            {
                try
                {
                    mFullFileList = Directory.GetFiles(directory, "*", SearchOption.AllDirectories).ToList();
                }
                catch(Exception ex)
                {
                    ErrorMessage = String.Format("Error while searching {0}  -  {1}", directory, ex.Message);
                    Logger.get().AddErrorFormat(ErrorMessage);
                    e.Result = null;
                    return;
                }
                List<String> tempFileList = removeExcludedFilesTrial();
                //e.Result = tempFileList;
                mFileList = tempFileList;
                Logger.get().AddInfoFormat("Cached file list count {0}", mFullFileList.Count);
                Logger.get().AddDataFormat(mFullFileList, "List<String>", "{0} files in full cached list.", mFullFileList.Count);
            }
            else
            {
                e.Result = null;
                ErrorMessage = "Error: Directory does not exist.";
            }
        }

        /// <summary>
        /// Uses the list of files already obtained and just applies to the exclusions and scope parameters.
        /// </summary>
        /// <param name="sender">The object that started this function</param>
        /// <param name="e">The background worker event argumes</param>
        void worker_DoFastWork(object sender, DoWorkEventArgs e)
        {
            List<String> tempFileList = removeExcludedFilesTrial();
            //e.Result = tempFileList;
            mFileList = tempFileList;
            Logger.get().AddInfoFormat("Fast file list {0}", tempFileList.Count);
        }

        /// <summary>
        /// After the background thread finishes this updates the publicly available file list with new results
        /// </summary>
        /// <param name="sender">The object that started this function</param>
        /// <param name="e">The background worker event argumes</param>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Locking and unlocking cannot occur on the background thread because of synchronization issues!
            if (e.Result != null)
            {
                mFileList = (List<String>)e.Result;
            }
            mMutex.ReleaseMutex();
        }

        /// <summary>
        /// Removes excluded files from the list of directory files.
        /// This is a trial function that partitions the list so that multiple threads can be used.
        /// It seems to work faster for file name searches and about the same for other searches.
        /// </summary>
        /// <returns>The list containing only the files that have not been excluded.</returns>
        private List<String> removeExcludedFilesTrial()
        {
            List<String> tempFileList = new List<string>(mFullFileList);
            int numberOfPartitions = Math.Max(1, tempFileList.Count / 5);
            // First remove subdirectories
            List<String> finalList = new List<string>();
            if (!mSearchParams.SearchSubdirectories)
            {
                tempFileList.Partition(numberOfPartitions).AsParallel().ForAll(partition =>
                {
                    partition.RemoveAll(element => mSearchParams.SearchDirectory.endDir() != Path.GetDirectoryName(element).endDir());
                    lock (finalList)
                    {
                        finalList.AddRange(partition);
                    }
                });
                tempFileList = finalList;
            }

            // If we're doing a file name search skip the scope options
            numberOfPartitions = Math.Max(1, tempFileList.Count / 5);
            if (!mSearchParams.FileNameSearch)
            {
                Regex positiveScopeRegex;
                Regex negativeScopeRegex;
                getScopeRegex(mSearchParams.Scope, out positiveScopeRegex, out negativeScopeRegex);
                if (positiveScopeRegex != null || negativeScopeRegex != null)
                {
                    finalList = new List<string>();
                    tempFileList.Partition(numberOfPartitions).AsParallel().ForAll(partition =>
                    {
                        if (positiveScopeRegex != null)
                        {
                            partition.RemoveAll(element => !positiveScopeRegex.IsMatch(Path.GetFileName(element)));
                        }
                        if (negativeScopeRegex != null)
                        {
                            partition.RemoveAll(element => negativeScopeRegex.IsMatch(element));
                        }
                        lock (finalList)
                        {
                            finalList.AddRange(partition);
                        }
                    });
                    tempFileList = finalList;
                }
            }

            // Remove everything that matches an exclusion pattern
            numberOfPartitions = Math.Max(1, tempFileList.Count / 5);
            finalList = new List<string>();
            tempFileList.Partition(numberOfPartitions).AsParallel().ForAll(partition =>
            {
                foreach (Regex exclusion in getExclusionList())
                {
                    partition.RemoveAll(element => exclusion.IsMatch(element));
                }
                lock (finalList)
                {
                    finalList.AddRange(partition);
                }
            });
            tempFileList = finalList;
            Logger.get().AddInfo(String.Format("Found {0} files to search", tempFileList.Count));
            Logger.get().AddDataFormat(tempFileList, "fileList", "{0} files to search.", tempFileList.Count);
            return tempFileList;
        }

        /// <summary>
        /// Removes excluded files from the list of directory files
        /// </summary>
        /// <returns>The list containing only the files that have not been excluded.</returns>
        private List<String> removeExcludedFiles()
        {
            List<String> tempFileList = new List<string>(mFullFileList);
            DateTime start = DateTime.Now;
            // First remove subdirectories
            if (!mSearchParams.SearchSubdirectories)
            {
                tempFileList.RemoveAll(element => mSearchParams.SearchDirectory.endDir() != Path.GetDirectoryName(element).endDir());
            }

            // If we're doing a file name search skip the scope options
            if (!mSearchParams.FileNameSearch)
            {
                Regex positiveScopeRegex;
                Regex negativeScopeRegex;
                getScopeRegex(mSearchParams.Scope, out positiveScopeRegex, out negativeScopeRegex);
                if (positiveScopeRegex != null)
                {
                    tempFileList.RemoveAll(element => !positiveScopeRegex.IsMatch(Path.GetFileName(element)));
                }
                if (negativeScopeRegex != null)
                {
                    tempFileList.RemoveAll(element => negativeScopeRegex.IsMatch(Path.GetFileName(element)));
                }
            }

            foreach (Regex exclusion in getExclusionList())
            {
                tempFileList.RemoveAll(element => exclusion.IsMatch(element));
            }
            Logger.get().AddInfo(tempFileList.Count + " files to search");
            return tempFileList;
        }

        public Mutex getMutex()
        {
            return mMutex;
        }
    }

    public static class Partitoner
    {
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }
}
