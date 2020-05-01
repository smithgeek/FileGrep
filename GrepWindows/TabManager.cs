using System;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GrepWindows
{
    class TabManager
    {
        /// <summary>
        /// Singleton instance of the tab manager.
        /// </summary>
        private static TabManager sTabManager = null;

        /// <summary>
        /// The tab control that the manager manages.
        /// </summary>
        TabControl mTabControl;

        /// <summary>
        /// Get the single instance of the tab manager.
        /// </summary>
        /// <returns>The single instance of the tab manager.</returns>
        public static TabManager get()
        {
            if (sTabManager == null)
            {
                sTabManager = new TabManager();
            }
            return sTabManager;
        }

        /// <summary>
        /// Initialize the tab manager.
        /// </summary>
        /// <param name="aTabControl">The tab control that the tab manager should manage.</param>
        public void init(TabControl aTabControl)
        {
            mTabControl = aTabControl;
            mTabControl.Selected += new TabControlEventHandler(mTabControl_Selected);
            mTabControl.MouseClick += new MouseEventHandler(mTabControl_MouseClick);
            ((GrepResults)mTabControl.TabPages[0].Controls[0]).ResultEvents += new GrepResults.ResultsEventSignal(aResultsControl_ResultEvents);
        }


        /// <summary>
        /// Creates a new tab if the setting is desired and returns a grep results control.
        /// </summary>
        /// <returns>Grep Results control that can be used for displaying results.</returns>
        public GrepResults getControlForNewResults()
        {
            if (Settings.get().OpenNewTab && ((GrepResults)mTabControl.SelectedTab.Controls[0]).TextBox.Text != String.Empty)
            {
                AddTab();
            }
            GrepResults results = (GrepResults)mTabControl.SelectedTab.Controls[0];
            results.TextBox.Text = "";
            results.SearchedDirectory = Settings.get().SearchParams.SearchDirectory;
            mTabControl.SelectedTab.Text = Settings.get().SearchParams.Pattern;
            return results;
        }

        /// <summary>
        /// Creates a new tab and sets up a results control on it.
        /// </summary>
        public void AddTab()
        {
            mTabControl.TabPages.Insert(mTabControl.TabPages.Count - 1, new TabPage("Results"));
            mTabControl.SelectedTab = mTabControl.TabPages[mTabControl.TabPages.Count - 2];
            GrepResults results = new GrepResults();
            results.ResultEvents += new GrepResults.ResultsEventSignal(aResultsControl_ResultEvents);
            results.Dock = DockStyle.Fill;
            mTabControl.SelectedTab.Controls.Add(results);
        }

        /// <summary>
        /// Handle events from the results window
        /// </summary>
        /// <param name="aEvent">The event that was sent.</param>
        void aResultsControl_ResultEvents(GrepResults.Event aEvent)
        {
            switch (aEvent)
            {
                case GrepResults.Event.CLOSE_TAB:
                    mTabControl.TabPages.Remove(mTabControl.SelectedTab);
                    break;

                case GrepResults.Event.NEXT_TAB:
                    if (mTabControl.SelectedIndex < mTabControl.TabPages.Count - 2)
                    {
                        mTabControl.SelectedIndex += 1;
                    }
                    else
                    {
                        mTabControl.SelectedIndex = 0;
                    }
                    break;

                case GrepResults.Event.PREVIOUS_TAB:
                    if (mTabControl.SelectedIndex > 0)
                    {
                        mTabControl.SelectedIndex -= 1;
                    }
                    else
                    {
                        mTabControl.SelectedIndex = mTabControl.TabPages.Count - 2;
                    }
                    break;

                case GrepResults.Event.NEW_TAB:
                    AddTab();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handles when a new tab is selected.  If it the new tab is the "+" tab create a new tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mTabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == "addTab")
            {
                AddTab();
            }
        }

        /// <summary>
        /// Handle when the mouse clicks a tab.  If it is the middle mouse button close the tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mTabControl_MouseClick(object sender, MouseEventArgs e)
        {
            var tabControl = sender as TabControl;
            var tabs = tabControl.TabPages;

            if (e.Button == MouseButtons.Middle)
            {
                TabPage theTab = tabs.Cast<TabPage>()
                        .Where((t, i) => tabControl.GetTabRect(i).Contains(e.Location) && t.Name != "addTab")
                        .FirstOrDefault();
                if (theTab != null)
                {
                    if (tabControl.TabCount > 2)
                    {
                        tabs.Remove(theTab);
                    }
                    else
                    {
                        ((GrepResults)theTab.Controls[0]).TextBox.Text = String.Empty;
                        theTab.Text = "Results";
                        GC.Collect();
                    }
                }
            }
        }

        /// <summary>
        /// Recursively connects all child elements to the ctrl+T shortcut.
        /// </summary>
        /// <param name="parentControl">The parent control that contains all children to connect.</param>
        public void connectKeyShortcuts(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control.Controls.Count > 0)
                {
                    connectKeyShortcuts(control);
                }
                control.KeyUp += new KeyEventHandler(checkForNewTab);
                control.KeyPress += new KeyPressEventHandler(control_KeyPress);
            }
        }

        /// <summary>
        /// Handle the key press so we can get rid of the beep.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)20)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Checks for a ctrl+T shortcut and opens a new tab if found.  Also restores selction state for
        /// combo boxes and text boxes.
        /// </summary>
        private void checkForNewTab(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.T && e.Control)
            {
                int selectionStart = 0;
                int selectionLength = 0;
                ComboBox combo = sender as ComboBox;
                TextBox textbox = sender as TextBox;
                if (combo != null)
                {
                    selectionStart = combo.SelectionStart;
                    selectionLength = combo.SelectionLength;
                }
                else if (textbox != null)
                {
                    selectionLength = textbox.SelectionLength;
                    selectionStart = textbox.SelectionStart;
                }
                TabManager.get().AddTab();
                e.Handled = true;
                ((Control)sender).Focus();
                if (combo != null)
                {
                    combo.SelectionStart = selectionStart;
                    combo.SelectionLength = selectionLength;
                }
                else if (textbox != null)
                {
                    textbox.SelectionStart = selectionStart;
                    textbox.SelectionLength = selectionLength;
                }
            }
        }

    }
}
