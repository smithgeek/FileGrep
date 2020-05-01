using System;
using System.Collections.Generic;
using Smithgeek.Extensions;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace Smithgeek.IO
{
    /// <summary>
    /// Static helper methods when dealing with directories.
    /// </summary>
    public static class DirectoryExt
    {
        /// <summary>
        /// Delete all files in the given directory that match the provided pattern.
        /// </summary>
        /// <param name="aPath">The directory where the files should be found.</param>
        /// <param name="aSearchOption">The directory search options.</param>
        /// <returns>If the deletion was successful.</returns>
        public static bool DeleteFiles(String aPath, DirectorySearchOption aSearchOption)
        {
            bool success = true;
            if (Directory.Exists(aPath))
            {
                success = FileExt.Delete(DirectoryExt.GetFiles(aPath, aSearchOption));
            }
            return success;
        }

        /// <summary>
        /// Moves a directory from one location to another.
        /// </summary>
        /// <param name="aSource">The source location for the move.  If the destination location already exists
        /// it will be renamed.</param>
        /// <param name="aDestination">The destination location for the move.</param>
        /// <returns>If the move was successful.</returns>
        public static bool Move(String aSource, String aDestination)
        {
            return Move(aSource, aDestination, NameConflictOption.RenameExisting);
        }

        /// <summary>
        /// Creates a new unique directory name based off of the given directory name.
        /// </summary>
        /// <param name="aPath">The path to the directory.</param>
        /// <returns>New unique directory name.</returns>
        public static String MakeUnique(String aPath)
        {
            if (aPath.isEmpty())
                return String.Empty;
            String newDirectoryName = aPath.TrimEnd(new char[] { '\\' }) + "_1";
            while (Directory.Exists(newDirectoryName))
            {
                newDirectoryName += "_1";
            }
            return newDirectoryName;
        }

        /// <summary>
        /// Moves a directory from one location to another.
        /// </summary>
        /// <param name="aSource">The source location for the move.</param>
        /// <param name="aDestination">The destination location for the move.</param>
        /// <param name="aNameConflictOption">How to handle name conflicts</param>
        /// <returns>If the move was successful.</returns>
        public static bool Move(String aSource, String aDestination, NameConflictOption aNameConflictOption)
        {
            if (!Directory.Exists(aSource))
            {
                throw new FileNotFoundException("The source directory does not exist");
            }
            bool success = true;
            if (Directory.Exists(aSource))
            {
                success = false;
                
                if(Directory.Exists(aDestination))
                {
                    switch (aNameConflictOption)
                    {
                        case NameConflictOption.Skip:
                        case NameConflictOption.Cancel:
                            return false;
                        case NameConflictOption.Overwrite:
                            DirectoryExt.Delete(aDestination);
                            break;
                        case NameConflictOption.RenameExisting:
                            DirectoryExt.Move(aDestination, DirectoryExt.MakeUnique(aDestination), NameConflictOption.RenameNew);
                            break;
                        case NameConflictOption.RenameNew:
                            aDestination = DirectoryExt.MakeUnique(aDestination);
                            break;
                        default:
                            throw new ArgumentException("Invalid conflict option.");
                    }
                }

                int tries = 0;
                while (!success && tries < Constants.STANDARD_RETRY_ATTEMPTS)
                {
                    try
                    {
                        Directory.Move(aSource, aDestination);
                        success = true;
                    }
                    catch
                    {
                        tries++;
                        System.Threading.Thread.Sleep(Constants.STANDARD_RETRY_SLEEP);
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Delete a directory
        /// </summary>
        /// <param name="aDirectory">The path to the directory</param>
        /// <returns>If the deletion was successful.</returns>
        public static bool Delete(String aDirectory)
        {
            bool success = true;
            if (Directory.Exists(aDirectory))
            {
                success = false;
                int tries = 0;
                while (!success && tries < Constants.STANDARD_RETRY_ATTEMPTS)
                {
                    try
                    {
                        Directory.Delete(aDirectory, true);
                        success = true;
                    }
                    catch (Exception)
                    {
                        tries++;
                        System.Threading.Thread.Sleep(Constants.STANDARD_RETRY_SLEEP);
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Copies a directory including any subdirectories and overwrites any files if they exist.
        /// </summary>
        /// <param name="aSource">The path to the source location</param>
        /// <param name="aDestination">The path to the destination location</param>
        public static void Copy(String aSource, String aDestination)
        {
            Copy(aSource, aDestination, new DirectorySearchOption(SearchOption.AllDirectories), NameConflictOption.Overwrite);
        }

        /// <summary>
        /// Copies a directory.
        /// </summary>
        /// <param name="aSource">The source location.</param>
        /// <param name="aDestination">The destination location.</param>
        /// <param name="aSearchOption">If sub directories should be copied.</param>
        /// <param name="aConflictOption">How to handle name conflicts.</param>
        public static void Copy(String aSource, String aDestination, DirectorySearchOption aSearchOption, NameConflictOption aConflictOption)
        {
            if (Directory.Exists(aSource))
            {
                if (!Directory.Exists(aDestination))
                {
                    Directory.CreateDirectory(aDestination);
                }
                //Now Create all of the directories
                foreach (string dirPath in DirectoryExt.GetDirectories(aSource, aSearchOption))
                {
                    Create(dirPath.Replace(aSource.endDir(), aDestination.endDir()));
                }

                //Copy all the files
                foreach (string copyFrom in DirectoryExt.GetFiles(aSource, aSearchOption))
                {
                    String copyTo = copyFrom.Replace(aSource.endDir(), aDestination.endDir());
                    if (File.Exists(copyTo))
                    {
                        if (aConflictOption == NameConflictOption.Cancel)
                            return;
                        else if (aConflictOption == NameConflictOption.Skip)
                            continue;
                    }
                    FileExt.Copy(copyFrom, copyTo, aConflictOption);
                }
            }
        }

        /// <summary>
        /// Creates a directory if it doesn't exist.
        /// </summary>
        /// <param name="aDirectory">The directory to create.</param>
        public static void Create(String aDirectory)
        {
            if (!Directory.Exists(aDirectory))
            {
                Directory.CreateDirectory(aDirectory);
            }
        }

        /// <summary>
        /// Gets the first file in the directory that matches the search options.
        /// </summary>
        /// <param name="aPath">The directory to search.</param>
        /// <param name="aSearchOption">The options to use when searching.</param>
        /// <param name="aFile">The path to the first file or an empty string if no files are found.</param>
        /// <returns>True if a file was found.</returns>
        public static bool GetFirstFile(String aPath, DirectorySearchOption aSearchOption, out String aFile)
        {
            aFile = String.Empty;
            List<String> files = DirectoryExt.GetFiles(aPath, aSearchOption);
            if (!files.isNull())
            {
                foreach (String file in files)
                {
                    aFile = file;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if a directory is completely empty.
        /// </summary>
        /// <param name="aPath">The path to the directory to search.</param>
        /// <returns>True if the directory contains no files or folders.</returns>
        public static bool Empty(String aPath)
        {
            bool empty = Directory.GetDirectories(aPath, "*", SearchOption.TopDirectoryOnly).Length == 0;
            if (empty)
            {
                empty = Directory.GetFiles(aPath, "*", SearchOption.TopDirectoryOnly).Length == 0;
            }
            return empty;
        }

        /// <summary>
        /// Gets a list of files.
        /// </summary>
        /// <param name="aPath">The directory to search.</param>
        /// <param name="aSearchOption">The search options.</param>
        /// <returns>List of files</returns>
        public static List<String> GetFiles(String aPath, DirectorySearchOption aSearchOption)
        {
            List<String> files = new List<string>();
            if (Directory.Exists(aPath))
            {
                files = Directory.GetFiles(aPath, aSearchOption.Pattern, aSearchOption.SearchOption).ToList();
                if (aSearchOption.ExclusionFilter != null)
                {
                    foreach (Regex exclusion in aSearchOption.ExclusionFilter)
                    {
                        files.RemoveAll(element => exclusion.IsMatch(PathExt.GetRelativePath(aPath, element)));
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// Gets a list of directories.
        /// </summary>
        /// <param name="aPath">The directory to search.</param>
        /// <param name="aSearchOption">The search options.</param>
        /// <returns>List of directories.</returns>
        public static List<String> GetDirectories(String aPath, DirectorySearchOption aSearchOption)
        {
            List<String> directories = new List<string>();
            if (Directory.Exists(aPath))
            {
                directories = Directory.GetDirectories(aPath, aSearchOption.Pattern, aSearchOption.SearchOption).ToList();
                if (aSearchOption.ExclusionFilter != null)
                {
                    foreach (Regex exclusion in aSearchOption.ExclusionFilter)
                    {
                        directories.RemoveAll(element => exclusion.IsMatch(PathExt.GetRelativePath(aPath, element.endDir())));
                    }
                }
            }
            return directories;
        }
    }
}