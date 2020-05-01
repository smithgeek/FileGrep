using System;
using System.Collections.Generic;
using Smithgeek.Extensions;
using System.IO;

namespace Smithgeek.IO
{
    /// <summary>
    /// Static helper methods when dealing with files.
    /// </summary>
    public static class FileExt
    {
        /// <summary>
        /// Creates a new unique filename based off of the given file name.
        /// </summary>
        /// <param name="aPath">The path to the file.</param>
        /// <returns>New unique file name.</returns>
        public static String MakeUnique(String aPath)
        {
            if (aPath.isEmpty())
                return String.Empty;
            String directory = Path.GetDirectoryName(aPath);
            String filename = Path.GetFileNameWithoutExtension(aPath) + "_1";
            String ext = Path.GetExtension(aPath);
            while (File.Exists(PathExt.Combine(directory, filename + ext)))
            {
                filename += "_1";
            }
            return PathExt.Combine(directory, filename + ext);
        }

        /// <summary>
        /// Copies a file from one directory to another.
        /// </summary>
        /// <param name="aSourceDir">The source directory.</param>
        /// <param name="aDestinationDir">The destination directory</param>
        /// <param name="aFilename">The name of the file</param>
        /// <param name="aNameConflictOption">How to handle conflicts.</param>
        public static void Copy(String aSourceDir, String aDestinationDir, String aFilename, NameConflictOption aNameConflictOption)
        {
            Copy(aSourceDir, aDestinationDir, aFilename, aNameConflictOption, null);
        }

        /// <summary>
        /// Copy a file from on
        /// </summary>
        /// <param name="aSourceDir">The source directory.</param>
        /// <param name="aDestinationDir">The destination directory</param>
        /// <param name="aFilename">The name of the file</param>
        /// <param name="aNameConflictOption">How to handle conflicts.</param>
        /// <param name="aHandler">Callback function to handle progress notifications</param>
        public static void Copy(String aSourceDirectory, String aDestinationDirectory, String aFilename, NameConflictOption aNameConflictOption, Action<ProcessProgress> aHandler)
        {
            int cancel = 0;
            Copy(aSourceDirectory, aDestinationDirectory, aFilename, aNameConflictOption, aHandler, ref cancel);
        }

        /// <summary>
        /// Copy a file from on
        /// </summary>
        /// <param name="aSourceDir">The source directory.</param>
        /// <param name="aDestinationDir">The destination directory</param>
        /// <param name="aFilename">The name of the file</param>
        /// <param name="aNameConflictOption">How to handle conflicts.</param>
        /// <param name="aHandler">Callback function to handle progress notifications</param>
        /// <param name="aCancel"></param>
        public static void Copy(String aSourceDirectory, String aDestinationDirectory, String aFilename, NameConflictOption aNameConflictOption, Action<ProcessProgress> aHandler, ref int aCancel)
        {
            Copy(PathExt.Combine(aSourceDirectory, aFilename), PathExt.Combine(aDestinationDirectory, aFilename), aNameConflictOption, aHandler, ref aCancel);
        }

        public static void Copy(String source, String destination, NameConflictOption aNameConflictOption)
        {
            Copy(source, destination, aNameConflictOption, null);
        }

        public static void Copy(String source, String destination, NameConflictOption aNameConflictOption, Action<ProcessProgress> handler)
        {
            int cancel = 0;
            Copy(source, destination, aNameConflictOption, handler, ref cancel);
        }
        public static void Copy(String source, String destination, NameConflictOption aNameConflictOption, Action<ProcessProgress> handler, ref int cancel)
        {
            if (!File.Exists(source))
            {
                throw new FileNotFoundException("The source file could not be found.");
            }
            if (File.Exists(destination))
            {
                switch (aNameConflictOption)
                {
                    case NameConflictOption.Cancel:
                    case NameConflictOption.Skip:
                        return;
                    case NameConflictOption.Overwrite:
                        Delete(destination);
                        break;
                    case NameConflictOption.RenameExisting:
                        FileExt.Move(destination, FileExt.MakeUnique(destination));
                        break;
                    case NameConflictOption.RenameNew:
                        destination = FileExt.MakeUnique(destination);
                        break;
                    default:
                        break;
                }
            }

            if (!Directory.Exists(Path.GetDirectoryName(destination)))
                DirectoryExt.Create(Path.GetDirectoryName(destination));

            FileInfo fileInfo = new FileInfo(source);
            bool noBuffer = fileInfo.Length < Constants.USE_BUFFER_FILE_SIZE_BYTES;
            XCopy.Copy(source, destination, aNameConflictOption == NameConflictOption.Overwrite, noBuffer, handler, ref cancel);
        }

