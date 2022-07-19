using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using VdfParser;

namespace VTCManager_Client.Controllers
{
    public static class PluginInstaller
    {
        //private static string ETS2_EXE = "eurotrucks2.exe";
        //STEAM
        private static readonly string SteamLibraryConfigFile = @"\steamapps\libraryfolders.vdf";
        private static readonly string LogPrefix = "[PluginInstaller] ";
        public static void Install()
        {
            InstallTelemetryETS2();
        }
        private static void InstallTelemetryETS2()
        {
            if (StorageController.Config.ETS_Plugin_Installation_Tried || StorageController.Config.ATS_Plugin_Installation_Tried) return;

            LogController.Write(LogPrefix + "Starting ETS/ATS tracker installation");

            StorageController.Config.ETS_Plugin_Installed = false;
            StorageController.Config.ATS_Plugin_Installed = false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                String SteamInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam\", "InstallPath", null).ToString();
                if (!String.IsNullOrEmpty(SteamInstallPath))
                {
                    LogController.Write(LogPrefix + "Found Steam Installation in '" + SteamInstallPath + "'.");
                    if(File.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\win_x64\eurotrucks2.exe") || File.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\win_x86\eurotrucks2.exe"))
                    {
                        LogController.Write(LogPrefix + "Found ETS2 in the default installation path.");
                        InstallPlugin(SteamInstallPath + @"\steamapps\common\", Games.ETS2);
                        StorageController.Config.ETS_Plugin_Installed = true;
                    }
                    if(File.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\win_x86\amtrucks.exe") || File.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\win_x64\amtrucks.exe"))
                    {
                        LogController.Write(LogPrefix + "Found ATS in the default installation path.");
                        InstallPlugin(SteamInstallPath + @"\steamapps\common\", Games.ATS);
                        StorageController.Config.ATS_Plugin_Installed = true;
                    }

                    String SteamLibraryConfigPath = SteamInstallPath + SteamLibraryConfigFile;
                    if (File.Exists(SteamLibraryConfigPath))
                    {
                        LogController.Write(LogPrefix + "Found Steam Library Configuration File in '" + SteamLibraryConfigPath + "'. Looking for ganes...");
                        string testFile = File.ReadAllText(SteamLibraryConfigPath);
                        VdfDeserializer deserializer = new VdfDeserializer();
                        List<string> SteamLibraries = new List<string>();

                        // try catch because it's dynamic and can change
                        try
                        {
                            dynamic result = deserializer.Deserialize(testFile);
                            IDictionary<string, dynamic> result_dictionary = result.libraryfolders;
                            for (int i = 1; i < 5; i++)
                            {
                                if (result_dictionary.ContainsKey(i.ToString()))
                                {
                                    SteamLibraries.Add(result_dictionary[i.ToString()].path.ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogController.Write("Error while parsing Steam lib: " + ex.Message, LogController.LogType.Error);
                        }

                        foreach (String Steampath in SteamLibraries)
                        {
                            if (File.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\win_x86\eurotrucks2.exe") || File.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\win_x64\eurotrucks2.exe"))
                            {
                                InstallPlugin(Steampath + @"\steamapps\common\", Games.ETS2);
                                StorageController.Config.ETS_Plugin_Installed = true;
                            }
                            if (File.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\win_x86\amtrucks.exe") || File.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\win_x64\amtrucks.exe"))
                            {
                                InstallPlugin(Steampath + @"\steamapps\common\", Games.ATS);
                                StorageController.Config.ATS_Plugin_Installed = true;
                            }
                        }
                    }
                }
            }
            if (!StorageController.Config.ETS_Plugin_Installed)
            {
                LogController.Write(LogPrefix + "Couldn't auto detect ETS2 installation folder.", LogController.LogType.Warning);
            }
            if (!StorageController.Config.ATS_Plugin_Installed)
            {
                LogController.Write(LogPrefix + "Couldn't auto detect ATS installation folder.", LogController.LogType.Warning);
            }

            StorageController.Config.ETS_Plugin_Installation_Tried = true;
            StorageController.Config.ATS_Plugin_Installation_Tried = true;
        }

        private static void InstallPlugin(string GamePath, Games Game)
        {
            if (Game == Games.ETS2)
            {
                GamePath += @"Euro Truck Simulator 2\bin\";
            }
            else if (Game == Games.ATS)
            {
                GamePath += @"American Truck Simulator\bin\";
            }
            LogController.Write(LogPrefix + "Installing " + Game.ToString() + " plugin...");
            try
            {
                //64
                if (!Directory.Exists(GamePath + @"win_x64\plugins"))
                    Directory.CreateDirectory(GamePath + @"win_x64\plugins");

                //make sure to got the correct telemetry version
                if (File.Exists(GamePath + @"win_x64\plugins\scs-telemetry.dll"))
                    File.Delete(GamePath + @"win_x64\plugins\scs-telemetry.dll");
                File.Copy(Environment.CurrentDirectory + "/Resources/Files/scs-telemetry.dll", GamePath + @"win_x64\plugins\scs-telemetry.dll");

                //86
                if (!Directory.Exists(GamePath + @"win_x86\plugins"))
                    Directory.CreateDirectory(GamePath + @"win_x86\plugins");

                //make sure to got the correct telemetry version
                if (File.Exists(GamePath + @"win_x86\plugins\scs-telemetry.dll"))
                    File.Delete(GamePath + @"win_x86\plugins\scs-telemetry.dll");
                File.Copy(Environment.CurrentDirectory + @"/Resources/Files/scs-telemetry.dll", GamePath + @"win_x86\plugins\scs-telemetry.dll");
            }
            catch (Exception ex)
            {
                LogController.Write(LogPrefix + "An error occured while installing the " + Game.ToString() + " plugin. " + ex.Message + " // ", LogController.LogType.Error);
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Windows.ErrorWindow errorWindow = new Windows.ErrorWindow("An error occured while installing the " + Game.ToString() + " plugin.", "We could detect the " + Game.ToString() + " location but could not finish the installation. " + ex.Message, false);
                    errorWindow.Show();
                });
            }
            LogController.Write(LogPrefix + Game.ToString() + " plugin installation finished.");
        }

        public enum Games
        {
            ETS2,
            ATS
        }
    }
}