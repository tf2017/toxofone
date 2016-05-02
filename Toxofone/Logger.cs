namespace Toxofone
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    public enum LogLevel : int
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Verbose = 4
    }

    public static class Logger
    {
        private static readonly object staticSyncLock = new object();

        private static LogLevel currentLogLevel = LogLevel.Error;

        public static LogLevel SetLogLevel(LogLevel logLevel)
        {
            lock (staticSyncLock)
            {
                LogLevel prevLogLevel = currentLogLevel;
                currentLogLevel = logLevel;
                return prevLogLevel; 
            }
        }

        public static void CleanupLogs()
        {
            lock (staticSyncLock)
            {
                DateTime now = DateTime.Now;

                // collect log files older than 3 days
                IList<FileInfo> filesToDelete = new List<FileInfo>();
                string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string[] logFiles = Directory.GetFiles(rootDir, "Toxofone_*.log", SearchOption.TopDirectoryOnly);
                foreach (string logFile in logFiles)
                {
                    FileInfo fi = new FileInfo(Path.Combine(rootDir, logFile));
                    if (now - fi.CreationTime >= TimeSpan.FromDays(3))
                    {
                        filesToDelete.Add(fi);
                    }
                }

                for (int i = 0; i < filesToDelete.Count; i++)
                {
                    filesToDelete[i].Delete();
                }
            }
        }

        public static void Log(LogLevel logLevel,
            string text,
            StackTrace stackTrace = null)
        {
            lock (staticSyncLock)
            {
                if (Convert.ToInt32(logLevel) <= Convert.ToInt32(currentLogLevel))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(logLevel.ToString()[0]);
                    sb.Append(" | ");

                    sb.Append(DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture));
                    sb.Append(" ");

                    if (stackTrace != null && stackTrace.FrameCount > 0)
                    {
                        string fileName = stackTrace.GetFrame(0).GetFileName();
                        string member = stackTrace.GetFrame(0).GetMethod().Name;
                        int line = stackTrace.GetFrame(0).GetFileLineNumber();
                        sb.AppendFormat("[{0}] {1}.{2}:{3}", Thread.CurrentThread.ManagedThreadId, Path.GetFileNameWithoutExtension(fileName), member, line);
                    }

                    while (sb.Length < 72)
                    {
                        sb.Append(" ");
                    }

                    sb.AppendFormat(" | {0}", text);

                    Debug.WriteLine(sb.ToString());
                    WriteLine(sb.ToString());
                }
            }
        }

        private static void WriteLine(string msg)
        {
            try
            {
                string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string dateNow = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                string logPath = Path.Combine(rootDir, string.Format("Toxofone_{0}.log", dateNow));

                if (!Directory.Exists(rootDir))
                {
                    Directory.CreateDirectory(rootDir);
                }

                using (StreamWriter sw = new StreamWriter(new FileStream(logPath, FileMode.Append, FileAccess.Write)))
                {
                    sw.WriteLine(msg);
                    sw.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
