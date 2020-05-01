using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Smithgeek.Extensions;
using System.IO;

namespace GrepWindows
{
    public partial class FrmSettings : Form
    {

        Settings mSettings;
        BindingList<QuickScope> mBindingList;
        private bool mSkipCustomLoad;

        public FrmSettings(Settings settings)
        {
            InitializeComponent();
            mSkipCustomLoad = false;
            mSettings = settings;

            uExternalApplication.Text = mSettings.ExternalApplication;
            uFileCommand.Text = mSettings.FileArgs;
            uFileLineCommand.Text = mSettings.WithLineNumberArgs;
            uResultsTab.Checked = mSettings.OpenNewTab;

            mBindingList = new BindingList<QuickScope>(mSettings.QuickScopeList);
            uQuickScopeList.DataSource = mBindingList;
            uQuickScopeList.DisplayMember = "DisplayText";
            uPresetList.SelectedIndex = (int)mSettings.ExternalAppType;
            
            // Color settings tab
            uListColor.DataSource = Settings.get().Colors.ColorList;
            uListColor.DisplayMember = "Description";
            writeExampleText();
        }

        private void FrmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (uPresetList.SelectedIndex != (int)GrepWindows.Settings.BuiltInExternalApps.VISUAL_STUDIO)
            {
                mSettings.ExternalApplication = uExternalApplication.Text;
                mSettings.FileArgs = uFileCommand.Text;
                mSettings.WithLineNumberArgs = uFileLineCommand.Text;
            }
            mSettings.OpenNewTab = uResultsTab.Checked;
            mSettings.ExternalAppType = (Settings.BuiltInExternalApps)uPresetList.SelectedIndex;
        }

