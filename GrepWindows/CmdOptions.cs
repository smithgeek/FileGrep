using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using System.IO;

namespace GrepWindows
{
    class CmdOptions
    {
        [Option('o', "oneInstance", HelpText = "Don't open a new instance if one is already open.")]
        public bool OneInstance { get; set; }

        [Option('p', "pattern", HelpText = "Set the search pattern")]
        public string Pattern { get; set; }

        [Option('d', "directory", HelpText = "Set the directory to search")]
        public string Directory { get; set; }

        public CmdOptions()
        {
            OneInstance = false;
            Pattern = string.Empty;
            Directory = string.Empty;
        }
    }
}
