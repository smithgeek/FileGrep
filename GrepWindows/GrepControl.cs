using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Smithgeek.Extensions;
using System.IO;

namespace GrepWindows
{
    /// <summary>
    /// This control is the top half of the window and contains all of the search inputs
    /// </summary>
    public partial class GrepControl : UserControl
    {
        /// <summary>
        /// Settings that should be used by the control.  Just an alias for Settings.get()
        /// </summary>
        private Settings mSettings;

        /// <summary>
        /// The search manager to use when kicking off a search.  A new manager should be created each time
        /// the search button is clicked.
        /// </summary>
        private SearchManager mSearchManager;

        /// <summary>
        /// Just a simple directory browser, but Ooki is much better than the built in directory browser in .NET
        /// </summary>
        Ookii.Dialogs.WinForms.VistaFolderBrowserDialog uDirectoryBrowser;

        /// <summary>
        /// Consturctor, sets up data binding and calls init().
        /// </summary>
        public GrepControl()
        {
            InitializeComponent();
            uDirectoryBrowser = new Ookii.Dialogs.WinForms.VistaFolderBrowserDialog();
            setSettings(Settings.get());
        }

        public void setSettings(Settings settings)
        {
            mSettings = settings;
            uPatternList.DataBindings.Clear();
            uPatternList.DataBindings.Add("Text", mSettings.SearchParams, "Pattern", false, DataSourceUpdateMode.OnValidation);
            uScopeList.DataBindings.Clear();
            uScopeList.DataBindings.Add("Text", mSettings.SearchParams, "Scope", false, DataSourceUpdateMode.OnPropertyChanged);
            uScopeList.DataBindings.Add("Enabled", mSettings.SearchParams, "ScopeEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            uIgnoreCase.DataBindings.Clear();
            uIgnoreCase.DataBindings.Add("Checked", mSettings.SearchParams, "IgnoreCase", false, DataSourceUpdateMode.OnPropertyChanged);
            uRegex.DataBindings.Clear();
            uRegex.DataBindings.Add("Checked", mSettings.SearchParams, "UseRegex", false, DataSourceUpdateMode.OnPropertyChanged);
            uSubdirectories.DataBindings.Clear();
            uSubdirectories.DataBindings.Add("Checked", mSettings.SearchParams, "SearchSubdirectories", false, DataSourceUpdateMode.OnPropertyChanged);
            uWholeWord.DataBindings.Clear();
            uWholeWord.DataBindings.Add("Checked", mSettings.SearchParams, "WholeWord", false, DataSourceUpdateMode.OnPropertyChanged);
            uOrderFiles.DataBindings.Clear();
            uOrderFiles.DataBindings.Add("Checked", mSettings.SearchParams, "SortByFile", false, DataSourceUpdateMode.OnPropertyChanged);
            uChFileFind.DataBindings.Clear();
            uChFileFind.DataBindings.Add("Checked", mSettings.SearchParams, "FileNameSearch", false, DataSourceUpdateMode.OnPropertyChanged);
            uDirectoryList.DataBindings.Clear();
            uDirectoryList.DataBindings.Add("Text", mSettings.SearchParams, "SearchDirectory", false, DataSourceUpdateMode.OnValidation);
            uBtnCancel.DataBindings.Clear();
            uBtnCancel.DataBindings.Add("Enabled", mSettings.SearchParams, "SearchInProgress", false, DataSourceUpdateMode.OnPropertyChanged);
            uBtnSearch.DataBindings.Clear();
            uBtnSearch.DataBindings.Add("Enabled", mSettings.SearchParams, "SearchNotInProgress", false, DataSourceUpdateMode.OnPropertyChanged);
            uChListExclusions.DataBindings.Clear();
            uChListExclusions.BindingList = mSettings.SearchParams.Exclusions;
            uNumericContext.DataBindings.Clear();
            uNumericContext.DataBindings.Add("Value", mSettings, "ContextLineCount", false, DataSourceUpdateMode.OnPropertyChanged);
            mSearchManager = new SearchManager();
            init();
        }

        /// <summary>
        /// Reads all the history from settings and populates the drop down lists.
        /// </summary>
        private void init()
        {
            uPatternList.AutoCompleteCustomSource.AddRange(mSettings.PatternHistory.ToArray());
            uPatternList.Items.AddRange(mSettings.PatternHistory.Take(Math.Min(15, mSettings.PatternHistory.Count)).ToArray());

            uScopeList.AutoCompleteCustomSource.AddRange(mSettings.ScopeHistory.ToArray());
            uScopeList.Items.AddRange(mSettings.ScopeHistory.Take(Math.Min(15, mSettings.ScopeHistory.Count)).ToArray());
            if (mSettings.ScopeHistory.Count > 0)
            {
                uScopeList.Text = mSettings.ScopeHistory.First();
            }

            uDirectoryList.Items.AddRange(mSettings.SearchDirHistory.Take(Math.Max(15, mSettings.SearchDirHistory.Count)).ToArray());

            uQuickScopePanel.Controls.Clear();
            foreach (QuickScope scope in mSettings.QuickScopeList)
            {
                if (scope.Display != null && scope.Scope != null)
                {
                    Button btn = new Button();
                    btn.Text = scope.Display.Replace("&", "&&");
                    btn.Tag = scope.Scope;
                    btn.Click += new EventHandler(handleQuickScopeButtonClick);
                    btn.Size = new Size(82, 29);
                    uQuickScopePanel.Controls.Add(btn);
                }
            }

        }

        /// <summary>
        /// Handles when a quick scope button is clicked and updates the textbox.
        /// </summary>
        void handleQuickScopeButtonClick(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control || (ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (!mSettings.SearchParams.Scope.EndsWith(";"))
                {
                    mSettings.SearchParams.Scope += ";";
                }
                mSettings.SearchParams.Scope += ((Button)sender).Tag.ToString();
            }
            else
            {
                mSettings.SearchParams.Scope = ((Button)sender).Tag.ToString();
            }
        }

        /// <summary>
        /// Handles when the browse button is clicked.
        /// </summary>
        private void uBtnBrowse_Click(object sender, EventArgs e)
        {
            showDirectoryBrowser();
        }

        /// <summary>
        /// Shows the directory browser and updates the search directory if ok is clicked on the dialog.
        /// </summary>
        private void showDirectoryBrowser()
        {
            if (uDirectoryBrowser.ShowDialog() == DialogResult.OK)
            {
                uDirectoryList.Text = uDirectoryBrowser.SelectedPath;
            }
        }

        /// <summary>
        /// Watches for key special keystrokes to move focus.  I don't think this is really
        /// used and I will probably delete it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrepControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                if (e.KeyCode == Keys.P)
                {
                    uPatternList.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.E)
                {
                    uScopeList.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.D)
                {
                    uDirectoryList.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.B)
                {
                    showDirectoryBrowser();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                performSearch();
            }
        }