        public static void Create(String aPath)
        {
            Create(aPath, NameConflictOption.Skip);
        }

        public static void Create(String path, NameConflictOption aNameConflictOption)
        {
            if (File.Exists(path))
            {
                switch (aNameConflictOption)
                {
                    case NameConflictOption.Cancel:
                    case NameConflictOption.Skip:
                        return;
                    case NameConflictOption.Overwrite:
                        FileExt.Delete(path);
                        break;
                    case NameConflictOption.RenameExisting:
                        FileExt.Move(path, FileExt.MakeUnique(path));
                        break;
                    case NameConflictOption.RenameNew:
                        throw new InvalidDataException("Can't rename a file we are trying to create");
                    default:
                        break;
                }
            }
            DirectoryExt.Create(Path.GetDirectoryName(path));
            FileStream stream = File.Create(path);
            stream.Close();
        }

        public static bool Delete(String file)
        {
            bool success = true;
            if (File.Exists(file))
            {
                success = false;
                int tries = 0;
                while (!success && tries < Constants.STANDARD_RETRY_ATTEMPTS)
                {
                    try
                    {
                        File.Delete(file);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        success = false;
                        tries++;
                        System.Threading.Thread.Sleep(Constants.STANDARD_RETRY_SLEEP);
                    }
                }
            }
            return success;
        }

        public static bool Delete(String[] files)
        {
            bool cancel = false;
            return Delete(files, null, ref cancel);
        }

        public static bool Delete(String[] files, Action<ProcessProgress> handler, ref bool cancel)
        {
            bool success = true;
            ProcessProgress progress = new ProcessProgress(files.Length, handler);
            try
            {
                foreach (String file in files)
                {
                    if (cancel)
                    {
                        progress.Canceled = true;
                        break;
                    }
                    progress.Text = file;
                    success = success && Delete(file);
                    progress.Processed++;
                }
                progress.Completed = true;
            }
            catch
            {
                progress.Canceled = true;
                success = false;
            }
            return success;
        }

        public static bool Delete(List<String> files)
        {
            bool cancel = false;
            return Delete(files, null, ref cancel);
        }

        public static bool Delete(List<String> files, Action<ProcessProgress> handler, ref bool cancel)
        {
            return Delete(files.ToArray(), handler, ref cancel);
        }

        public static bool ExistsWait(String path)
        {
            int tries = 0;
            bool exists = File.Exists(path);
            while (!exists && tries < Constants.STANDARD_RETRY_ATTEMPTS)
            {
                tries++;
                System.Threading.Thread.Sleep(Constants.STANDARD_RETRY_SLEEP);
                exists = File.Exists(path);
            }
            System.Threading.Thread.Sleep(Constants.STANDARD_RETRY_SLEEP);
            return exists;
        }

        /// <summary>
        /// Move file without overwriting.
        /// </summary>
        /// <param name="source">Source file</param>
        /// <param name="destination">Destination file</param>
        /// <returns></returns>
        public static bool Move(String source, String destination)
        {
            return Move(source, destination, false);
        }

        public static bool Move(String source, String destination, bool overwrite)
        {
            bool success = false;
            if (File.Exists(source))
            {
                if (overwrite)
                {
                    Delete(destination);
                }
                else
                {
                    while (File.Exists(destination))
                    {
                        destination = destination.Insert(destination.LastIndexOf("."), "_1");
                    }
                }
                int tries = 0;
                while (!success && tries < Constants.STANDARD_RETRY_ATTEMPTS)
                {
                    try
                    {
                        File.Move(source, destination);
                        success = true;
                    }
                    catch
                    {
                        success = false;
                        tries++;
                        System.Threading.Thread.Sleep(Constants.STANDARD_RETRY_SLEEP);
                    }
                }
            }
            return success;
        }

