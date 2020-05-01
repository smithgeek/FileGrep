using System;
using System.Collections.Generic;
using System.Text;
using Smithgeek.Extensions;
using System.Linq;
using System.IO;

namespace Smithgeek.IO
{
    /// <summary>
    /// Helper static functions to extend the path class.
    /// </summary>
    public static class PathExt
    {
        /// <summary>
        /// Gets a relative path between a root directory and a full file path.
        /// </summary>
        /// <param name="aRootDir">The root directory.</param>
        /// <param name="aFullPath">The full file path.</param>
        /// <returns>The relative path.</returns>
        public static String GetRelativePath(String aRootDir, String aFullPath)
        {
            if (!aFullPath.StartsWith(aRootDir.endDir()))
                return String.Empty;
            return aFullPath.Substring(aRootDir.endDir().Length);
        }

        /// <summary>
        /// Combines a list of paramters into a directory path, directories can be separated by '/' or '\'.
        /// </summary>
        /// <param name="strings">Parts of the path to combine</param>
        /// <returns>The full file path.</returns>
        public static String Combine(params String[] strings)
        {
            if (strings == null || strings.Length == 0)
                return string.Empty;
            if (strings.Length == 1)
                return strings[0];
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < strings.Length; ++i)
            {
                if (!strings[i].isEmpty())
                {
                    String str = strings[i].Replace("/", @"\");
                    if(result.Length > 0)
                        str = str.Trim(new char[] { Path.DirectorySeparatorChar });
                    result.Append(str.endDir());
                }
            }
            return result.ToString().TrimEnd(Path.DirectorySeparatorChar); ;
        }

        /// <summary>
        /// Gets the directory name before the last '\'.
        /// </summary>
        /// <param name="aPath">The file path.</param>
        /// <returns>The directory name.</returns>
        public static String GetParentDirectory(String aPath)
        {
            String result = String.Empty;
            if (!aPath.isEmpty())
            {
                String fullDirectory = Path.GetDirectoryName(aPath);
                if (fullDirectory.isEmpty())
                {
                    fullDirectory = aPath;
                }
                String[] directories = fullDirectory.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (directories.Length > 0)
                {
                    result = directories[directories.Length - 1];
                }
            }
            return result;
        }
    }

}