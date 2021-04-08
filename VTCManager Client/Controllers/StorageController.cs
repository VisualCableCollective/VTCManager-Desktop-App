using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;

namespace VTCManager_Client.Controllers
{
    public static class StorageController
    {
        public static string AppDataFolder;
        private static string DataFilePath;
        public static Models.Config Config;
        private static Timer AutoSaveTimer = new Timer(60000);
        public static string InitErrorMessage = null;

        public static string LogPrefix = "[StorageController] "; 

        private static bool InitDone = false;
        public static Models.ControllerStatus Init()
        {
            AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\VTCManager\";
            if (!Directory.Exists(AppDataFolder))
            {
                try
                {
                    Directory.CreateDirectory(AppDataFolder);
                }
                catch (Exception ex)
                {
                    InitErrorMessage = "Couldn't create AppDataFolder. Aborting...\nDetailed Information: " + ex.Message;
                    return Models.ControllerStatus.FatalErrorIEM;
                }
            }
            //Data File
            DataFilePath = AppDataFolder + "data.vtcm";
            FileStream DataFileStream;
            if (!File.Exists(DataFilePath))
            {
                try
                {
                    DataFileStream = File.Create(DataFilePath);
                }
                catch (Exception ex)
                {
                    InitErrorMessage = "Couldn't create data file. Aborting...\nDetailed Information: " + ex.Message;
                    return Models.ControllerStatus.FatalErrorIEM;
                }
                Config = new Models.Config();
            }
            else
            {
                try
                {
                    DataFileStream = File.Open(DataFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex)
                {
                    InitErrorMessage = "Couldn't open data file. Aborting...\nDetailed Information: " + ex.Message;
                    return Models.ControllerStatus.FatalErrorIEM;
                }
                string DataFileContent;
                StreamReader reader = new StreamReader(DataFileStream);
                DataFileContent = reader.ReadToEnd();
                if (String.IsNullOrWhiteSpace(DataFileContent))
                {
                    Config = new Models.Config();
                }
                else
                {
                    Config = JsonConvert.DeserializeObject<Models.Config>(DataFileContent);
                }
            }

            if (Config.Debug == true)
                VTCManager.DebugMode = true;

            DataFileStream.Close();

            //AutoSave
            AutoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;
            AutoSaveTimer.Start();

            InitDone = true;
            return Models.ControllerStatus.OK;
        }

        private static void AutoSaveTimer_Elapsed(object sender, ElapsedEventArgs e) => SaveConfig();

        public static void ShutDown()
        {
            if (!InitDone)
                return;
            SaveConfig();
            InitDone = false;
        }

        private static void SaveConfig()
        {
            var ConfigJSON = JsonConvert.SerializeObject(Config);
            File.WriteAllText(DataFilePath, ConfigJSON);
        }
    }
}