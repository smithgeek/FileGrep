using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandLine;
using System.Threading;

namespace GrepWindows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            CmdOptions options = new CmdOptions();
			Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(o =>
			{
				options = o;
			});

            bool mutexCreated = true;
            using (Mutex mutex = new Mutex(true, "GrepWindowsMutex", out mutexCreated))
            {
                if (mutexCreated || !options.OneInstance)
                {
                    if (mutexCreated)
                    {
                        NamedPipes.CreatePipe();
                    }
                    Application.Run(new Form1(options.Directory.Trim(new char[]{'\"'}), options.Pattern));
                }
                else
                {
                    if(options.Pattern != String.Empty)
                    {
                        NamedPipes.SendSearchPattern(options.Directory.Trim(new char[] { '\"' }), options.Pattern);
                    }
                }
            }
        }
    }
}