        /// <summary>
        /// Handles the enter key on text boxes so that it doesn't beep!
        /// </summary>
        private void GrepControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles when the search button is clicked.
        /// </summary>
        private void uBtnSearch_Click(object sender, EventArgs e)
        {
            performSearch();
        }

        /// <summary>
        /// Saves search parameters to the history list for later retrieval.
        /// </summary>
        /// <param name="box">The drop down box that will show this history.</param>
        /// <param name="history">The list containing the history for this item.</param>
        /// <param name="text">The current text that is being searched and needs to be saved to history.</param>
        /// <param name="maxList">The maximum number of items to save in history.</param>
        /// <param name="maxAutoComplete">The maximum number of items to keep in the auto complete history.</param>
        private void saveHistory(ComboBox box, List<String> history, String text, int maxList, int maxAutoComplete)
        {
            // Add the text to the autocomplete source if it doesn't already exist.
            if (!box.Items.Contains(text))
            {
                if (box.AutoCompleteSource == AutoCompleteSource.CustomSource)
                {
                    box.AutoCompleteCustomSource.Insert(0, text);
                    if (box.AutoCompleteCustomSource.Count > maxAutoComplete)
                    {
                        box.AutoCompleteCustomSource.RemoveAt(box.AutoCompleteCustomSource.Count - 1);
                    }
                }
            }
            // If the item was already in the dropdown list, remove it and add it back to the top of the list
            else
            {
                box.Items.Remove(text);
            }
            box.Items.Insert(0, text);
            if (box.Items.Count > maxList)
            {
                box.Items.RemoveAt(box.Items.Count - 1);
            }
            box.Text = text;

            // Update the history so the last search result is at the top of the list
            if (history.Contains(text))
            {
                history.Remove(text);
            }
            history.Insert(0, text);
            if (history.Count > maxAutoComplete)
            {
                history.RemoveAt(history.Count - 1);
            }
        }

