using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GrepWindows
{
    /// <summary>
    /// Contains a color that can be converted to an RTF string and has an Id to correspond to the color table
    /// </summary>
    [Serializable]
    public class RtfColor
    {
        /// <summary>
        /// Color to use
        /// </summary>
        private Color mColor;

        /// <summary>
        /// Id of this color in the rtf color table
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Create an rtf color
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <param name="id">The id in the rtf color table</param>
        public RtfColor(Color color, int id)
        {
            mColor = color;
            Id = id;
        }

        /// <summary>
        /// Gets the actual color
        /// </summary>
        /// <returns>Return the color</returns>
        public Color getColor()
        {
            return mColor;
        }

        /// <summary>
        /// Sets the color
        /// </summary>
        /// <param name="color">The new color to use</param>
        public void setColor(Color color)
        {
            mColor = color;
        }

        /// <summary>
        /// Converts the color into an rtf color table string
        /// </summary>
        /// <returns></returns>
        public String getRtfString()
        {
            return String.Format(@"\red{0}\green{1}\blue{2};", mColor.R, mColor.G, mColor.B);
        }
    }

    /// <summary>
    /// Groups a color and an optional alternate color
    /// </summary>
    [Serializable]
    public class ColorGroup
    {
        /// <summary>
        /// The main color to use
        /// </summary>
        private RtfColor mColor;

        /// <summary>
        /// An alternate color to use (may be null)
        /// </summary>
        private RtfColor mAltColor;

        /// <summary>
        /// If an alternate color was provided
        /// </summary>
        private bool mHasAltColor;

        /// <summary>
        /// Description text
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Create a color group without an alternate color
        /// </summary>
        /// <param name="description">The description for this group</param>
        /// <param name="color">The main color to use</param>
        public ColorGroup(String description, RtfColor color)
        {
            Description = description;
            mColor = color;
            mHasAltColor = false;
        }

        /// <summary>
        /// Create a color group with an alternate color
        /// </summary>
        /// <param name="description">The description for this group</param>
        /// <param name="color">The main color</param>
        /// <param name="altColor">The alternate color</param>
        public ColorGroup(String description, RtfColor color, RtfColor altColor)
        {
            Description = description;
            mColor = color;
            mAltColor = altColor;
            mHasAltColor = true;
        }

        /// <summary>
        /// Get the main color
        /// </summary>
        /// <returns>The main color</returns>
        public RtfColor getColor()
        {
            return getColor(false);
        }

        /// <summary>
        /// Get the color. If an alternate color is requested but one does not exist
        /// the main color will be returned
        /// </summary>
        /// <param name="alt">If you want the alternate color</param>
        /// <returns>The color requested</returns>
        public RtfColor getColor(bool alt)
        {
            if (!alt || !mHasAltColor)
            {
                return mColor;
            }
            else
            {
                return mAltColor;
            }
        }

        /// <summary>
        /// Does this group have an alternate color?
        /// </summary>
        /// <returns>True if an alternate color exists</returns>
        public bool hasAlternateColor()
        {
            return mHasAltColor;
        }

        /// <summary>
        /// Gets all the colors in this group.
        /// </summary>
        /// <returns>All colors in the group</returns>
        public IEnumerable<RtfColor> getColors()
        {
            List<RtfColor> colors = new List<RtfColor>();
            colors.Add(mColor);
            if (mHasAltColor)
            {
                colors.Add(mAltColor);
            }
            return colors;
        }
    }

    /// <summary>
    /// Settings for all the possible colors used in the RTF display
    /// </summary>
    [Serializable]
    public class ColorSettings
    {
        /// <summary>
        /// Background for the filename
        /// </summary>
        public ColorGroup FileBack { get; set; }

        /// <summary>
        /// Font color for the filename
        /// </summary>
        public ColorGroup FileText { get; set; }

        /// <summary>
        /// Background for the line number
        /// </summary>
        public ColorGroup LineNumBack { get; set; }

        /// <summary>
        /// Font color for the line number
        /// </summary>
        public ColorGroup LineNumText { get; set; }

        /// <summary>
        /// Highlight background for text that matches the search pattern.
        /// </summary>
        public ColorGroup MatchHighBack { get; set; }

        /// <summary>
        /// Font color for text that matches the search pattern.
        /// </summary>
        public ColorGroup MatchHighText { get; set; }

        /// <summary>
        /// Background for normal text
        /// </summary>
        public ColorGroup NormalBack { get; set; }

        /// <summary>
        /// Font color for normal text
        /// </summary>
        public ColorGroup NormalText { get; set; }

        /// <summary>
        /// Background for context text
        /// </summary>
        public ColorGroup ContextBack { get; set; }

        /// <summary>
        /// Font color for context text
        /// </summary>
        public ColorGroup ContextText { get; set; }

        /// <summary>
        /// Background for line number context
        /// </summary>
        public ColorGroup ContextLineNumBack { get; set; }

        /// <summary>
        /// Font color for line number context
        /// </summary>
        public ColorGroup ContextLineNumText { get; set; }

        /// <summary>
        /// Background color for the results rich text box
        /// </summary>
        public ColorGroup ResultsBackground { get; set; }

        /// <summary>
        /// List of all the color groups
        /// </summary>
        public List<ColorGroup> ColorList { get; set; }

        /// <summary>
        /// Construction time
        /// </summary>
        public ColorSettings()
        {
            resetToDefaults();
        }

        /// <summary>
        /// Reset all colors to their default values.
        /// </summary>
        public void resetToDefaults()
        {
            FileBack = new ColorGroup("Filename Highlight", new RtfColor(Color.FromArgb(198, 195, 198), 1));
            FileText = new ColorGroup("Filename Text", new RtfColor(Color.FromArgb(0, 0, 132), 2));

            Color altBackground = Color.FromArgb(225, 225, 225);
            LineNumBack = new ColorGroup("Line Number Highlight", new RtfColor(Color.FromArgb(255, 255, 255), 3), new RtfColor(altBackground, 21));
            LineNumText = new ColorGroup("Line Number Text", new RtfColor(Color.FromArgb(43, 145, 175), 4), new RtfColor(Color.FromArgb(43, 145, 175), 22));
            MatchHighBack = new ColorGroup("Match Highlight", new RtfColor(Color.FromArgb(0, 0, 255), 5), new RtfColor(Color.FromArgb(0, 0, 255), 7));
            MatchHighText = new ColorGroup("Match Text", new RtfColor(Color.FromArgb(255, 255, 255), 6), new RtfColor(Color.FromArgb(255, 255, 255), 8));
            NormalBack = new ColorGroup("Normal Highlight", new RtfColor(Color.FromArgb(255, 255, 255), 9), new RtfColor(altBackground, 11));
            NormalText = new ColorGroup("Normal Text", new RtfColor(Color.FromArgb(0, 0, 0), 10), new RtfColor(Color.FromArgb(0, 0, 0), 12));
            ContextBack = new ColorGroup("Context Highlight", new RtfColor(Color.FromArgb(255, 255, 255), 13), new RtfColor(altBackground, 15));
            ContextText = new ColorGroup("Context Text", new RtfColor(Color.FromArgb(140, 140, 140), 14), new RtfColor(Color.FromArgb(140, 140, 140), 16));
            ContextLineNumBack = new ColorGroup("Context Line Number Highlight", new RtfColor(Color.FromArgb(255, 255, 255), 17), new RtfColor(altBackground, 19));
            ContextLineNumText = new ColorGroup("Context Line Number Text", new RtfColor(Color.FromArgb(140, 140, 140), 18), new RtfColor(Color.FromArgb(140, 140, 140), 20));
            ResultsBackground = new ColorGroup("Results Background", new RtfColor(Color.White, -1));

            ColorList = new List<ColorGroup>();
            ColorList.Add(FileBack);
            ColorList.Add(FileText);
            ColorList.Add(LineNumBack);
            ColorList.Add(LineNumText);
            ColorList.Add(MatchHighBack);
            ColorList.Add(MatchHighText);
            ColorList.Add(NormalBack);
            ColorList.Add(NormalText);
            ColorList.Add(ContextBack);
            ColorList.Add(ContextText);
            ColorList.Add(ContextLineNumBack);
            ColorList.Add(ContextLineNumText);
            ColorList.Add(ResultsBackground);
        }

        /// <summary>
        /// Creates an RTF color table string from all the color groups
        /// </summary>
        /// <returns></returns>
        public String getColorTblString()
        {
            List<RtfColor> orderedList = new List<RtfColor>();
            foreach (ColorGroup group in ColorList)
            {
                orderedList.AddRange(group.getColors());
            }
            orderedList = orderedList.OrderBy(c => c.Id).ToList();
            StringBuilder builder = new StringBuilder();
            foreach (RtfColor color in orderedList)
            {
                if (color.Id != -1)
                {
                    builder.Append(color.getRtfString());
                }
            }
            return builder.ToString();
        }
    }
}
