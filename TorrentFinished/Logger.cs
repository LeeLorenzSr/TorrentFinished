using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentFinished
{
    class Logger : IDisposable
    {
        private static Logger currentLogger = null;

        public static void Init(string logPath)
        {
            currentLogger = new Logger(logPath);
        }

        private Logger(string filePath)
        {
            logPath = filePath;
            // currentLogger = this;
        }

        string logPath;
        StreamWriter logWriter = null;

        private static bool CheckLogger()
        {
            if (currentLogger == null)
            {
                Init("program.log");
            }
            return (currentLogger != null);
        }

        private bool CheckWriter()
        {
            if ( logWriter == null )
            {
                try {
                    logWriter = File.AppendText(logPath);
                    logWriter.AutoFlush = true;
                }
                catch
                {

                }
            }
            return (logWriter != null);
        }

        public static void Write(string logMessage)
        {
            if (CheckLogger())
            {
                currentLogger.WriteMessageToLog(logMessage);
            }
        }

        public static void Write(Exception e, string logMessage)
        {
            if (CheckLogger())
            {
                currentLogger.WriteMessageToLog(logMessage);
                currentLogger.WriteMessageToLog(e.ToString());
            }
        }

        private void WriteMessageToLog(string logMessage)
        {
            if (CheckWriter())
            {
                Console.WriteLine("{0} {1} : {2}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), logMessage);
                logWriter.WriteLine("{0} {1} : {2}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), logMessage);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (logWriter != null)
                    {
                       // logWriter.Flush();
                        // logWriter.Close();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Logger() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
