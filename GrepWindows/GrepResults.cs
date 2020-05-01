using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GrepWindows
{
    public partial class GrepResults : UserControl
    {
        /// <summary>
        /// Events that the results control can send
        /// </summary>
        public enum Event
        {
            CLOSE_TAB,
            NEXT_TAB,
            PREVIOUS_TAB,
            NEW_TAB
        }

        /// <summary>
        /// Result event signal delegate
        /// </summary>
        /// <param name="aEvent">The event to send</param>
        public delegate void ResultsEventSignal(Event aEvent);

        /// <summary>
        /// The result event handler
        /// </summary>
        public event ResultsEventSignal ResultEvents;

        /// <summary>
        /// Maintain the directory was searched so that we can send it to external applications
        /// when a line is double clicked.  (Jeffrey wanted this for some crazy vim thing)
        /// </summary>
        public String SearchedDirectory { get; set; }

        /// <summary>
        /// A list of files that were skipped for some reason.
        /// </summary>
        private List<String> mSkippedFiles;

        /// <summary>
        /// Construction
        /// </summary>
        public GrepResults()
        {
            InitializeComponent();
            TextBox.BackColor = Settings.get().Colors.ResultsBackground.getColor().getColor();
        }

        /// <summary>
        /// Updates the status information. Number of files searched, number of results...
        /// </summary>
        /// <param name="aSearchStats"></param>
        public void updateStatus(SearchStats aSearchStats)
        {
            uStatus.Text = aSearchStats.StatusText;
            uTime.Text = aSearchStats.ElapsedTime;
            if (aSearchStats.Finished)
            {
                uSkipped.Visible = aSearchStats.SkippedFiles.Count > 0;
                mSkippedFiles = new List<string>(aSearchStats.SkippedFiles);
            }
        }

        /// <summary>
        /// Replace any variables before sending the command to an external application.
        /// </summary>
        /// <param name="cmd">The command to be sent</param>
        /// <param name="lineNumber">The line number to be sent</param>
        /// <param name="file">The file to be sent</param>
        /// <returns>String with replaced arguments.</returns>
        private String replaceVariables(String cmd, int lineNumber, String file)
        {
            if (lineNumber >= 0)
            {
                return cmd.Replace("{line}", lineNumber.ToString()).Replace("{file}", file).Replace("{searchDir}", SearchedDirectory);
            }
            else
            {
                return cmd.Replace("{line}", "").Replace("{file}", file).Replace("{searchDir}", SearchedDirectory);
            }
        }

        /// <summary>
        /// Handles when someone double clicks in the results text box.  Sends a message to an external application.
        /// </summary>
        private void TextBox_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            int rowIndex = TextBox.GetLineFromCharIndex(TextBox.SelectionStart);
            if (rowIndex >= 0 && rowIndex < TextBox.Lines.Length && TextBox.Lines[rowIndex].Length > 0)
            {
                if (Char.IsDigit(TextBox.Lines[rowIndex][0]))
                {
                    int lineNumber = Int32.Parse(TextBox.Lines[rowIndex].Substring(0, TextBox.Lines[rowIndex].IndexOf(":")));
                    for (int i = rowIndex - 1; i >= 0; i--)
                    {
                        if (!Char.IsDigit(TextBox.Lines[i][0]))
                        {
                            try
                            {
                                if(Settings.get().ExternalAppType == Settings.BuiltInExternalApps.VISUAL_STUDIO)
                                {
                                    NamedPipes.SendFileAndLine(lineNumber.ToString() + "?" + TextBox.Lines[i]);
                                }
                                else
                                {
                                    Process.Start(Settings.get().ExternalApplication, replaceVariables(Settings.get().WithLineNumberArgs, lineNumber, TextBox.Lines[i]));
                                }
                            }
                            catch(Exception ex)
                            {
                                Logger.get().AddError(ex.Message);
                                MessageBox.Show(ex.Message);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    String filepath = TextBox.Lines[rowIndex];
                    if (!File.Exists(filepath))
                    {
                        filepath = Path.Combine(SearchedDirectory, filepath);
                    }
                    if (File.Exists(filepath))
                    {
                        try
                        {
                            if (Settings.get().ExternalAppType == Settings.BuiltInExternalApps.VISUAL_STUDIO)
                            {
                                NamedPipes.SendFileAndLine("?" + filepath);
                            }
                            else
                            {
                                Process.Start(Settings.get().ExternalApplication, replaceVariables(Settings.get().FileArgs, -1, filepath));
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.get().AddError(ex.Message);
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles when the skipped file list link is clicked.  Opens a new window with a list of skipped files.
        /// </summary>
        private void uSkipped_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmSkippedFiles form = new FrmSkippedFiles(mSkippedFiles);
            form.Show();
        }

        /// <summary>
        /// Handles special key characters to switch or close tabs.
        /// </summary>
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (ResultEvents != null)
            {
                if ((e.KeyCode == Keys.W || e.KeyCode == Keys.F4) && e.Control)
                {
                    ResultEvents(Event.CLOSE_TAB);
                }
                else if (e.KeyCode == Keys.Tab && e.Control)
                {
                    if (e.Shift)
                    {
                        ResultEvents(Event.PREVIOUS_TAB);
                    }
                    else
                    {
                        ResultEvents(Event.NEXT_TAB);
                    }
                }
                else if (e.KeyCode == Keys.T && e.Control)
                {
                    ResultEvents(Event.NEW_TAB);
                }
            }
        }

        /// <summary>
        /// Handles when the X button on the find panel is clicked.  Closes the find panel.
        /// </summary>
        private void uBtnCloseFind_Click(object sender, EventArgs e)
        {
            showFindPanel(false);
        }

        /// <summary>
        /// Shows or hides the find panel.
        /// </summary>
        /// <param name="show">If the panel should be shown or hidden.</param>
        public void showFindPanel(bool show)
        {
            if (show)
            {
                if (!uPanelFind.Visible)
                {
                    uPanelFind.Visible = true;
                    TextBox.Height -= uPanelFind.Height;
                }
                uTxtFindString.Focus();
            }
            else
            {
                if (uPanelFind.Visible)
                {
                    TextBox.Height += uPanelFind.Height;
                    uPanelFind.Visible = false;
                }
            }
        }

        /// <summary>
        /// Handles the find next button.  Finds the next result.
        /// </summary>
        private void uBtnFindNext_Click(object sender, EventArgs e)
        {
            search();
        }

        /// <summary>
        /// Searches the result window for some text.
        /// </summary>
        private void search()
        {
            String textToFind = uTxtFindString.Text;
            RichTextBoxFinds options = RichTextBoxFinds.NoHighlight;
            if (uChFindMatchCase.Checked)
            {
                options |= RichTextBoxFinds.MatchCase;
            }
            if (uChFindWholeWord.Checked)
            {
                options |= RichTextBoxFinds.WholeWord;
            }
            if (uChFindSearchUp.Checked)
            {
                options |= RichTextBoxFinds.Reverse;
            }

            int start = 0;
            int end = TextBox.Text.Length;

            if (uChFindSearchUp.Checked && TextBox.SelectionStart != -1)
            {
                end = TextBox.SelectionStart;
            }
            else if (!uChFindSearchUp.Checked)
            {
                start = TextBox.SelectionStart + TextBox.SelectionLength;
            }

            int foundStart = TextBox.Find(textToFind, start, end, options);
            if (foundStart == -1)
            {
                start = 0;
                foundStart = TextBox.Find(textToFind, start, TextBox.Text.Length, options);
            }
            if (foundStart >= 0)
            {
                TextBox.Select(foundStart, textToFind.Length);
            }
            else
            {
                TextBox.SelectionLength = 0;
            }
        }

        /// <summary>
        /// Watches the find window textbox for enter so that it can start searching.
        /// </summary>
        private void uTxtFindString_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                search();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handle the enter key so it doesn't beep, I found it really annoying.
        /// </summary>
        private void uTxtFindString_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Watch the results window for Ctrl+F and open the find panel
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.F && e.Control)
            {
                showFindPanel(true);
            }
        }

        private void uBtnReplace_Click(object sender, EventArgs e)
        {
            if (TextBox.SelectionStart >= 0 && TextBox.SelectionLength >= 0 && TextBox.SelectedText.Equals(uTxtFindString.Text, uChFindMatchCase.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
            {
                int rowIndex = TextBox.GetLineFromCharIndex(TextBox.SelectionStart);
                String lineNumberStr = String.Empty;
                for (int charIndex = 0; charIndex < TextBox.Lines[rowIndex].Length && Char.IsDigit(TextBox.Lines[rowIndex][charIndex]); charIndex++)
                {
                    lineNumberStr += TextBox.Lines[rowIndex][charIndex];
                }
                if (lineNumberStr == String.Empty)
                {
                    MessageBox.Show("Sorry, you can't modify filenames using find replace.");
                }
                else
                {
                    int lineNumber = Int32.Parse(lineNumberStr) - 1;
                    int fileLineNumber = rowIndex;
                    while (fileLineNumber >= 0 && Char.IsDigit(TextBox.Lines[fileLineNumber][0]))
                    {
                        fileLineNumber--;
                    }

                    String filePath = TextBox.Lines[fileLineNumber];
                    if (File.Exists(filePath))
                    {
                        String[] lines = File.ReadAllLines(filePath);
                        int startPosition = TextBox.SelectionStart - lineNumberStr.Length - TextBox.GetFirstCharIndexFromLine(rowIndex) - 2;
                        String newLine = lines[lineNumber].Remove(startPosition, TextBox.SelectionLength);
                        newLine = newLine.Insert(startPosition, uTxtReplaceString.Text);
                        lines[lineNumber] = newLine;
                        File.WriteAllLines(filePath, lines);
                    }

                    // We have to temporarily change readonly in case the replacement string is an empty string.
                    // For some reason .NET won't let us change the selected text to "" if readonly is true???
                    TextBox.ReadOnly = false;
                    TextBox.SelectedText = uTxtReplaceString.Text;
                    TextBox.ReadOnly = true;
                    search();
                }
            }
            else
            {
                search();
            }
        }
    }
}
