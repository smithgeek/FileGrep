using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Smithgeek.Extensions;

namespace Smithgeek.IO
{
    /// <summary>
    /// Describes how a directory search should be performed.  Including providing a wildcard pattern to include, 
    /// an optional exclusion wildcard or regex pattern and the option to search sub directories.
    /// </summary>
    public class DirectorySearchOption
    {
        /// <summary>
        /// Optional: List of regular expressions or wildcard strings to exclude in the directory search.
        /// </summary>
        public List<Regex> ExclusionFilter { get; set; }

        /// <summary>
        /// Required: The wildcard pattern to use when searching for files or directories.
        /// </summary>
        public String Pattern
        {
            get { return mPattern; }
            set
            {
                if (value.isEmpty())
                {
                    mPattern = "*";
                }
                else
                {
                    mPattern = value;
                }
            }
        }
        private String mPattern;

        /// <summary>
        /// If subdirectories should be searched.
        /// </summary>
        public System.IO.SearchOption SearchOption { get; set; }

        /// <summary>
        /// Default directory search option finds all files or directories and searches sub directories.
        /// </summary>
        public DirectorySearchOption()
        {
            Pattern = "*";
            ExclusionFilter = null;
            SearchOption = System.IO.SearchOption.AllDirectories;
        }

        /// <summary>
        /// Finds all files or directories.
        /// </summary>
        /// <param name="aSearchOption">If sub directories will be searched</param>
        public DirectorySearchOption(System.IO.SearchOption aSearchOption)
        {
            Pattern = "*";
            ExclusionFilter = null;
            SearchOption = aSearchOption;
        }

        /// <summary>
        /// Finds all files or directories following the given pattern including sub directories.
        /// </summary>
        /// <param name="aPattern">The pattern to use when finding files or directories.</param>
        public DirectorySearchOption(String aPattern)
        {
            Pattern = aPattern;
            ExclusionFilter = null;
            SearchOption = System.IO.SearchOption.AllDirectories;
        }

        /// <summary>
        /// Finds all files or directories following the given pattern.
        /// </summary>
        /// <param name="aPattern">The pattern to use when finding files or directories.</param>
        /// <param name="aSearchOption">If sub directories will be searched</param>
        public DirectorySearchOption(String aPattern, System.IO.SearchOption aSearchOption)
        {
            Pattern = aPattern;
            ExclusionFilter = null;
            SearchOption = aSearchOption;
        }

        /// <summary>
        /// Finds all files or directories following the given pattern that aren't excluded.
        /// </summary>
        /// <param name="aPattern">The pattern to use when finding files or directories.</param>
        /// <param name="aExclusions">List of regular expressions or wildcard expressions to exclude even if they match the pattern.</param>
        /// <param name="aSearchOption">If sub directories will be searched</param>
        public DirectorySearchOption(String aPattern, List<Regex> aExclusions, System.IO.SearchOption aSearchOption)
        {
            Pattern = aPattern;
            ExclusionFilter = aExclusions;
            SearchOption = aSearchOption;
        }
    }
}
