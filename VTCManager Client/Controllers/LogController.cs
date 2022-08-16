using System;
using System.IO;
using System.Linq;

namespace VTCManager_Client.Controllers
{
    public static class LogController
    {
        private static string LogFilePath;
        private static string LogFileFolderPath;
        private static StreamWriter StreamWriter;
        public static string InitErrorMessage = null;
        public enum LogType
        {
            Info,
            Error,
            Warning,
            Debug,
        }

        private static bool InitDone = false;

        public static Models.ControllerStatus Init()
        {
            LogFileFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\VTCManager\";
            if (!Directory.Exists(LogFileFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(LogFileFolderPath);
                }
                catch (Exception ex)
                {
                    InitErrorMessage = "Couldn't create LogFileFolder. Aborting...\nDetailed Information: " + ex.Message;
                    return Models.ControllerStatus.FatalErrorIEM;
                }
            }
            else
            {
                // check for older log files
                DirectoryInfo info = new DirectoryInfo(LogFileFolderPath);
                FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                if (files.Length > 10)
                {
                    int files_to_remove = files.Length - 10;
                    foreach (FileInfo file in files)
                    {
                        File.Delete(file.FullName);
                        files_to_remove--;
                        if (files_to_remove <= 0)
                            break;
                    }
                }
            }

            LogFilePath = LogFileFolderPath + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".log";
            try
            {
                StreamWriter = new StreamWriter(File.Create(LogFilePath));
            }
            catch (Exception ex)
            {
                InitErrorMessage = "Couldn't create LogFile. Aborting...\nDetailed Information: " + ex.Message;
                return Models.ControllerStatus.FatalErrorIEM;
            }
            InitDone = true;
            return Models.ControllerStatus.OK;
        }

        public static void Write(string Message, LogType logType = LogType.Info)
        {
            string ltype;
            ConsoleColor consoleColor;
            switch (logType)
            {
                case LogType.Error:
                    ltype = "ERROR";
                    consoleColor = ConsoleColor.Red;
                    break;
                case LogType.Warning:
                    ltype = "WARNING";
                    consoleColor = ConsoleColor.Yellow;
                    break;
                case LogType.Debug:
                    if (!AppInfo.DebugMode)
                        return;
                    ltype = "DEBUG";
                    consoleColor = ConsoleColor.Gray;
                    break;
                default:
                    ltype = "INFO";
                    consoleColor = ConsoleColor.White;
                    break;
            }
            if (consoleColor != ConsoleColor.Black)
                Console.ForegroundColor = consoleColor;
            Console.Write("[" + ltype + "]\t");
            Console.ResetColor(); // only change the color of the log type

            Console.WriteLine("<" + DateTime.Now + "> " + Message);
            if (!InitDone)
                return;
            StreamWriter.WriteLine("[" + ltype + "]\t<" + DateTime.Now + "> " + Message);
            StreamWriter.Flush();
        }

        public static void ShutDown()
        {
            if (!InitDone)
                return;
            StreamWriter.Close();
            InitDone = false;
        }
    }
}