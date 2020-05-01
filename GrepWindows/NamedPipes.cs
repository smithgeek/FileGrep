using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO.Pipes;
using System.IO;

namespace GrepWindows
{
    /// <summary>
    /// Handles interaction between app and visual studio plugin.
    /// </summary>
    internal class NamedPipes
    {
        /// <summary>
        /// Delegate for when search parameters are received from an external source.
        /// </summary>
        /// <param name="directory">Directory to search.</param>
        /// <param name="pattern">Pattern to search.</param>
        public delegate void SearchPatternCallback(String directory, String pattern);

        /// <summary>
        /// Event sent when external search parameters are received.
        /// </summary>
        public static event SearchPatternCallback SearchPatternReceived;

        /// <summary>
        /// Creates a background worker that will create the named pipe.
        /// </summary>
        internal static void CreatePipe()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Creates a new thread for the pipe to run on.
        /// </summary>
        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            CreatePipeThread();
        }

        /// <summary>
        /// Creates a named pipe server and waits for input.
        /// </summary>
        private static void CreatePipeThread()
        {
            try
            {
                while (true)
                {
                    NamedPipeServerStream pipeServer = new NamedPipeServerStream("GrepPipStreamInput", PipeDirection.In);

                    pipeServer.WaitForConnection();
                    try
                    {
                        // Read the request from the client. Once the client has
                        // written to the pipe its security token will be available.

                        StreamString ss = new StreamString(pipeServer);
                        string text = ss.ReadString();
                        if (SearchPatternReceived != null)
                        {
                            string searchText = text.Substring(text.IndexOf("?") + 1);
                            string directory = text.Substring(0, text.IndexOf("?"));
                            SearchPatternReceived(directory, searchText);
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.get().AddError(ex.Message);
                    }
                    finally
                    {
                        pipeServer.Close();
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Creates a named pipe client and sends a search pattern.
        /// </summary>
        /// <param name="pattern">Patter to search.</param>
        /// <param name="directory">Directory to search.</param>
        public static void SendSearchPattern(String pattern, String directory)
        {
            try
            {
                using(NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "GrepPipStreamInput", PipeDirection.Out))
                {
                    try
                    {
                        pipeClient.Connect(1000);
                        StreamString ss = new StreamString(pipeClient);
                        ss.WriteString(pattern + "?" + directory);
                        pipeClient.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.get().AddError(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.get().AddError(ex.Message);
            }
        }

        /// <summary>
        /// Creates a named pipe client and sends the file and line number.
        /// </summary>
        /// <param name="info">The file and line number external programs are expecting.</param>
        public static void SendFileAndLine(String info)
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "GrepVisualStudioPluginInput", PipeDirection.Out))
                {
                    try
                    {
                        pipeClient.Connect(500);
                        StreamString ss = new StreamString(pipeClient);
                        ss.WriteString(info);
                        pipeClient.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.get().AddError(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.get().AddError(ex.Message);
            }
        }

        /// <summary>
        /// Defines the data protocol for reading and writing strings on our stream
        /// </summary>
        public class StreamString
        {
            private Stream ioStream;
            private UnicodeEncoding streamEncoding;

            public StreamString(Stream ioStream)
            {
                this.ioStream = ioStream;
                streamEncoding = new UnicodeEncoding();
            }

            public string ReadString()
            {
                int len = 0;

                len = ioStream.ReadByte() * 256;
                len += ioStream.ReadByte();
                byte[] inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                return streamEncoding.GetString(inBuffer);
            }

            public int WriteString(string outString)
            {
                byte[] outBuffer = streamEncoding.GetBytes(outString);
                int len = outBuffer.Length;
                if (len > UInt16.MaxValue)
                {
                    len = (int)UInt16.MaxValue;
                }
                ioStream.WriteByte((byte)(len / 256));
                ioStream.WriteByte((byte)(len & 255));
                ioStream.Write(outBuffer, 0, len);
                ioStream.Flush();

                return outBuffer.Length + 2;
            }
        }
    }
}