        /// <summary>
        /// Perform a search.  First save search parameters to the history and then create a search manager
        /// and search.
        /// </summary>
        private void performSearch()
        {
            // We have to update the drop down list text here otherwise the cursor does strange jumping to the begining
            mSettings.SearchParams.Pattern = uPatternList.Text;
            mSettings.SearchParams.Scope = uScopeList.Text;
            mSettings.SearchParams.SearchDirectory = uDirectoryList.Text;

            saveHistory(uPatternList, mSettings.PatternHistory, uPatternList.Text, 15, 50);
            saveHistory(uScopeList, mSettings.ScopeHistory, uScopeList.Text, 15, 50);
            saveHistory(uDirectoryList, mSettings.SearchDirHistory, uDirectoryList.Text, 15, 15);

            mSearchManager.startSearch();
        }

        /// <summary>
        /// Handles when the add exclusion button is clicked.
        /// </summary>
        private void uBtnAddExclusion_Click(object sender, EventArgs e)
        {
            addExclusion();
        }

        /// <summary>
        /// Adds an exclusion that is in the exclusion textbox
        /// to the search parameters and the text list box.  Clears the exclusion textbox
        /// </summary>
        private void addExclusion()
        {
            if (!uTxtExclusions.Text.isEmpty() && !uChListExclusions.Items.Contains(uTxtExclusions.Text))
            {
                mSettings.SearchParams.Exclusions.Add(new CheckBoxModel(uTxtExclusions.Text, true));
            }
            uTxtExclusions.Text = String.Empty;
        }

        /// <summary>
        /// Handles when the remove exclusion button is clicked.
        /// </summary>
        private void uBtnRemoveExlcusion_Click(object sender, EventArgs e)
        {
            removeExclusion();
        }

        /// <summary>
        /// Removes the selected exclusion from the search parameters.
        /// </summary>
        private void removeExclusion()
        {
            if (uChListExclusions.SelectedItem != null)
            {
                mSettings.SearchParams.Exclusions.RemoveAt(uChListExclusions.SelectedIndex);
            }
        }

        /// <summary>
        /// Handles when the cancel button is clicked.
        /// </summary>
        private void uBtnCancel_Click(object sender, EventArgs e)
        {
            mSettings.SearchParams.SearchInProgress = false;
            mSearchManager.cancelSearch();
        }

        /// <summary>
        /// Handles when the enter key is pressed while typing in the exclusion textbox.
        /// </summary>
        private void uTxtExclusions_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addExclusion();
            }
        }

        /// <summary>
        /// Handles when the delete key is pressed when focus is on the exclusions list.
        /// </summary>
        private void uChListExclusions_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                removeExclusion();
            }
        }

        // The point of this is to maintain the caret position.  For some reason .NET will put the
        // caret back to index 0 if it matches something in the autocomplete list.  Complete hack, but 
        // the only way I could figure out how to fix it.
        private static int mCaretPosition = 0;
        private void uScopeList_TextChanged(object sender, EventArgs e)
        {
            if (uScopeList.SelectionStart == 0)
            {
                uScopeList.Select(Math.Min(uScopeList.Text.Length, mCaretPosition), 0);
            }
            else
            {
                mCaretPosition = uScopeList.SelectionStart;
            }
        }
    }
}
