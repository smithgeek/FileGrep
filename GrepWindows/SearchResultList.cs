using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrepWindows
{
    /// <summary>
    /// Contains a list of all the results for a single file
    /// </summary>
    [Serializable]
    class SearchResultList
    {
        /// <summary>
        /// List of results found in this file
        /// </summary>
        public List<SearchResult> Results { get; set; }

        /// <summary>
        /// The name of the file
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// The regular expresion matches for this file
        /// </summary>
        public MatchCollection Matches { get; set; }

        /// <summary>
        /// Create a blank result list for a given file
        /// </summary>
        /// <param name="filename">The name of the file being searched.</param>
        public SearchResultList(String filename)
        {
            FileName = filename;
            Results = new List<SearchResult>();
        }

        /// <summary>
        /// Destruct it all, waiting for garbage collection is for whimps!
        /// </summary>
        ~SearchResultList()
        {
            Results.Clear();
        }

        public override string ToString()
        {
            return String.Format("{0}: {1} matches found", FileName, Matches.Count);
        }

        /// <summary>
        /// Removes context lines that either are also a match line or would duplicate another context line.
        /// </summary>
        public void removeExtraContext()
        {
            for (int i = 0; i < Results.Count; i++)
            {
                SearchResult result = Results[i];
                if (result.AfterContext.Count > 0 && i < Results.Count - 1)
                {
                    int nextMatchLine = Results[i + 1].MatchLine.LineNumber;
                    result.AfterContext.RemoveAll(cl => cl.LineNumber >= nextMatchLine);
                }

                if (i > 0 && result.BeforeContext.Count > 0)
                {
                    SearchResult previousResult = Results[i - 1];
                    int previousMatchLine = previousResult.MatchLine.LineNumber;
                    result.BeforeContext.RemoveAll(cl => cl.LineNumber <= previousMatchLine);
                    if (previousResult.AfterContext.Count > 0)
                    {
                        result.BeforeContext.RemoveAll(cl => cl.LineNumber <= previousResult.AfterContext[previousResult.AfterContext.Count - 1].LineNumber);
                    }
                }
            }
        }
    }
}
