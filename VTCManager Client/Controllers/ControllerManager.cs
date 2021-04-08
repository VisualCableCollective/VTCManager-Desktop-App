using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VTCManager_Client.Controllers
{
    /// <summary>
    /// This class manages the initialization and shutdown of all controllers.
    /// The controller's Init() and ShutDown() function should be called here.<br/>
    /// </summary>
    public static class ControllerManager
    {
        public static string LogPrefix = "[" + nameof(ControllerManager) +"] ";

        private static Windows.LoadingWindow LoadingWindow = null;
        public static Windows.MainWindow MainWindow = null;

        /// <summary>
        /// Initializes all controllers of the application, which are registered in this function, at the start of the application, when the loading window is displayed. If a critical error occurs, the application will be closed.
        /// </summary>
        /// <returns>
        /// Information, warnings, and non-critical errors returned by the controllers.
        /// </returns>
        public static List<Models.ControllerStatus> BootInit()
        {
            LogController.Write(LogPrefix + "Starting boot initialization...");

            //get the loadingwindow to change the status label
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(Windows.LoadingWindow))
                    {
                        LoadingWindow = (window as Windows.LoadingWindow);
                    }
                }
            });

            #region Important Controllers
            //Important controllers!!! If error, then close application. Do not change the boot order!
            Initialize(nameof(StorageController), StorageController.Init(), StorageController.InitErrorMessage, true, true);
            Initialize(nameof(LogController), LogController.Init(), LogController.InitErrorMessage, true, true);
            #endregion

            AuthDataController.Init();
            Initialize(nameof(LoadingWindow), UpdateController.Init(LoadingWindow), UpdateController.InitErrorMessage, true, true);

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() != StorageController.Config.last_deploy_version_used)
                    UpdateController.InstallUpdate();
            }

            //API
            LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            LoadingWindow.ChangeStatusText("Connecting to the server");
                        }));

            List<Models.ControllerStatus> APIInitStatusList = API.MainAPIController.Init();
            if (APIInitStatusList.Contains(Models.ControllerStatus.VTCMServerInoperational))
                ShowErrorWindow("VTCManager API", "The VTCManager server is currently not available. Please check your internet connection or check the VisualCable Collective status page (https://status.vcc-online.eu/) for further information.");

            // installs the telemetry DLL if not already installed
            if (!StorageController.Config.ETS_Plugin_Installed && !StorageController.Config.ATS_Plugin_Installed)
            {
                LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            LoadingWindow.ChangeStatusText("Installing ETS2/ATS Plugin");
                        }));
                PluginInstaller.Install();
            }

            Initialize(nameof(GameLogController), GameLogController.Init(), GameLogController.InitErrorMessage);

            LogController.Write(LogPrefix + "Boot initialization finished.");
            return APIInitStatusList;
        }

        private static void Initialize(String ControllerName, Models.ControllerStatus ControllerStatus, String InitErrorMessageVariable, bool showErrorWindowIfInitFails = false, bool shutDownIfInitFails = false)
        {
            LogController.Write(LogPrefix + "Initialization of " + ControllerName + " returned " + ControllerStatus, LogController.LogType.Debug);

            if (ControllerStatus == Models.ControllerStatus.OK)
                return;

            if(ControllerStatus == Models.ControllerStatus.FatalErrorIEM)
            {
                LogController.Write(LogPrefix + "Initialization of " + ControllerName + " returned " + ControllerStatus + " | Error: " + InitErrorMessageVariable, LogController.LogType.Error);
            }
            else
            {
                LogController.Write(LogPrefix + "Initialization of " + ControllerName + " returned " + ControllerStatus, LogController.LogType.Error);
            }

            if (showErrorWindowIfInitFails)
            {
                if(ControllerStatus == Models.ControllerStatus.FatalErrorIEM)
                {
                    ShowErrorWindow("Initialization Error: " + ControllerName, InitErrorMessageVariable, shutDownIfInitFails);
                }
                else
                {
                    ShowErrorWindow("Initialization Error: " + ControllerName, "The initialization of " + ControllerName + " failed.", shutDownIfInitFails);
                }
            }
        }

        /// <summary>
        /// Initializes controllers, which need the MainWindow to run properly.
        /// </summary>
        public static void MainWindowInit()
        {
            //get the main window to show the modals
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(Windows.MainWindow))
                    {
                        MainWindow = (window as Windows.MainWindow);
                    }
                }
            });

            ModalController.Init();
        }

        /// <summary>
        /// Shutdowns all controllers of the application, which are registered in this function.
        /// </summary>
        public static void ShutDown()
        {
            API.MainAPIController.ShutDown();
            DiscordRPCController.ShutDown();
            LogController.ShutDown();
            StorageController.ShutDown();
            TelemetryController.ShutDown();
        }

        /// <summary>
        /// Shows an error window and closes the loading window.
        /// </summary>
        /// <param name="title">
        /// The name of the error.
        /// </param>
        /// <param name="description">
        /// Detailed information about the error.
        /// </param>
        /// <param name="ShutDown">
        /// Closes the loading window now and the application after the user closed the error window.
        /// </param>
        private static void ShowErrorWindow(string title, string description, bool ShutDown = true)
        {
            LogController.Write("Error title: " + title + "\nDescription: " + description, LogController.LogType.Error);
            MessageBox.Show(description, title, MessageBoxButton.OK, MessageBoxImage.Error);
            if (ShutDown)
            {
                Controllers.ControllerManager.ShutDown();
                Environment.Exit(-1);
            }
        }
    }
}