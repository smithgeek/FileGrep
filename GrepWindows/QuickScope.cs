using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepWindows
{
    [Serializable]
    public class QuickScope
    {
        public String Scope { get; set; }
        public String Display { get; set; }

        public String DisplayText
        {
            get { return String.Format("{0} ({1})", Display, Scope); }
        }

        public QuickScope()
        {
        }

        public QuickScope(String scope, String display)
        {
            Scope = scope;
            Display = display;
        }
    }
}
