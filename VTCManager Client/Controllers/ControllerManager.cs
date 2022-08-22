using System;
using System.Collections.Generic;
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
        public static readonly string LogPrefix = "[" + nameof(ControllerManager) + "] ";

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
                        LoadingWindow = window as Windows.LoadingWindow;
                    }
                }
            });

            #region Important Controllers
            //Important controllers!!! If an error occurs, then close application. Do not change the boot order!
            Initialize(nameof(StorageController), StorageController.Init(), StorageController.InitErrorMessage, true, true);
            Initialize(nameof(LogController), LogController.Init(), LogController.InitErrorMessage, true, true);
            Initialize(nameof(UpdateController), UpdateController.Init(LoadingWindow), UpdateController.InitErrorMessage, true, true);
            #endregion

            Initialize(nameof(AuthDataController), AuthDataController.Init(), null);

            //API
            _ = LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            LoadingWindow.ChangeStatusText("Connecting to the server");
                        }));

            List<Models.ControllerStatus> apiInitStatusList = API.MainAPIController.Init();
            if (apiInitStatusList.Contains(Models.ControllerStatus.VTCMServerInoperational))
            {
                ShowErrorWindow("VTCManager API", 
                    "The VTCManager server is currently not available. Please check your internet connection or check the VisualCable Collective status page " +
                    "(https://status.vcc-online.eu/) for further information.");
            }

            // installs the telemetry DLL if not already installed
            if (!StorageController.Config.ETS_Plugin_Installed || !StorageController.Config.ATS_Plugin_Installed)
            {
                _ = LoadingWindow.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            LoadingWindow.ChangeStatusText("Installing ETS2/ATS Plugin");
                        }));
                
                if (!StorageController.Config.ETS_Plugin_Installation_Tried || !StorageController.Config.ATS_Plugin_Installation_Tried)
                {
                    PluginInstaller.Install();

                    if (!StorageController.Config.ETS_Plugin_Installed || StorageController.Config.ATS_Plugin_Installed)
                    {
                        string mbText = "Automatic plugin installation failed for these games: ";

                        if (!StorageController.Config.ETS_Plugin_Installed)
                        {
                            mbText += "Euro Truck Simulator 2";
                        }

                        if (!StorageController.Config.ATS_Plugin_Installed)
                        {
                            if (mbText.Contains("Euro Truck"))
                            {
                                mbText += " & American Truck Simulator";
                            } 
                            else
                            {
                                mbText += "American Truck Simulator";
                            }
                        }

                        mbText += "\nThis might happen, if a game is not installed.";

                        MessageBox.Show(mbText, "VTCManager: Plugin Installation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

            Initialize(nameof(GameLogController), GameLogController.Init(), GameLogController.InitErrorMessage);

            LogController.Write(LogPrefix + "Boot initialization finished.");
            return apiInitStatusList;
        }

        /// <summary>
        /// Initializes the specified controller.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerStatus">The status of the controller returned by the Init() function.</param>
        /// <param name="initErrorMessageVariable">String, where the error message of the Init() function can be found.</param>
        /// <param name="showErrorWindowOnError">If true, the error window is displayed in the event of an error.</param>
        /// <param name="shutDownIfInitFails">Shuts down the application if an error occurs.</param>
        private static void Initialize(string controllerName, Models.ControllerStatus controllerStatus, string initErrorMessage, bool showErrorWindowOnError = false, bool shutDownOnError = false)
        {
            //Logging
            LogController.Write($"{LogPrefix}Initialization of {controllerName} returned {controllerStatus}", LogController.LogType.Debug);

            if (controllerStatus == Models.ControllerStatus.OK)
            {
                return;
            }

            switch (controllerStatus)
            {
                case Models.ControllerStatus.FatalErrorIEM:
                    LogController.Write($"{LogPrefix}Initialization of {}controllerName returned " + controllerStatus + " | Error: " + initErrorMessage, LogController.LogType.Error);
                    break;
            }

            if (controllerStatus == Models.ControllerStatus.FatalErrorIEM)
            {
                LogController.Write(LogPrefix + "Initialization of " + controllerName + " returned " + controllerStatus + " | Error: " + initErrorMessage, LogController.LogType.Error);
            }
            else
            {
                LogController.Write(LogPrefix + "Initialization of " + controllerName + " returned " + controllerStatus, LogController.LogType.Error);
            }


            // Error Window
            if (!showErrorWindowOnError)
            {
                return;
            }

            if (controllerStatus == Models.ControllerStatus.FatalErrorIEM)
            {
                ShowErrorWindow("Initialization Error: " + controllerName, initErrorMessage, shutDownOnError);
            }
            else
            {
                ShowErrorWindow("Initialization Error: " + controllerName, "The initialization of " + controllerName + " failed.", shutDownOnError);
            }
        }

        /// <summary>
        /// Initializes controllers, which need the MainWindow to run properly.
        /// </summary>
        public static void MainWindowInit()
        {
            //get the main window
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(Windows.MainWindow))
                    {
                        MainWindow = window as Windows.MainWindow;
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
        /// <param name="shutDown">
        /// Closes the loading window now and the application after the user closed the error window.
        /// </param>
        private static void ShowErrorWindow(string title, string description, bool shutDown = true)
        {
            LogController.Write("Error title: " + title + "\nDescription: " + description, LogController.LogType.Error);
            _ = MessageBox.Show(description, title, MessageBoxButton.OK, MessageBoxImage.Error);
            if (shutDown)
            {
                ShutDown();
                Environment.Exit(-1);
            }
        }
    }
}