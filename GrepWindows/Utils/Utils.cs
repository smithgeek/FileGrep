using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Smithgeek
{
    using System.Net.Mail;
    using Smithgeek.Extensions;
    using Smithgeek.IO;
    using System.IO;
    public static class Utils
    {
        public static void KillTask(String ProcessName)
        {
            RunCmd("taskkill", "/IM " + ProcessName, true);
        }

        public static void OutputEmbededResource(String destinationDirectory, String filename, bool overwrite)
        {
            Assembly assembly = System.Reflection.Assembly.GetCallingAssembly();
            OutputEmbededResource(destinationDirectory, assembly, filename, overwrite);
        }

        public static void OutputEmbededResource(String destinationDirectory, Assembly assembly, String filename, bool overwrite)
        {
            
            String outputFilePath = PathExt.Combine(destinationDirectory, filename);
            if (File.Exists(outputFilePath) && !overwrite)
                return;

            String[] names = assembly.GetManifestResourceNames();
            String theResource = String.Empty;
            foreach (String name in names)
            {
                if (name.EndsWith(filename))
                {
                    theResource = name;
                    break;
                }
            }
            if (theResource == String.Empty)
                return;
            System.IO.Stream manifestStream = assembly.GetManifestResourceStream(theResource);
            using (System.IO.FileStream fileStream = new System.IO.FileStream(outputFilePath, System.IO.FileMode.OpenOrCreate))
            {
                if (null != manifestStream)
                {
                    int BUFFER_SIZE = (int)manifestStream.Length;
                    byte[] buffer = new byte[BUFFER_SIZE];
                    manifestStream.Read(buffer, 0, BUFFER_SIZE);
                    fileStream.Write(buffer, 0, BUFFER_SIZE);
                    manifestStream.Close();
                }
            }
        }

        public static int RunCmd(String cmd, String args, bool wait)
        {
            return RunCmd(cmd, args, null, wait);
        }

        public static int RunCmd(String cmd, String args, String outputFile, bool wait)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo(cmd);
                procStartInfo.Arguments = args;
                Process proc = new Process();
                if (wait)
                {
                    bool useOutputFile = null != outputFile && String.Empty != outputFile;
                    String output = String.Empty;
                    if (useOutputFile)
                    {
                        procStartInfo.UseShellExecute = false;
                        procStartInfo.RedirectStandardOutput = true;
                    }
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                    if (useOutputFile)
                    {
                        output = proc.StandardOutput.ReadToEnd();
                    }
                    proc.WaitForExit();
                    if (useOutputFile)
                    {
                        FileExt.Write(outputFile, output, false);
                    }
                    return proc.ExitCode;
                }
                else
                {
                    proc = Process.Start(procStartInfo);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static String getExecutingAssemblyPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6).endDir();
        }
    }

}

namespace Smithgeek.Bits
{
    public static class Bits
    {
        public static bool IsBitSet(this byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        public static bool IsBitSet(this UInt16 val, int pos)
        {
            return (val & (1 << pos)) != 0;
        }

        public static bool IsBitSet(this UInt32 val, int pos)
        {
            return (val & (1 << pos)) != 0;
        }

    }
}