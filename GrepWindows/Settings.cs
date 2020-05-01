using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Smithgeek.IO;
using Smithgeek;
using System.Windows.Forms;

namespace GrepWindows
{
    [Serializable]
    public class Settings
    {
        #if(DEBUG)
            private static readonly String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Barools\Grep\settingstest.dat");
        #else
            //private static readonly String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Barools\Grep\settingstest.dat");
            private static readonly String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Barools\Grep\settings.dat");
        #endif
        
        private static int CURRENT_VERSION = 8;

        public enum BuiltInExternalApps
        {
            VISUAL_STUDIO,
            NOTEPAD_PLUS_PLUS,
            CUSTOM,
        }

        public SearchParameters SearchParams { get; set; }
        public List<String> PatternHistory { get; set; }
        public List<String> ScopeHistory { get; set; }
        public List<String> SearchDirHistory { get; set; }
        public List<String> ExculsionList { get; set; }
        public Dictionary<String, bool> ExclusionDictionary { get; set; }
        public List<QuickScope> QuickScopeList { get; set; }
        public bool IgnoreCase { get; set; }
        public bool Regex { get; set; }
        public bool Subdirectories { get; set; }
        public bool OpenNewTab { get; set; }
        public String ExternalApplication { get; set; }
        public String FileArgs { get; set; }
        public String WithLineNumberArgs { get; set; }
        public int Version { get; set; }
        public DateTime AnalyticsDate { get; set; }
        public bool WholeWord { get; set; }
        public bool SortByFile { get; set; }
        public bool FileNameSearch { get; set; }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public FormWindowState WindowState { get; set; }
        public BuiltInExternalApps ExternalAppType { get; set; }

        public int ContextLineCount { get; set; }
        public ColorSettings Colors { get; set; }


        private static Settings sSettings = null;

        public Settings()
        {
            Version = CURRENT_VERSION;
            SearchParams = new SearchParameters();
            PatternHistory = new List<string>();
            ScopeHistory = new List<string>();
            SearchDirHistory = new List<string>();
            ExclusionDictionary = new Dictionary<string, bool>();
            ExclusionDictionary[@"*\boost\*"] = true;
            ExclusionDictionary[@"*\_Output\*"] = true;
            ExclusionDictionary[@"*\.git\*"] = true;
            QuickScopeList = new List<QuickScope>();
            QuickScopeList.Add(new QuickScope("*.?pp", "C++ files"));
            QuickScopeList.Add(new QuickScope("*.c;*.h;", "C files"));
            QuickScopeList.Add(new QuickScope("*.?pp;*.c;*.h", "C & C++"));
            QuickScopeList.Add(new QuickScope("*.hpp;*.h", "Headers"));
            QuickScopeList.Add(new QuickScope("*", "All"));
            ExternalApplication = "c:\\Program Files (x86)\\Notepad++\\notepad++.exe";
            FileArgs =  "\"{file}\"";
            WithLineNumberArgs = "\"{file}\" -n{line}";
            OpenNewTab = false;

            IgnoreCase = false;
            Regex = false;
            Subdirectories = true;

            WindowWidth = 1167;
            WindowHeight = 715;
            WindowState = FormWindowState.Normal;

            ExternalAppType = BuiltInExternalApps.CUSTOM;
            ContextLineCount = 0;
            Colors = new ColorSettings();
        }

        public void Serialize()
        {
            Serialize(filePath);
        }

        public void Serialize(String path)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    formatter.Serialize(stream, this);
                }
            }
            catch(Exception ex)
            {
                Logger.get().AddError(ex.Message);
            }
        }

        private static Settings Deserialize(String path)
        {
            Settings settings = new Settings();
            bool isBackupFile = Path.GetExtension(path) == ".backup";
            try
            {
                if (File.Exists(path))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        stream.Position = 0;
                        settings = (Settings)formatter.Deserialize(stream);
                        // If we got here then the file is probably ok.  Make a backup.
                        if (!isBackupFile)
                        {
                            FileExt.Copy(path, path + ".backup", NameConflictOption.Overwrite);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.get().AddError(ex.Message);
                if (!isBackupFile)
                {
                    settings = Deserialize(path + ".backup");
                }
            }
            for (int i = settings.Version; i < CURRENT_VERSION; i++)
            {
                switch (i)
                {
                    case 1:
                        settings.AnalyticsDate = new DateTime(0);
                        break;

                    case 2:
                    case 3:
                        //obsoleted code
                        break;

                    case 4: // added for .26
                        settings.SearchParams = new SearchParameters();
                        settings.SearchParams.IgnoreCase = settings.IgnoreCase;
                        settings.SearchParams.UseRegex = settings.Regex;
                        settings.SearchParams.SearchSubdirectories = settings.Subdirectories;
                        settings.SearchParams.WholeWord = settings.WholeWord;
                        settings.SearchParams.SortByFile = settings.SortByFile;
                        settings.SearchParams.FileNameSearch = settings.FileNameSearch;
                        foreach (KeyValuePair<String, bool> exclusion in settings.ExclusionDictionary)
                        {
                            settings.SearchParams.Exclusions.Add(new CheckBoxModel(exclusion.Key, exclusion.Value));
                        }
                        if (settings.PatternHistory.Count > 0)
                        {
                            settings.SearchParams.Pattern = settings.PatternHistory.First();
                        }
                        if (settings.ScopeHistory.Count > 0)
                        {
                            settings.SearchParams.Scope = settings.ScopeHistory.First();
                        }
                        if (settings.SearchDirHistory.Count > 0)
                        {
                            settings.SearchParams.SearchDirectory = settings.SearchDirHistory.First();
                        }
                        break;

                    case 5:
                        settings.ContextLineCount = 0;
                        break;

                    case 6:
                        settings.Colors = new ColorSettings();
                        break;

                    case 7:
                        settings.Colors.ResultsBackground = new ColorGroup("Results Background", new RtfColor(System.Drawing.Color.White, -1));
                        settings.Colors.ColorList.Add(settings.Colors.ResultsBackground);
                        break;

                    default:
                        break;
                }
            }
            // Update the current version and save the upgraded settings.
            settings.Version = CURRENT_VERSION;
            settings.SearchParams.SearchInProgress = false;
            settings.Serialize();
            return settings;
        }

        public static Settings get()
        {
            return get(filePath);
        }

        public static Settings loadNewSettings(String path)
        {
            Smithgeek.IO.FileExt.Copy(path, filePath, NameConflictOption.Overwrite);
            sSettings = Deserialize(filePath);
            return sSettings;
        }

        public static Settings get(String path)
        {
            if (sSettings == null)
            {
                sSettings = Deserialize(path);
            }
            return sSettings;
        }
    }
}
