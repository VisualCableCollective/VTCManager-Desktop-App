using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;

namespace VTCManager_Client.Controllers
{
    public static class StorageController
    {
        public static Models.Config Config;
        private static readonly Timer _autoSaveTimer = new Timer(60000);
        public static string InitErrorMessage = "";

        private static bool _initDone = false;
        public static Models.ControllerStatus Init()
        {
            if (_initDone)
            {
                return Models.ControllerStatus.InitAlreadyDone;
            }

            if (!Directory.Exists(Models.Folder.AppDataFolder))
            {
                try
                {
                    _ = Directory.CreateDirectory(Models.Folder.AppDataFolder);
                }
                catch (Exception ex)
                {
                    InitErrorMessage = "Couldn't create AppDataFolder. Aborting...\nDetailed Information: " + ex.Message;
                    return Models.ControllerStatus.FatalErrorIEM;
                }
            }
            //Data File
            FileStream DataFileStream;
            if (!File.Exists(Models.File.DataFile))
            {
                try
                {
                    DataFileStream = File.Create(Models.File.DataFile);
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
                    DataFileStream = File.Open(Models.File.DataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex)
                {
                    InitErrorMessage = "Couldn't open data file. Aborting...\nDetailed Information: " + ex.Message;
                    return Models.ControllerStatus.FatalErrorIEM;
                }
                string DataFileContent;
                StreamReader reader = new StreamReader(DataFileStream);
                DataFileContent = reader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(DataFileContent))
                {
                    Config = new Models.Config();
                }
                else
                {
                    Config = JsonConvert.DeserializeObject<Models.Config>(DataFileContent);

                    // rare error, that the config is empty, but it may occur if the config file is broken
                    if (Config == null)
                    {
                        Config = new Models.Config();
                    }
                }
            }

            if (Config.Debug)
            {
                VTCManager.DebugMode = true;
            }

            DataFileStream.Close();

            //AutoSave
            _autoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;
            _autoSaveTimer.Start();

            _initDone = true;
            return Models.ControllerStatus.OK;
        }

        private static void AutoSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveConfig();
        }

        public static void ShutDown()
        {
            if (!_initDone)
            {
                return;
            }

            SaveConfig();
            _initDone = false;
        }

        private static void SaveConfig()
        {
            string ConfigJSON = JsonConvert.SerializeObject(Config);
            File.WriteAllText(Models.File.DataFile, ConfigJSON);
        }
    }
}