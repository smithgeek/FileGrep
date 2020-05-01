using System;
using System.Text.RegularExpressions;

namespace GrepWindows
{
	/// <summary>
	/// Stubbed out log class 
	/// </summary>
	public class Log
	{
		public Log(string path)
		{

		}

		public bool LoggingEnabled { get; set; }

		public virtual string handleCommand(string command)
		{
			return string.Empty;
		}

		public void AddInfo(string message)
		{

		}

		public void AddInfoFormat(String format, params object[] args)
		{

		}

		public void AddData(Object obj, String objId, String info)
		{
		}

		public void AddDataFormat(Object obj, String objId, String format, params object[] args)
		{
		}

		public void AddError(string message)
		{
		}

		public void AddErrorFormat(String format, params object[] args)
		{

		}
	}

    /// <summary>
    /// Logger specific for the grep app. 
    /// </summary>
    class Logger : Log
    {
        /// <summary>
        /// Singleton instance of logger.
        /// </summary>
        private static Logger sLog = null;

        /// <summary>
        /// Constructor doesn't really do anything.
        /// </summary>
        /// <param name="path"></param>
        public Logger(String path)
            : base(path)
        {
        }

        /// <summary>
        /// Gets the singleton instance of logger
        /// </summary>
        /// <returns>Singleton instance of logger.</returns>
        public static Logger get()
        {
            if(sLog == null)
            {
                sLog = new Logger(@"C:\Projects\Trash\output.log");
                sLog.LoggingEnabled = false;
                //sLog.LogOutput = Log.Output.DebugWindow;
                //sLog.StackFrameIgnoreFiles.Add("Logger.cs");
            }
            return sLog;
        }

        /// <summary>
        /// Handles commands from the debug window that the default logger doesn't know about.
        /// </summary>
        /// <param name="command">The command entered.</param>
        /// <returns>Output text to print to the debug window.</returns>
        public override string handleCommand(string command)
        {
            Regex parser = new Regex(@"[^\s""]+|""[^""]*""");
            MatchCollection matches = parser.Matches(command);

            String output;
            switch (matches[0].Value)
            {
                case "clearCache":
                    DirectoryCache.get().update(DirectoryCache.UpdateType.Full, true);
                    output = "Directory cache cleared.";
                    break;

                case "track":
                    if (matches.Count == 2)
                    {
                        //Updater.WriteTrackFile(matches[1].Value);
                        output = "Track changed";
                    }
                    else
                    {
                        output = "I don't know what you are trying to do.";
                    }
                    break;

                case "help_ext":
                    output = "clearCache - clears the cached file list\r\ntrack name - sets the update track for this app";
                    break;

                default:
                    output = base.handleCommand(command);
                    break;
            }
            return output;
        }
    }
}
