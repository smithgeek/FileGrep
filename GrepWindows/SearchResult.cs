using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepWindows
{
    /// <summary>
    /// Contains a line of text from a file and the line number in the file
    /// </summary>
    [Serializable]
    public class LineText
    {
        public int LineNumber { get; set; }
        public String Text { get; set; }

        public LineText()
        {
        }

        public LineText(int lineNumber, String text)
        {
            LineNumber = lineNumber;
            Text = text;
        }

        /// <summary>
        /// Why did I do this... I really can't remember.
        /// </summary>
        ~LineText()
        {
            Text = String.Empty;
        }
    }

    /// <summary>
    /// Contains a search result that consists of the line number where something was found
    /// and the line text.
    /// </summary>
    [Serializable]
    public class SearchResult
    {
        public LineText MatchLine { get; set; }

        public List<LineText> BeforeContext { get; set; }

        public List<LineText> AfterContext { get; set; }

        /// <summary>
        /// The longest string width between the match line and all of the context lines.
        /// </summary>
        private int mMaxWidth;

        public SearchResult()
        {
            MatchLine = new LineText();
            BeforeContext = new List<LineText>();
            AfterContext = new List<LineText>();
            mMaxWidth = -1;
        }

        /// <summary>
        /// Gets the max line width for all context lines and the matching line.
        /// </summary>
        /// <returns>The length of the longest string</returns>
        public int getMaxWidth()
        {
            if (mMaxWidth == -1)
            {
                int width = MatchLine.Text.Length;
                if (BeforeContext.Count > 0)
                {
                    width = Math.Max(width, BeforeContext.Max(cl => cl.Text.Length));
                }
                if (AfterContext.Count > 0)
                {
                    width = Math.Max(width, AfterContext.Max(cl => cl.Text.Length));
                }
                mMaxWidth = width;
            }
            return mMaxWidth;
        }
    }
}
