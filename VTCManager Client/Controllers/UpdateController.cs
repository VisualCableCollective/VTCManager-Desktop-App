using System;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace VTCManager_Client.Controllers
{
    public static class UpdateController
    {
        private static Windows.LoadingWindow LoadingWindow = null;
        private static bool InitDone = false;
        public static string InitErrorMessage = null;
        private static string LogPrefix = "[Updater] ";
        public static Models.ControllerStatus Init(Windows.LoadingWindow loadingWindow)
        {
            UpdateController.LoadingWindow = loadingWindow;
            //if (!CheckAndInstallUpdate())
            //{
            //    InitErrorMessage = "Couldn't check for updates or install new updates for the VTCManager client. The application will now be closed.";
            //    return Models.ControllerStatus.FatalError;
            //}

            //if (ApplicationDeployment.IsNetworkDeployed)
            //{
            //    if (ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() != StorageController.Config.last_deploy_version_used)
            //        InstallUpdate();
            //}

            InitDone = true;

            return Models.ControllerStatus.OK;
        }

        //private static bool CheckAndInstallUpdate()
        //{
        //    UpdateCheckInfo info = null;

        //    if (ApplicationDeployment.IsNetworkDeployed)
        //    {
        //        LogController.Write(LogPrefix + "Current version: " + ApplicationDeployment.CurrentDeployment.CurrentVersion);
        //        LogController.Write(LogPrefix + "Checking for new updates...");
        //        ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

        //        try
        //        {
        //            info = ad.CheckForDetailedUpdate();

        //        }
        //        catch (DeploymentDownloadException dde)
        //        {
        //            MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
        //            LogController.Write(LogPrefix + "The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message, LogController.LogType.Error);
        //            return false;
        //        }
        //        catch (InvalidDeploymentException ide)
        //        {
        //            MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
        //            LogController.Write(LogPrefix + "Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message, LogController.LogType.Error);
        //            return false;
        //        }
        //        catch (InvalidOperationException ioe)
        //        {
        //            MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
        //            LogController.Write(LogPrefix + "This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message, LogController.LogType.Error);
        //            return false;
        //        }

        //        if (info.UpdateAvailable)
        //        {
        //            LogController.Write(LogPrefix + "New update is available: " + info.AvailableVersion);
        //            LogController.Write(LogPrefix + "Downloading update...");
        //            LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
        //                new Action(() =>
        //                {
        //                    LoadingWindow.ChangeStatusText("Downloading new update");
        //                    LoadingWindow.UpdateProgressBar.Visibility = Visibility.Visible;
        //                    LoadingWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
        //                    LoadingWindow.TaskbarItemInfo.ProgressValue = 0;
        //                }));
        //            try
        //            {
        //                ad.UpdateProgressChanged += Ad_UpdateProgressChanged;
        //                ad.UpdateCompleted += Ad_UpdateCompleted;
        //                ad.UpdateAsync();
        //                while (true)
        //                {

        //                }
        //            }
        //            catch (DeploymentDownloadException dde)
        //            {
        //                MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
        //                LogController.Write(LogPrefix + "Cannot install the latest version of the application. Error: " + dde, LogController.LogType.Error);
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //private static void Ad_UpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    if (e.Cancelled)
        //    {
        //        LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
        //                new Action(() =>
        //                {
        //                    LoadingWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
        //                }));
        //        if (e.Error != null)
        //        {
        //            MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + e.Error.Message);
        //            LogController.Write(LogPrefix + "Cannot install the latest version of the application. Error: " + e.Error.Message, LogController.LogType.Error);
        //        }
        //        else
        //        {
        //            MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: n/a");
        //            LogController.Write(LogPrefix + "Cannot install the latest version of the application. Error: n/a", LogController.LogType.Error);
        //        }
        //        ControllerManager.ShutDown();
        //        Environment.Exit(-1);
        //        return;
        //    }
        //    string program_path = Path.Combine(ApplicationDeployment.CurrentDeployment.DataDirectory, "VTCManager Client.exe");
        //    if (!File.Exists(program_path))
        //    {
        //        MessageBox.Show("Couldn't restart VTCManager Client. Please run it from the start menu again.", "Error: Restart failed", MessageBoxButton.OK, MessageBoxImage.Error);
        //        LogController.Write(LogPrefix + "Couldn't restart app: couldn't find program in start menu.", LogController.LogType.Error);
        //        ControllerManager.ShutDown();
        //        Environment.Exit(-1);
        //    }
        //    ControllerManager.ShutDown(); //shutdown controllers before starting the new process to prevent errors when opening the config by the new process
        //    Process process = Process.Start(program_path);
        //    Environment.Exit(0);
        //    throw new NotImplementedException();
        //}

        //private static void Ad_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        //{
        //    LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
        //                new Action(() =>
        //                {
        //                    LoadingWindow.UpdateProgressBar.Value = e.ProgressPercentage;
        //                    LoadingWindow.TaskbarItemInfo.ProgressValue = e.ProgressPercentage;
        //                }));
        //}

        public static void ShutDown()
        {
            if (!InitDone)
                return;
        }
        //public static void InstallUpdate()
        //{
        //    /*
        //     *  xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\locales\en-US.pak"  "$(ProjectDir)$(OutDir)\locales"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\swiftshader\libEGL.dll"  "$(ProjectDir)$(OutDir)\swiftshader"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\swiftshader\libGLESv2.dll"  "$(ProjectDir)$(OutDir)\swiftshader"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\cef.pak"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\cef_100_percent.pak"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\cef_200_percent.pak"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\cef_extensions.pak"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\CefSharp.BrowserSubprocess.exe"  "$(ProjectDir)$(OutDir)"    
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\chrome_elf.dll"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\d3dcompiler_47.dll"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\icudtl.dat"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\libcef.dll"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\libEGL.dll"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\libGLESv2.dll"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\snapshot_blob.bin"  "$(ProjectDir)$(OutDir)"
        //        xcopy /y "$(ProjectDir)Resources\DLLs\CefSharp\v8_context_snapshot.bin"  "$(ProjectDir)$(OutDir)"
        //     * */
        //    if (!ApplicationDeployment.IsNetworkDeployed)
        //        return;
        //    LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
        //                new Action(() =>
        //                {
        //                    LoadingWindow.ChangeStatusText("Copying update files");
        //                }));
        //    string location = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("VTCManager Client.exe", "");
        //    LogController.Write(LogPrefix + "Current exe location: " + location);

        //    if(!Directory.Exists(location + @"locales\"))
        //        Directory.CreateDirectory(location + @"locales\");
        //    if(!Directory.Exists(location + @"swiftshader\"))
        //        Directory.CreateDirectory(location + @"swiftshader\");

        //    if(!File.Exists(location + @"locales\en-US.pak"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\locales\en-US.pak", location + @"locales\en-US.pak", true);
        //    if(!File.Exists(location + @"swiftshader\libEGL.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\swiftshader\libEGL.dll", location + @"swiftshader\libEGL.dll", true);
        //    if(!File.Exists(location + @"swiftshader\libGLESv2.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\swiftshader\libGLESv2.dll", location + @"swiftshader\libGLESv2.dll", true);
        //    if(!File.Exists(location + @"cef.pak"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\cef.pak", location + @"cef.pak", true);
        //    if(!File.Exists(location + @"cef_100_percent.pak"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\cef_100_percent.pak", location + @"cef_100_percent.pak", true);
        //    if(!File.Exists(location + @"cef_200_percent.pak"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\cef_200_percent.pak", location + @"cef_200_percent.pak", true);
        //    if(!File.Exists(location + @"ef_extensions.pak"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\cef_extensions.pak", location + @"ef_extensions.pak", true);
        //    if(!File.Exists(location + @"CefSharp.BrowserSubprocess.exe"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\CefSharp.BrowserSubprocess.exe", location + @"CefSharp.BrowserSubprocess.exe", true);
        //    if(!File.Exists(location + @"chrome_elf.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\chrome_elf.dll", location + @"chrome_elf.dll", true);
        //    if(!File.Exists(location + @"d3dcompiler_47.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\d3dcompiler_47.dll", location + @"d3dcompiler_47.dll", true);
        //    if(!File.Exists(location + @"icudtl.dat"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\icudtl.dat", location + @"icudtl.dat", true);
        //    if(!File.Exists(location + @"libcef.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\libcef.dll", location + @"libcef.dll", true);
        //    if(!File.Exists(location + @"libEGL.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\libEGL.dll", location + @"libEGL.dll", true);
        //    if(!File.Exists(location + @"libGLESv2.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\libGLESv2.dll", location + @"libGLESv2.dll", true);
        //    if(!File.Exists(location + @"snapshot_blob.bin"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\snapshot_blob.bin", location + @"snapshot_blob.bin", true);
        //    if(!File.Exists(location + @"v8_context_snapshot.bin"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\v8_context_snapshot.bin", location + @"v8_context_snapshot.bin", true);

        //    if(!File.Exists(location + @"CefSharp.Core.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\CefSharp.Core.dll", location + @"CefSharp.Core.dll", true);
        //    if(!File.Exists(location + @"CefSharp.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\CefSharp.dll", location + @"CefSharp.dll", true);
        //    if(!File.Exists(location + @"CefSharp.Wpf.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\CefSharp.Wpf.dll", location + @"CefSharp.Wpf.dll", true);
        //    if (!File.Exists(location + @"CefSharp.Core.Runtime.dll"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\CefSharp.Core.Runtime.dll", location + @"CefSharp.Core.Runtime.dll", true);
        //    if (!File.Exists(location + @"CefSharp.Core.Runtime.xml"))
        //        File.Copy(location + @"Resources\DLLs\CefSharp\CefSharp.Core.Runtime.xml", location + @"CefSharp.Core.Runtime.xml", true);

        //    //update auto start registry
        //    if (!StorageController.Config.User_Disabled_Auto_Start)
        //    {
        //        try
        //        {
        //            RegistryKey RegSubKeyAutoStart =
        //                Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        //            RegSubKeyAutoStart.SetValue("VTCManager",
        //                "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"" + " -silent");
        //        }
        //        catch (Exception ex)
        //        {
        //            LogController.Write(LogPrefix + "Failed to update auto start registry key: " + ex.Message, LogController.LogType.Error);
        //        }
        //    }

        //    StorageController.Config.last_deploy_version_used = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
        //}
    }
}