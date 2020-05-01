//#define UPDATER_SUPPORTED
//#define ANALYTICS_SUPPORTED
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Smithgeek.IO;


#if UPDATER_SUPPORTED
using GarminUpdater;
    [assembly:AssemblyGarminUpdateInformation(@"\\OLA-CJL2HS1.ad.garmin.com\Shared\Release\Tools\Grep", "Grep", "Barools", 0.43)]
#endif

namespace GrepWindows
{
    public partial class Form1 : Form
    {
        #if ANALYTICS_SUPPORTED
            /// <summary>
            /// Writes analytics once a week 
            /// </summary>
            private Analytics.Analytics mAnalytics;
        #endif

        /// <summary>
        /// Periodically we will check for updates in case someone never closes grep.
        /// </summary>
        private bool mShouldCheckForUpdate;

        /// <summary>
        /// Construction, set up some defaults and initialize other components.
        /// </summary>
        /// <param name="startDirectory">Allows the program to be opened with a start directory given.</param>
        /// <param name="startPattern">Allows the program to be opened with a start search pattern given.</param>
        public Form1(String startDirectory, String startPattern)
        {
            InitializeComponent();
            Logger.get();
            mShouldCheckForUpdate = false;
            #if UPDATER_SUPPORTED
                Updater.Check();
                this.Text += " - " + Updater.getUpdateInfo().Version.ToString("F2");
            #endif
            checkSettings();
            TabManager.get().init(tabControl1);
            NamedPipes.SearchPatternReceived += new NamedPipes.SearchPatternCallback(mNamedPipe_SearchPatternReceived);
            writePathToAppData();
            TabManager.get().connectKeyShortcuts(this);
            // This looks unecessary (and really is), but it will speed up the very first search
            // after the program is opened.  It seems like .NET doesn't load something in PLINQ 
            // until it is first needed.
            List<String> unecessary = new List<string>();
            unecessary.AsParallel().ForAll(res => { res = String.Empty; });
        }

        /// <summary>
        /// Checks for saved settings and restores if they are found.  Also does some analytics.
        /// </summary>
        private void checkSettings()
        {
            if ((DateTime.Now - Settings.get().AnalyticsDate).Days > 5)
            {
                Settings.get().AnalyticsDate = DateTime.Now;
                #if ANALYTICS_SUPPORTED
                    mAnalytics = new Analytics.Analytics("Grep", @"\\OLA-CJL2HS1.ad.garmin.com\Stats\Grep", Updater.getUpdateInfo().Version.ToString("F2"));
                #endif
            }
            this.Width = Settings.get().WindowWidth;
            this.Height = Settings.get().WindowHeight;
            if (Settings.get().WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// Writes out the path to the grep executable to the app data folder.  Used in the VS plugin to find the exe.
        /// </summary>
        private void writePathToAppData()
        {
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Barools\Grep\path.txt");
            DirectoryExt.Create(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8));
        }

        /// <summary>
        /// Handles an event from a named pipe when a search pattern is received
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <param name="pattern">The pattern to search.</param>
        void mNamedPipe_SearchPatternReceived(string directory, string pattern)
        {
            if (uGrepControl.InvokeRequired)
            {
                Invoke(new MethodInvoker(() => 
                {
                    Settings.get().SearchParams.Pattern = pattern;
                    Settings.get().SearchParams.SearchDirectory = directory;
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Normal;
                    this.Focus();
                    this.BringToFront();
                    this.TopMost = false;
                }));
            }
            else
            {
                Settings.get().SearchParams.Pattern = pattern;
            }
        }

        /// <summary>
        /// When the form is closing we need to save the window state, height, and width so we can open 
        /// at the same window size.
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.get().WindowHeight = this.Height;
                Settings.get().WindowWidth = this.Width;
            }
            Settings.get().WindowState = this.WindowState;
            Settings.get().Serialize();
        }

        /// <summary>
        /// Handle when the settings menu item is clicked.
        /// </summary>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings form = new FrmSettings(Settings.get());
            form.ShowDialog();
            uGrepControl.setSettings(Settings.get());
        }

        /// <summary>
        /// Handles when the exit menu item is clicked, closes the program.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Handles when the update timer fires, this means we will check for an update the next
        /// time the window gains focus.
        /// </summary>
        private void uUpdaterCheck_Tick(object sender, EventArgs e)
        {
            mShouldCheckForUpdate = true;
        }

        /// <summary>
        /// The window was activated so we might need to check for a software update.
        /// </summary>
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (mShouldCheckForUpdate)
            {
                Settings.get().Serialize();
                #if UPDATER_SUPPORTED
                    Updater.Check();
                #endif
                mShouldCheckForUpdate = false;
            }
            GC.Collect();
            DirectoryCache.get().update(DirectoryCache.UpdateType.Automatic, false);
        }

        /// <summary>
        /// Handle when the make a suggestion menu item is clicked
        /// </summary>
        private void reportIssueMakeSuggestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("mailto:Brent.Smith@garmin.com?subject=Grep Issue/Request&body=My software version is%20" + 
                #if UPDATER_SUPPORTED
                    Updater.getUpdateInfo().Version.ToString("F2") +
                #endif
                ".");
        }

        /// <summary>
        /// Handle when the wiki page menu item is clicked.
        /// </summary>
        private void wikiPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://confluence.garmin.com:8453/display/~smithbr/Grep");
        }

        /// <summary>
        /// Handle when the form looses focus, force garbage collection.
        /// </summary>
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            GC.Collect();
        }

        /// <summary>
        /// Handles when the debug menu option is clicked.
        /// </summary>
        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Logger.get().showDebugWindow();
        }

        /// <summary>
        /// Handles when the regex help tip menu option is clicked.
        /// </summary>
        private void regularExpressionTipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegexHelp form = new RegexHelp();
            form.Show();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Settings files|*.dat";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.get().Serialize(dialog.FileName);
            }
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Settings files|*.dat";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.loadNewSettings(dialog.FileName);
                uGrepControl.setSettings(Settings.get());
            }
        }
    }
}
