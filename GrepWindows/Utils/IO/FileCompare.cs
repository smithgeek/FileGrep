using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using Smithgeek.Extensions;

namespace Smithgeek.IO
{
    public class ComparisonResult
    {
        public enum DifferenceType
        {
            SourceMissing,
            DestMissing,
            NotEqual,
            Equal,
            Unknown
        }

        public String SourcePath { get; set; }
        public class Destination
        {
            public String Path;
            public DifferenceType Difference;
            public bool Checked;

            public Destination(String aPath, DifferenceType aDifference)
            {
                Path = aPath;
                Difference = aDifference;
                Checked = true;
            }
        }

        public List<Destination> Destinations;

        /// <summary>
        /// True if the items compared were files, false if it was a directory
        /// </summary>
        public bool IsFile { get; set; }

        public ComparisonResult(String aSourcePath, String aDestPath, DifferenceType aType, bool aIsFile)
        {
            SourcePath = aSourcePath;
            Destinations = new List<Destination>();
            Destinations.Add(new Destination(aDestPath, aType));
            IsFile = aIsFile;
        }

        public ComparisonResult(String aSourcePath, List<Destination> aDestinations, bool aIsFile)
        {
            SourcePath = aSourcePath;
            Destinations = aDestinations;
            IsFile = aIsFile;
        }
    }

    public class FileCompare
    {
        public delegate void FileCompareCompleted(ComparisonResult Difference);
        public event FileCompareCompleted HandleFileCompared;

        public delegate void ProgressChanged(ProcessProgress Progress);
        public event ProgressChanged HandleDirectoryCompareProgress;

        public event ProgressChanged HandleFileCompareProgress;

        public const UInt32 FullBinaryCompare = UInt32.MaxValue;
         
        //private ProcessProgress mCurrentDirProgress;
        //private ProcessProgress mCurrentProcessProgress;

        /// <summary>
        /// Keeps track if a cancellation has been requested.
        /// </summary>
        private bool mCancelRequested;