        public static void Write(String filename, String text, bool append)
        {
            //Clear readonly flag
            if (File.Exists(filename))
            {
                File.SetAttributes(filename, FileAttributes.Normal);
            }
            if (append)
            {
                File.AppendAllText(filename, text);
            }
            else
            {
                if (File.Exists(filename))
                {
                    Delete(filename);
                }
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    DirectoryExt.Create(Path.GetDirectoryName(filename));
                }
                File.WriteAllText(filename, text);
            }
        }


        private class DestinationInfo
        {
            public String Path { get; set; }
            public bool Done { get; set; }
            public List<String> FileList { get; set; }
            public List<String> DirList { get; set; }
            public ComparisonResult Result { get; set; }
            public BufferedStream BufferStream { get; set; }
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

        public static void Copy(String source, List<String> destinations, NameConflictOption aNameConflictOption)
        {
            Copy(source, destinations, aNameConflictOption, null);
        }

        public static void Copy(String source, List<String> destinations, NameConflictOption aNameConflictOption, Action<ProcessProgress> handler)
        {
            int cancel = 0;
            Copy(source, destinations, aNameConflictOption, handler, ref cancel);
        }

        public static void Copy(String source, List<String> destinations, NameConflictOption aNameConflictOption, Action<ProcessProgress> handler, ref int cancel)
        {
            if (destinations.isEmpty())
                throw new ArgumentNullException("You must specify a destinatino location(s)");
            List<DestinationInfo> files = new List<DestinationInfo>();
            foreach (String file in destinations)
            {
                if (File.Exists(file))
                {
                    switch (aNameConflictOption)
                    {
                        case NameConflictOption.Cancel:
                            return;
                        case NameConflictOption.Overwrite:
                            files.Add(new DestinationInfo(file));
                            break;
                        case NameConflictOption.RenameExisting:
                            File.Move(file, FileExt.MakeUnique(file));
                            break;
                        case NameConflictOption.RenameNew:
                            files.Add(new DestinationInfo(FileExt.MakeUnique(file)));
                            break;
                        case NameConflictOption.Skip:
                            continue;
                        default:
                            break;
                    }
                }
                else
                {
                    files.Add(new DestinationInfo(file));
                }
            }

            if(files.Count == 0)
            {
                return;
            }
            // If we are only copying to one location don't use the multi copy
            else if (files.Count == 1)
            {
                Copy(source, files[0].Path, aNameConflictOption, handler, ref cancel);
            }
            else
            {
                CopyFileToMultipleDestinations(source, files, handler, ref cancel);
            }
        }

        private static void CopyFileToMultipleDestinations(String source, IEnumerable<DestinationInfo> destinations, Action<ProcessProgress> handler, ref int cancel)
        {
            BufferedStream sourceStream = null;
            ProcessProgress progress = null;
            Exception exception = null;
            byte[] sourceBuffer = new byte[Constants.COPY_BUFFER_SIZE];

            try
            {
                sourceStream = new BufferedStream(File.OpenRead(source));
                progress = new ProcessProgress((int)sourceStream.Length, handler);
                foreach (DestinationInfo file in destinations)
                {
                    FileExt.Create(file.Path, NameConflictOption.Overwrite);
                    file.BufferStream = new BufferedStream(File.OpenWrite(file.Path));
                }

                while (sourceStream.Position < sourceStream.Length)
                {
                    if (cancel == 1)
                    {
                        progress.Canceled = true;
                        break;
                    }
                    int streamLength = sourceStream.Read(sourceBuffer, 0, Constants.COPY_BUFFER_SIZE);
                    foreach (DestinationInfo file in destinations)
                    {
                        file.BufferStream.Write(sourceBuffer, 0, streamLength);
                    }
                    progress.Processed = sourceStream.Position;
                }
                progress.Completed = true;
            }
            catch (Exception ex) 
            {
                if (progress != null)
                {
                    progress.Canceled = true;
                }
                exception = ex;
            }
            finally
            {
                foreach (DestinationInfo file in destinations)
                {
                    if (file.BufferStream != null)
                    {
                        file.BufferStream.Close();
                    }
                }
                if (sourceStream != null)
                {
                    sourceStream.Close();
                }
            }
            if (exception != null)
            {
                throw exception;
            }
        }

    }
}