        /// <summary>
        /// Writes the example text to the rich text box.
        /// </summary>
        private void writeExampleText()
        {
            int oldContext = Settings.get().ContextLineCount;
            Settings.get().ContextLineCount = 3;
            using (MemoryStream stream = new MemoryStream())
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    RtfUtility.WriteFileHeader(writer);
                    writer.Write(generateExampleRtf());
                    writer.Flush();
                    stream.Position = 0;
                    uRtfExample.LoadFile(stream, RichTextBoxStreamType.RichText);
                }
            }
            Settings.get().ContextLineCount = oldContext;
        }

        /// <summary>
        /// Generates some exmaple rich text to demonstrate all the colors
        /// </summary>
        /// <returns>String full of rich text</returns>
        private String generateExampleRtf()
        {
            SearchResultList list = new SearchResultList(@"D:\Projects\PND\unified-2011\submodules\technology\os\core\include\tsk\TSK_pub_lib.h");
            SearchResult result = new SearchResult();
            result.BeforeContext.Add(new LineText(1058, @"                               VARIABLES"));
            result.BeforeContext.Add(new LineText(1059, @"--------------------------------------------------------------------*/"));
            result.BeforeContext.Add(new LineText(1060, @""));
            result.MatchLine.LineNumber = 1061;
            result.MatchLine.Text = @"extern TSK_id_t32       TSK_bkgd_id;";
            result.AfterContext.Add(new LineText(1062, @"extern uint32           TSK_bkgd_stack[];"));
            result.AfterContext.Add(new LineText(1063, @""));
            result.AfterContext.Add(new LineText(1064, @"extern TSK_id_t32       TSK_timr_id;"));
            list.Results.Add(result);
            result = new SearchResult();
            result.BeforeContext.Add(new LineText(1338, @"*********************************************************************/"));
            result.BeforeContext.Add(new LineText(1339, @""));
            result.BeforeContext.Add(new LineText(1340, @"#define TSK_multitasking() \"));
            result.MatchLine.LineNumber = 1341;
            result.MatchLine.Text = @"( ( ( *TSK_get_id_addr() == NULL) || ( TSK_get_id() == TSK_bkgd_id)) ? FALSE : TRUE)";
            result.AfterContext.Add(new LineText(1342, @""));
            result.AfterContext.Add(new LineText(1343, @"/*********************************************************************"));
            result.AfterContext.Add(new LineText(1344, @"*"));
            list.Results.Add(result);
            Regex regex = new Regex("TSK_bkgd_id");
            return RtfUtility.createRtf(list, false, regex);
        }

        private void uBtnAdd_Click(object sender, EventArgs e)
        {
            if(!uScopeText.Text.isEmpty() && !uButtonDisplayText.Text.isEmpty())
            {
                mBindingList.Add(new QuickScope(uScopeText.Text, uButtonDisplayText.Text));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (uQuickScopeList.SelectedItem != null)
            {
                mBindingList.Remove((QuickScope)uQuickScopeList.SelectedItem);
            }
        }

        private void uBtnUp_Click(object sender, EventArgs e)
        {
            int index = uQuickScopeList.SelectedIndex;
            if (uQuickScopeList.SelectedItem != null && index > 0)
            {
                QuickScope moving = mBindingList[index];
                mBindingList.RemoveAt(index);
                mBindingList.Insert(index - 1, moving);
                uQuickScopeList.SelectedIndex = index - 1;
            }
        }

        private void uBtnDown_Click(object sender, EventArgs e)
        {
            int index = uQuickScopeList.SelectedIndex;
            if (uQuickScopeList.SelectedItem != null && index < mBindingList.Count - 1)
            {
                QuickScope moving = mBindingList[index];
                mBindingList.RemoveAt(index);
                mBindingList.Insert(index + 1, moving);
                uQuickScopeList.SelectedIndex = index + 1;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uPresetList.SelectedIndex >= 0 && uPresetList.SelectedIndex < uPresetList.Items.Count)
            {
                bool custom = uPresetList.SelectedIndex != (int)GrepWindows.Settings.BuiltInExternalApps.VISUAL_STUDIO;
                uExternalApplication.Enabled = custom;
                uFileCommand.Enabled = custom;
                uFileLineCommand.Enabled = custom;

                if (uPresetList.SelectedIndex == (int)GrepWindows.Settings.BuiltInExternalApps.CUSTOM)
                {
                    if (mSkipCustomLoad)
                    {
                        mSkipCustomLoad = false;
                    }
                    else
                    {
                        uExternalApplication.Text = mSettings.ExternalApplication;
                        uFileCommand.Text = mSettings.FileArgs;
                        uFileLineCommand.Text = mSettings.WithLineNumberArgs;
                    }
                }
                else if (uPresetList.SelectedIndex == (int)GrepWindows.Settings.BuiltInExternalApps.NOTEPAD_PLUS_PLUS)
                {
                    uExternalApplication.Text = "c:\\Program Files (x86)\\Notepad++\\notepad++.exe";
                    uFileCommand.Text = "\"{file}\"";
                    uFileLineCommand.Text = "\"{file}\" -n{line}";
                    mSkipCustomLoad = true;
                    uPresetList.SelectedIndex = (int)GrepWindows.Settings.BuiltInExternalApps.CUSTOM;
                }
                else
                {
                    uExternalApplication.Text = String.Empty;
                    uFileCommand.Text = String.Empty;
                    uFileLineCommand.Text = String.Empty;
                }
            }
        }

        /// <summary>
        /// Handles when a color is selected in the drop down list.
        /// </summary>
        private void uListColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshDisplay();
        }

        /// <summary>
        /// Updates the color settings tab with up to date colors
        /// </summary>
        private void refreshDisplay()
        {
            ColorGroup colorGroup = uListColor.SelectedItem as ColorGroup;
            if (colorGroup != null)
            {
                bool altEnabled = colorGroup.hasAlternateColor();
                uPanelAltColor.Enabled = altEnabled;
                uBtnChangeAltColor.Enabled = altEnabled;
                uLblAltColor.Enabled = altEnabled;

                uPanelColor.BackColor = colorGroup.getColor().getColor();

                if (altEnabled)
                {
                    uPanelAltColor.BackColor = colorGroup.getColor(true).getColor();
                }
                else
                {
                    uPanelAltColor.BackColor = Color.Gray;
                }
                uRtfExample.BackColor = Settings.get().Colors.ResultsBackground.getColor().getColor();
            }
            writeExampleText();
        }

        /// <summary>
        /// Handles when the main color change button is clicked
        /// </summary>
        private void uBtnChangeColor_Click(object sender, EventArgs e)
        {
            if (uDlgColorPicker.ShowDialog() == DialogResult.OK)
            {
                ColorGroup colorGroup = uListColor.SelectedItem as ColorGroup;
                if (colorGroup != null)
                {
                    colorGroup.getColor().setColor(uDlgColorPicker.Color);
                    refreshDisplay();
                }
            }
        }

        /// <summary>
        /// Handles when the alternate color change button is clicked
        /// </summary>
        private void uBtnChangeAltColor_Click(object sender, EventArgs e)
        {
            if (uDlgColorPicker.ShowDialog() == DialogResult.OK)
            {
                ColorGroup colorGroup = uListColor.SelectedItem as ColorGroup;
                if (colorGroup != null)
                {
                    colorGroup.getColor(true).setColor(uDlgColorPicker.Color);
                    refreshDisplay();
                }
            }

        }

        /// <summary>
        /// Handles when the reset to defaults button is clicked
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            Settings.get().Colors.resetToDefaults();
            uListColor.DataSource = Settings.get().Colors.ColorList;
            uListColor.DisplayMember = "Description";
            refreshDisplay();
        }
    }
}