        /// <summary>
        /// If a file is larger than this size in bytes, the file will be compared only by file size.  
        /// Use FullBinaryCompare if you don't want fast file comparisons.
        /// </summary>
        public UInt32 FastFileCompareSize { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FileCompare()
        {
            mCancelRequested = false;
        }

        /// <summary>
        /// Requests that the comparison be canceled.
        /// </summary>
        public void Cancel()
        {
            mCancelRequested = true;
        }

        /// <summary>
        /// Sends an event when a file comparison has finished
        /// </summary>
        /// <param name="aDifference">Details of the comparison result</param>
        private void sendEvent(ComparisonResult aDifference)
        {
            if (HandleFileCompared != null)
            {
                HandleFileCompared(aDifference);
            }
        }

        private class DestinationInfo
        {
            public String Path { get; set; }
            public bool Done { get; set; }
            public List<String> FileList { get; set; }
            public List<String> DirList { get; set; }
            public ComparisonResult.Destination Result { get; set; }
            public System.IO.BufferedStream BufferStream { get; set; }
            public byte[] Buffer { get; set; }


            public DestinationInfo(String aPath)
            {
                Path = aPath;
                Done = false;
                FileList = new List<string>();
                DirList = new List<string>();
                BufferStream = null;
                Buffer = null;
            }
        }

        /// <summary>
        /// Compare two directories
        /// </summary>
        /// <param name="sourceDir">The source directory to compare</param>
        /// <param name="compareDir">The destination directory to compare</param>
        /// <param name="searchOption">If subdirectories should also be compared</param>
        /// <returns></returns>
        public List<ComparisonResult> CompareDirs(String sourceDir, String compareDir, DirectorySearchOption searchOption)
        {
            List<String> list = new List<string>();
            list.Add(compareDir);
            return CompareDirs(sourceDir, list, searchOption);
        }

        /// <summary>
        /// Compare one source directory with multiple destination directories
        /// </summary>
        /// <param name="sourceDir">The source directory to compare</param>
        /// <param name="compareDir">The destination directories to compare</param>
        /// <param name="searchOption">If subdirectories should also be compared</param>
        /// <returns></returns>
        public List<ComparisonResult> CompareDirs(String sourceDir, IEnumerable<String> compareDirs, DirectorySearchOption searchOption)
        {
            List<DestinationInfo> directories = new List<DestinationInfo>();
            IEnumerator<String> comparisonDirectory = compareDirs.GetEnumerator();
            while (comparisonDirectory.MoveNext())
            {
                directories.Add(new DestinationInfo(comparisonDirectory.Current));
            }

            bool bailOut = false;
            List<ComparisonResult> results = new List<ComparisonResult>();
            if (!Directory.Exists(sourceDir))
            {
                ComparisonResult result = new ComparisonResult(sourceDir, sourceDir, ComparisonResult.DifferenceType.SourceMissing, false);
                results.Add(result);
                sendEvent(result);
                bailOut = true;
            }
            List<ComparisonResult.Destination> missingDestinationDirectoires = new List<ComparisonResult.Destination>();
            foreach (DestinationInfo dir in directories.Where(d => !d.Done))
            {
                if (!Directory.Exists(dir.Path))
                {
                    missingDestinationDirectoires.Add(new ComparisonResult.Destination(dir.Path, ComparisonResult.DifferenceType.DestMissing));
                    dir.Done = true;
                }
            }
            if (missingDestinationDirectoires.Count > 0)
            {
                ComparisonResult compareResult = new ComparisonResult(sourceDir, missingDestinationDirectoires, false);
                sendEvent(compareResult);
                results.Add(compareResult);
            }

            if (bailOut || directories.All(d => d.Done))
            {
                return results;
            }

            // First compare all the files
            List<String> sourceFiles = DirectoryExt.GetFiles(sourceDir, searchOption);
            List<String> sourceDirs = DirectoryExt.GetDirectories(sourceDir, searchOption).ToList();
            ProcessProgress progress = null;
            if (!HandleDirectoryCompareProgress.isNull())
            {
                progress = new ProcessProgress(sourceFiles.Count + sourceDirs.Count, (prog) => HandleDirectoryCompareProgress(prog));
            }
            else
            {
                progress = new ProcessProgress(sourceFiles.Count + sourceDirs.Count);
            }

            foreach (DestinationInfo dir in directories.Where(d => !d.Done))
            {
                dir.FileList = DirectoryExt.GetFiles(dir.Path, searchOption).ToList();
            }


            for (int i = 0; i < sourceFiles.Count; ++i)
            {
                if (mCancelRequested)
                {
                    break;
                }
                List<ComparisonResult.Destination> destinationResults = new List<ComparisonResult.Destination>();
                String sourceFileName = sourceFiles[i];
                List<String> filePaths = new List<string>();
                foreach (DestinationInfo dir in directories.Where(d => !d.Done))
                {
                    String destFileName = PathExt.Combine(dir.Path, PathExt.GetRelativePath(sourceDir, sourceFileName));
                    if (dir.FileList.Contains(destFileName))
                    {
                        filePaths.Add(destFileName);
                        dir.FileList.Remove(destFileName);
                    }
                    else
                    {
                        destinationResults.Add(new ComparisonResult.Destination(destFileName, ComparisonResult.DifferenceType.DestMissing));
                    }
                }
                progress.Processed++;
                ComparisonResult compareResult = CompareFiles(sourceFileName, filePaths);
                compareResult.Destinations.AddRange(destinationResults);
                if (compareResult.Destinations.Count > 0)
                {
                    sendEvent(compareResult);
                    results.Add(compareResult);
                }
            }

            foreach (DestinationInfo dir in directories.Where(d => !d.Done))
            {
                foreach (String file in dir.FileList)
                {
                    if (mCancelRequested)
                    {
                        break;
                    }
                    ComparisonResult compareResult = new ComparisonResult(PathExt.Combine(sourceDir, PathExt.GetRelativePath(dir.Path, file)), file, ComparisonResult.DifferenceType.SourceMissing, true);
                    sendEvent(compareResult);
                    results.Add(compareResult);
                }
            }

            //Next compare the directories
            foreach (DestinationInfo dir in directories.Where(d => !d.Done))
            {
                if (mCancelRequested)
                {
                    break;
                }
                dir.DirList = DirectoryExt.GetDirectories(dir.Path, searchOption).ToList();
            }

            for (int i = 0; i < sourceDirs.Count; ++i)
            {
                foreach (DestinationInfo dir in directories.Where(d => !d.Done))
                {
                    if (mCancelRequested)
                    {
                        break;
                    }
                    String sourceDirName = sourceDirs[i];
                    String destDirName = PathExt.Combine(dir.Path, PathExt.GetRelativePath(sourceDir, sourceDirName));
                    if (dir.DirList.Contains(destDirName))
                    {
                        dir.DirList.Remove(destDirName);
                    }
                    else
                    {
                        // Only send a difference if the directory is empty, otherwise the file comparison will cover this case.
                        if (DirectoryExt.Empty(sourceDirName))
                        {
                            ComparisonResult compareResult = new ComparisonResult(sourceDirName, destDirName, ComparisonResult.DifferenceType.DestMissing, false);
                            sendEvent(compareResult);
                            results.Add(compareResult);
                        }
                    }
                }
                progress.Processed++;
            }

            foreach (DestinationInfo directory in directories.Where(d => !d.Done))
            {
                foreach (String dir in directory.DirList)
                {
                    if (mCancelRequested)
                    {
                        break;
                    }
                    // Only send a difference if the directory is empty, otherwise the file comparison will cover this case.
                    if (DirectoryExt.Empty(dir))
                    {
                        ComparisonResult compareResult = new ComparisonResult(PathExt.Combine(sourceDir, PathExt.GetRelativePath(directory.Path, dir)), dir, ComparisonResult.DifferenceType.SourceMissing, false);
                        sendEvent(compareResult);
                        results.Add(compareResult);
                    }
                }
            }

            if (mCancelRequested)
            {
                progress.Canceled = true;
            }
            progress.Completed = true;
            return results;
        }

        /// <summary>
        /// Determines if a fast file comparison should be performed
        /// </summary>
        /// <param name="aPath">Path to the file in question.</param>
        /// <returns>True if we should perform a fast file comparison, false if we need a full comparison.</returns>
        private bool useFastCompareForFile(String aPath)
        {
            bool fastCompare;
            if (FullBinaryCompare == FastFileCompareSize)
            {
                fastCompare = false;
            }
            else if (0 == FastFileCompareSize)
            {
                fastCompare = true;
            }
            else
            {
                System.IO.FileInfo info = new System.IO.FileInfo(aPath);
                fastCompare = info.Length > FastFileCompareSize;
            }
            return fastCompare;
        }

        /// <summary>
        /// Compare two files
        /// </summary>
        /// <param name="file1">First file to compare</param>
        /// <param name="file2">Second file to compare</param>
        /// <returns>True if files are equal</returns>
        public bool CompareFiles(String file1, String file2)
        {
            List<String> list = new List<string>();
            list.Add(file2);
            ComparisonResult comparison = CompareFiles(file1, list);
            return (comparison == null || comparison.Destinations[0].Difference == ComparisonResult.DifferenceType.Equal);
        }

        /// <summary>
        /// Compare a source file with multiple destination files
        /// </summary>
        /// <param name="sourceFile">Source file path</param>
        /// <param name="comparisonFiles">List of destination files.</param>
        /// <returns>The result of the comparison</returns>
        public ComparisonResult CompareFiles(String sourceFile, List<String> comparisonFiles)
        {
            List<DestinationInfo> files = new List<DestinationInfo>();
            comparisonFiles.ForEach(file => files.Add(new DestinationInfo(file)));

            if (!File.Exists(sourceFile))
            {
                files.ForEach(file =>
                {
                    file.Done = true;
                    file.Result = new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.SourceMissing);
                });
                return new ComparisonResult(sourceFile, files.Select(file => file.Result).ToList(), true);
            }

            List<ComparisonResult.Destination> results = new List<ComparisonResult.Destination>();
            // get file length and make sure lengths are identical
            long length = new System.IO.FileInfo(sourceFile).Length;

            foreach (DestinationInfo file in files.Where(file => !file.Done))
            {
                if (sourceFile == file.Path)
                {
                    file.Done = true;
                    file.Result = new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.Equal);
                }
                else if (!File.Exists(file.Path))
                {
                    file.Done = true;
                    file.Result = new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.DestMissing);
                }
                else if (length != new System.IO.FileInfo(file.Path).Length)
                {
                    file.Done = true;
                    file.Result = new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.NotEqual);
                }
                else if (useFastCompareForFile(file.Path))
                {
                    file.Done = true;
                    file.Result = new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.Equal);
                }
                if (file.Done)
                {
                    results.Add(file.Result);
                }
            }

            if (!files.All(f => f.Done))
            {
                System.IO.BufferedStream sourceStream = null;
                try
                {
                    sourceStream = new System.IO.BufferedStream(System.IO.File.OpenRead(sourceFile));
                    foreach (DestinationInfo file in files.Where(f => !f.Done))
                    {
                        file.BufferStream = new System.IO.BufferedStream(System.IO.File.OpenRead(file.Path));
                    }
                    results.AddRange(CompareStreams(sourceStream, files.Where(f => !f.Done)));
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                finally
                {
                    if (sourceStream != null)
                    {
                        sourceStream.Close();
                    }
                    foreach (DestinationInfo file in files.Where(f => f.Buffer != null))
                    {
                        file.BufferStream.Close();
                    }
                    foreach (DestinationInfo file in files.Where(f => !f.Done))
                    {
                        file.Done = true;
                        results.Add(new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.Equal));
                    }

                }
            }
            return new ComparisonResult(sourceFile, results, true);
        }

        /// <summary>
        /// Compares a source stream with multiple other files
        /// </summary>
        /// <param name="stream1">File stream for the source file</param>
        /// <param name="aFiles">List of files to compare</param>
        /// <returns>List of the results.</returns>
        private IEnumerable<ComparisonResult.Destination> CompareStreams(System.IO.Stream aSourceStream, IEnumerable<DestinationInfo> aFiles)
        {
            ProcessProgress progress = null;
            if (!HandleFileCompareProgress.isNull())
            {
                progress = new ProcessProgress((int)aSourceStream.Length, (prog) => HandleFileCompareProgress(prog));
            }
            else
            {
                progress = new ProcessProgress((int)aSourceStream.Length);
            }
            List<ComparisonResult.Destination> results = new List<ComparisonResult.Destination>();
            const int bufferSize = 1024 * 1024 * 1;// 100;
            var buffer1 = new byte[bufferSize];

            List<DestinationInfo> files = new List<DestinationInfo>();
            foreach (DestinationInfo file in aFiles)
            {
                files.Add(file);
                file.Buffer = new byte[bufferSize];
            }
            progress.Text = files[0].Path;

            uint hitCount = 0;
            bool done = false;
            while (!done && files.Count > 0)
            {
                if (mCancelRequested)
                {
                    progress.Canceled = true;
                    break;
                }
                int count1 = aSourceStream.Read(buffer1, 0, bufferSize);
                foreach (DestinationInfo file in files)
                {
                    int count2 = file.BufferStream.Read(file.Buffer, 0, bufferSize);

                    if (count1 != count2)
                    {
                        file.Result = new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.NotEqual);
                        results.Add(file.Result);
                        file.Done = true;
                        files.Remove(file);
                    }
                }

                if (count1 == 0)
                {
                    done = true;
                }
                else
                {
                    foreach (DestinationInfo file in files)
                    {
                        hitCount++;
                        if (!UnsafeCompare(buffer1, file.Buffer))
                        {
                            file.Result = new ComparisonResult.Destination(file.Path, ComparisonResult.DifferenceType.NotEqual);
                            results.Add(file.Result);
                            file.Done = true;
                            files.Remove(file);
                        }
                    }
                }
                progress.Processed = aSourceStream.Position;
            }
            progress.Completed = true;
            return results;
        }

        /// <summary>
        /// Compare two byte arrays really fast.
        /// </summary>
        /// <param name="a1">First array</param>
        /// <param name="a2">Second array</param>
        /// <returns>If the arrays are equal.</returns>
        private unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if ((l & 1) != 0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }
    }
}
