using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;

namespace GrepWindows
{
    /// <summary>
    /// This class should store all of the search parameters.  It inherits from INotifyPropertyChanged so that
    /// you can bind UI controls to each property and have the UI auto update if one changes.
    /// </summary>
    [Serializable]
    public class SearchParameters : BasePropertyChanged
    {
        private SerializableBindingList<CheckBoxModel> mExclusions;
        public SerializableBindingList<CheckBoxModel> Exclusions
        {
            get 
            {
                mExclusions.ListChanged -= new ListChangedEventHandler(mExclusions_ListChanged);
                mExclusions.ListChanged += new ListChangedEventHandler(mExclusions_ListChanged); 
                return mExclusions; 
            }
            set 
            { 
                SetField(ref mExclusions, value, "Exclusions");
                mExclusions.ListChanged += new ListChangedEventHandler(mExclusions_ListChanged);
            }
        }

        void mExclusions_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("Exclusions");
        }

        private String mPattern;
        public String Pattern 
        {
            get { return mPattern; }
            set { SetField(ref mPattern, value, "Pattern"); }
        }

        public String RegexPattern
        {
            get
            {
                String adjustedPattern = Pattern;
                if (UseRegex)
                {
                    if (WholeWord)
                    {
                        adjustedPattern = String.Format("\\b{0}\\b", Pattern);
                    }
                }
                else
                {                        
                    adjustedPattern = Regex.Escape(Pattern);
                    adjustedPattern = Regex.Replace(adjustedPattern, @"(?<!\\\\)\\\*", ".*");
                    adjustedPattern = Regex.Replace(adjustedPattern, @"(?<!\\\\)\\\?", ".");
                    adjustedPattern = Regex.Replace(adjustedPattern, @"\\\\\\\*", @"\*");
                    adjustedPattern = Regex.Replace(adjustedPattern, @"\\\\\\\?", @"\?");

                    if (WholeWord)
                    {
                        adjustedPattern = String.Format("\\b{0}\\b", adjustedPattern);
                    }
                }
                return adjustedPattern;
            }
        }

        private String mScope;
        public String Scope 
        {
            get { return mScope; }
            set { SetField(ref mScope, value, "Scope"); }
        }

        private String mSearchDirectory;
        public String SearchDirectory 
        {
            get { return mSearchDirectory; }
            set { SetField(ref mSearchDirectory, value, "SearchDirectory"); }
        }

        // Options
        private bool mIgnoreCase;
        public bool IgnoreCase 
        {
            get { return mIgnoreCase; }
            set { SetField(ref mIgnoreCase, value, "IgnoreCase"); } 
        }

        private bool mUseRegex;
        public bool UseRegex 
        {
            get { return mUseRegex; }
            set { SetField(ref mUseRegex, value, "UseRegex"); }
        }

        private bool mSearchSubdirectories;
        public bool SearchSubdirectories 
        {
            get { return mSearchSubdirectories; }
            set { SetField(ref mSearchSubdirectories, value, "SearchSubdirectories"); }
        }

        private bool mWholeWord;
        public bool WholeWord 
        {
            get { return mWholeWord; }
            set { SetField(ref mWholeWord, value, "WholeWord"); }
        }

        private bool mSortByFile;
        public bool SortByFile 
        {
            get { return mSortByFile; }
            set { SetField(ref mSortByFile, value, "SortByFile"); }
        }

        private bool mFileNameSearch;
        public bool FileNameSearch 
        {
            get { return mFileNameSearch; }
            set { SetField(ref mFileNameSearch, value, "FileNameSearch"); }
        }

        public bool ScopeEnabled
        {
            get { return !FileNameSearch; }
        }

        private bool mSearchInProgress;
        public bool SearchInProgress
        {
            get { return mSearchInProgress; }
            set 
            {
                SetField(ref mSearchInProgress, value, "SearchInProgress");
            }
        }

        public bool SearchNotInProgress
        {
            get { return !SearchInProgress; }
        }

        public SearchParameters()
        {
            Exclusions = new SerializableBindingList<CheckBoxModel>();
        }

        public SearchParameters(SearchParameters aParams)
        {
            Exclusions = new SerializableBindingList<CheckBoxModel>();
            foreach (CheckBoxModel exclusion in aParams.Exclusions)
            {
                Exclusions.Add(new CheckBoxModel(exclusion.Text, exclusion.Checked));
            }
            Pattern = aParams.Pattern;
            Scope = aParams.Scope;
            SearchDirectory = aParams.SearchDirectory;
            IgnoreCase = aParams.IgnoreCase;
            UseRegex = aParams.UseRegex;
            SearchSubdirectories = aParams.SearchSubdirectories;
            WholeWord = aParams.WholeWord;
            SortByFile = aParams.SortByFile;
            FileNameSearch = aParams.FileNameSearch;
            SearchInProgress = aParams.SearchInProgress;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Exclusions:");
            foreach (CheckBoxModel exclusion in mExclusions)
            {
                builder.AppendLine(String.Format("\t{0}: {1}", exclusion.Text, exclusion.Checked.ToString()));
            }
            builder.AppendLine("Pattern: " + Pattern);
            builder.AppendLine("Scope: " + Scope);
            builder.AppendLine("Search Dir: " + SearchDirectory);
            builder.AppendLine("Ignore Case: " + IgnoreCase.ToString());
            builder.AppendLine("UseRegex: " + UseRegex.ToString());
            builder.AppendLine("Search Subdirectories: " + SearchSubdirectories.ToString());
            builder.AppendLine("Whole Word: " + WholeWord.ToString());
            builder.AppendLine("Sort By File: " + SortByFile.ToString());
            builder.AppendLine("File Name Search: " + FileNameSearch.ToString());
            return builder.ToString();
        }
    }

    [Serializable]
    public class BasePropertyChanged : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        // boiler-plate
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }

}
