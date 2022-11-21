using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using VTCManager.Models.Enums;
using VTCManager.Logging;

namespace VTCManager_Client.Controllers
{
    public static class GameLogController
    {
        private static readonly String LogPrefix = "[" + nameof(GameLogController) + "] ";
        private static bool InitDone = false;
        public static string InitErrorMessage = null;
        private static String ETSFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Euro Truck Simulator 2\";
        private static String ETSLogFilePath;

        public static ControllerStatus Init()
        {
            if (InitDone)
                return ControllerStatus.InitAlreadyDone;
            ETSLogFilePath = ETSFolder + "game.log.txt";

            SetGameLanguageCodeInConfig();

            if (String.IsNullOrWhiteSpace(StorageController.Config.GameLanguageCode))
            {
                LogController.Write(LogPrefix + "Could not detect game language", LogController.LogType.Warning);
            }
            else
            {
                LogController.Write(LogPrefix + "Current detected game language: " + String.IsNullOrWhiteSpace(StorageController.Config.GameLanguageCode));
            }

            InitDone = true;
            return ControllerStatus.OK;
        }

        public static void SetGameLanguageCodeInConfig()
        {
            string LogFileContent;
            try
            {
                LogFileContent = File.ReadAllText(ETSLogFilePath);
            }
            catch
            {
                try
                {
                    File.Copy(ETSLogFilePath, ETSFolder + "game.log.backup.txt", true);
                    LogFileContent = File.ReadAllText(ETSFolder + "game.log.backup.txt");
                }
                catch(Exception ex2)
                {
                    LogController.Write(LogPrefix + "An error occured while copying the game log file and read it. Error: " + ex2.Message, LogController.LogType.Error);
                    StorageController.Config.GameLanguageCode = null;
                    return;
                }
            }

            string raw_code = GetStringBetweenText(LogFileContent, "Selected language: ", "\n");
            var seperator = new[] { '_' };
            string conv_code = raw_code.Split(seperator)[0];

            StorageController.Config.GameLanguageCode = conv_code;
        }

        private static string GetStringBetweenText(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        public static ControllerStatus ShutDown()
        {
            if (!InitDone)
                return ControllerStatus.InitNotDone;
            return ControllerStatus.OK;
        }
    }
}
