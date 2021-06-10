using CrashReporterDotNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace VTCManager_Client
{
    /// <summary>
    /// Interaction logic for "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private static ReportCrash _reportCrash;

        //Windows
        private Windows.LoadingWindow _loadingWindow;
        private Windows.MainWindow _mainWindow;

        /// <summary>
        /// The timer that searches for existing ".showapp" files to restore the currently active window.
        /// </summary>
        private readonly System.Timers.Timer _checkShowAppFilesTimer = new System.Timers.Timer(250);

        private readonly string _appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\VTCManager\";

        /// <summary>
        /// The name of the temporary files, that the application should create and list to,
        /// to display the currently active window again when the application is hidden.
        /// </summary>
        private readonly string SHOWAPPFILENAME = ".showapp";

        private readonly string LOGPREFIX = "[APP] ";

        public App()
        {
            //check if this app is already running
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == currentProcessName) > 1)
            {
                //notify the running process that it should show the current main window and set it to the topmost window
                if (!Directory.Exists(_appDataFolder))
                {
                    _ = Directory.CreateDirectory(_appDataFolder);
                }
                if (!File.Exists(_appDataFolder + SHOWAPPFILENAME))
                {
                    _ = File.Create(_appDataFolder + SHOWAPPFILENAME);
                }
                Current.Shutdown();
            }

            //set up a listener for .showapp files
            _checkShowAppFilesTimer.Elapsed += CheckShowAppFilesTimerElapsed;
            _checkShowAppFilesTimer.Start();

            // enable hardware acceleration
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
        }

        /// <summary>
        /// Shows the currently active window and sets it to the topmost window.
        /// </summary>
        private void ShowCurrentlyActiveWindow()
        {
            try
            {
                if (_mainWindow != null)
                {
                    _mainWindow.Show();
                    _mainWindow.Topmost = true;
                    Controllers.DiscordRPCController.Init();
                }
                else if (_loadingWindow != null)
                {
                    _loadingWindow.Show();
                    _loadingWindow.Topmost = true;
                }
            }
            catch (Exception ex)
            {
                Controllers.LogController.Write(LOGPREFIX + "Error while showing the currently active window: " + ex.Message,
                    Controllers.LogController.LogType.Error);
            }
        }

        /// <summary>
        /// Event fired by the <see cref="_checkShowAppFilesTimer"/> to regularly check if a ".showapp" file
        /// exists to show the currently active window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckShowAppFilesTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _checkShowAppFilesTimer.Stop();
            Dispatcher.Invoke(() =>
            {
                if (File.Exists(_appDataFolder + SHOWAPPFILENAME))
                {
                    ShowCurrentlyActiveWindow();

                    try
                    {
                        File.Delete(_appDataFolder + SHOWAPPFILENAME);
                    }
                    catch
                    {
                        Thread.Sleep(10); // just to make sure that it is not used by the other process
                        File.Delete(_appDataFolder + SHOWAPPFILENAME);
                    }
                }
            });
            _checkShowAppFilesTimer.Start();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // set up crash reporter
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            _reportCrash = new ReportCrash(VTCManager.CrashReportReceiverEmail)
            {
                Silent = true
            };
            _ = _reportCrash.RetryFailedReports();


            // silent mode checks
            if (e.Args.Contains("-silent"))
            {
                VTCManager.SilentAutoStartMode = true;
            }

            _loadingWindow = new Windows.LoadingWindow(this);
            if (!VTCManager.SilentAutoStartMode)
            {
                _loadingWindow.Show();
            }

            MainWindow = _loadingWindow;
        }

        #region CrashReporting
        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            SendReport(unobservedTaskExceptionEventArgs.Exception);
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            SendReport(dispatcherUnhandledExceptionEventArgs.Exception);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            SendReport((Exception)unhandledExceptionEventArgs.ExceptionObject);
        }

        public static void SendReport(Exception exception)
        {
            if (exception is AggregateException) //sometimes caused by the Pusher client
            {
                if (string.IsNullOrWhiteSpace(exception.Source))
                {
                    if (exception.InnerException != null)
                    {
                        if (exception.InnerException is System.Net.Sockets.SocketException)
                        {
                            return;
                        }
                    }
                }
            }

            _reportCrash.Silent = false;
            _reportCrash.Send(exception);
        }

        public static void SendReportSilently(Exception exception)
        {
            _reportCrash.Silent = true;
            _reportCrash.Send(exception);
        }
        #endregion

        /// <summary>
        /// Shows the <see cref="Windows.MainWindow"/> and closes the <see cref="Windows.LoadingWindow"/>.<br/>
        /// If <see cref="VTCManager.SilentAutoStartMode"/> is <see langword="true"/> the <see cref="Windows.MainWindow"/> won't be visible.
        /// </summary>
        /// <param name="AppInitResults"></param>
        public void LaunchMainWindow(List<Models.ControllerStatus> appInitResults)
        {
            if (_mainWindow != null)
            {
                return;
            }

            _mainWindow = new Windows.MainWindow(this, appInitResults);
            if (!VTCManager.SilentAutoStartMode)
            {
                _mainWindow.Show();
            }

            MainWindow = _mainWindow;

            _loadingWindow.Close();
            _loadingWindow = null;
        }
    }
}
