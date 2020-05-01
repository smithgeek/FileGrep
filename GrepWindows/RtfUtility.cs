using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace GrepWindows
{
    class RtfUtility
    {
        private const string RTF_HEADER = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Courier New;}}";

        private const String COLOR_TABLE = @"{{\colortbl;{0}}}";

        private const String VIEWKIND = @"\viewkind4\uc1\pard\fs20";

        public static void WriteFileHeader(TextWriter aWriter)
        {
            aWriter.Write(RtfUtility.RTF_HEADER);
            aWriter.Write(String.Format(RtfUtility.COLOR_TABLE, Settings.get().Colors.getColorTblString()));
            aWriter.Write(RtfUtility.VIEWKIND);
            if (Logger.get().LoggingEnabled)
            {
                Logger.get().AddData(RTF_HEADER + COLOR_TABLE + VIEWKIND, "RtfHeader", "Rich text format file header.");
            }
        }

        private static String getFontColor(String color)
        {
            return @"\cf" + color;
        }

        private static String getHighlightColor(String color)
        {
            return @"\highlight" + color;
        }

        public static String createRtf(SearchResultList results, bool filenameSearch, Regex regex)
        {
            ColorSettings colors = Settings.get().Colors;
            StringBuilder builder = new StringBuilder();
            if (filenameSearch || results.FileName.StartsWith("Error"))
            {
                append(builder, colors.NormalText.getColor(), colors.NormalBack.getColor(), @"{0}\par", results.FileName, 0);
            }
            else
            {
                append(builder, colors.FileText.getColor(), colors.FileBack.getColor(), @"{0}\par", results.FileName, 0);
            }

            results.removeExtraContext();
            for (int resultIndex = 0; resultIndex < results.Results.Count; ++resultIndex)
            {
                bool useAltColor = Settings.get().ContextLineCount > 0 && resultIndex % 2 == 1;
                SearchResult result = results.Results[resultIndex];
                int maxLineWidth = result.getMaxWidth();
                // Print context lines
                appendContextLines(builder, result.BeforeContext, resultIndex, maxLineWidth);

                MatchCollection matches = regex.Matches(result.MatchLine.Text);
                int index = 0;
                append(builder, colors.LineNumText.getColor(useAltColor), colors.LineNumBack.getColor(useAltColor), @" {0}: ", result.MatchLine.LineNumber.ToString(), 0);
                for (int mi = 0; mi < matches.Count; mi++)
                {
                    append(builder, colors.NormalText.getColor(useAltColor), colors.NormalBack.getColor(useAltColor), @" {0}", result.MatchLine.Text.Substring(index, matches[mi].Index - index), 0);
                    append(builder, colors.MatchHighText.getColor(useAltColor), colors.MatchHighBack.getColor(useAltColor), @" {0}", result.MatchLine.Text.Substring(matches[mi].Index, matches[mi].Length), 0);
                    index = matches[mi].Index + matches[mi].Length;
                }
                append(builder, colors.NormalText.getColor(useAltColor), colors.NormalBack.getColor(useAltColor), @" {0} \par", result.MatchLine.Text.Substring(index), maxLineWidth - index);

                // Print context lines
                appendContextLines(builder, result.AfterContext, resultIndex, maxLineWidth);
            }
            return builder.ToString().Replace("\0", "").Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Append the context lines to the rtf output.
        /// </summary>
        /// <param name="builder">String builder to append the context lines to</param>
        /// <param name="contextLines">The context lines to append</param>
        /// <param name="resultIndexInFile">The 0 based index of the result in the file</param>
        /// <param name="maxLineWidth">The longest line, used for padding</param>
        private static void appendContextLines(StringBuilder builder, List<LineText> contextLines, int resultIndexInFile, int maxLineWidth)
        {
            ColorSettings colors = Settings.get().Colors;
            contextLines = contextLines.OrderBy(cl => cl.LineNumber).ToList();
            foreach (LineText contextLine in contextLines)
            {
                bool useAltColor = resultIndexInFile % 2 == 1;
                append(builder, colors.ContextLineNumText.getColor(useAltColor), colors.ContextLineNumBack.getColor(useAltColor), @" {0}: ", contextLine.LineNumber.ToString(), 0);
                append(builder, colors.ContextText.getColor(useAltColor), colors.ContextBack.getColor(useAltColor), @" {0}\par", contextLine.Text, maxLineWidth);
            }
        }

        /// <summary>
        /// Appends some text to the string builder with the provided format.  Escapes certain characters for rtf printing
        /// </summary>
        /// <param name="builder">The string builder to append to</param>
        /// <param name="format">The format to use while appending</param>
        /// <param name="text">The text to append</param>
        private static void append(StringBuilder builder, RtfColor fontColor, RtfColor highlightColor, String format, String text, int maxLineWidth)
        {
            String formatWithColor = getFontColor(fontColor.Id.ToString()) + getHighlightColor(highlightColor.Id.ToString()) + format;
            builder.AppendFormat(formatWithColor, text.PadRight(maxLineWidth).Replace(@"\", @"\\").Replace("}", "\\}").Replace("{", "\\{"));
        }
    }
}
