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
using VTCManager_Client.Controllers;
using VTCManager_Client.Windows;

namespace VTCManager_Client
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private static ReportCrash _reportCrash;

        //Windows
        private Windows.LoadingWindow _LoadingWindow;
        private Windows.MainWindow _MainWindow;

        private System.Timers.Timer fcsmwTimer;

        private String AppDataFolder;

        public App()
        {
            AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\VTCManager\";

            //check if this app is already running
            String currentProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == currentProcessName) > 1)
            {
                //notify the running process that it should show the current main window
                if (!Directory.Exists(AppDataFolder))
                {
                    Directory.CreateDirectory(AppDataFolder);
                }
                if (!File.Exists(AppDataFolder + ".fcsmw"))
                {
                    File.Create(AppDataFolder + ".fcsmw");
                }

                Application.Current.Shutdown();
            }

            //set up a listener for .fcsmw files (FocusMainWindow file)
            fcsmwTimer = new System.Timers.Timer(250);
            fcsmwTimer.Elapsed += FcsmwTimer_Elapsed;
            fcsmwTimer.Start();

        }

        private void FcsmwTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            fcsmwTimer.Stop();
            this.Dispatcher.Invoke(() =>
            {
                if (File.Exists(AppDataFolder + ".fcsmw"))
                {
                    try
                    {
                        if (_MainWindow != null)
                        {
                            _MainWindow.Show();
                            _MainWindow.Topmost = true;
                            DiscordRPCController.Init();
                        }
                        else if (_LoadingWindow != null)
                        {
                            _LoadingWindow.Show();
                            _MainWindow.Topmost = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogController.Write("Error while showing the main window: " + ex.Message,
                            LogController.LogType.Error);
                    }

                    try
                    {
                        File.Delete(AppDataFolder + ".fcsmw");
                    }
                    catch
                    {
                        Thread.Sleep(10); // just to make sure that it is not used by the other process
                        File.Delete(AppDataFolder + ".fcsmw");
                    }
                }
            });
            fcsmwTimer.Start();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            _reportCrash = new ReportCrash("joschua.hass.sh@gmail.com")
            {
                Silent = true
            };
            _reportCrash.RetryFailedReports();

            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Contains("-silent"))
                VTCManager.SilentAutoStartMode = true;
            _LoadingWindow = new LoadingWindow(this);
            if(!VTCManager.SilentAutoStartMode)
                _LoadingWindow.Show();
            MainWindow = _LoadingWindow;
        }

        public void LaunchMainWindow(List<Models.ControllerStatus> AppInitResults)
        {
            if (_MainWindow != null)
                return;
            _MainWindow = new Windows.MainWindow(this, AppInitResults);
            if (!VTCManager.SilentAutoStartMode)
                _MainWindow.Show();
            MainWindow = _MainWindow;

            _LoadingWindow.Close();
            _LoadingWindow = null;
        }

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

        public static void SendReport(Exception exception, string developerMessage = "")
        {
            if (exception is System.AggregateException) //sometimes caused by the Pusher client
            {
                if (string.IsNullOrWhiteSpace(exception.Source))
                {
                    if (exception.InnerException != null)
                    {
                        if(exception.InnerException is System.Net.Sockets.SocketException)
                        {
                            return;
                        }
                    }
                }
            }

            _reportCrash.Silent = false;
            _reportCrash.Send(exception);
        }

        public static void SendReportSilently(Exception exception, string developerMessage = "")
        {
            _reportCrash.Silent = true;
            _reportCrash.Send(exception);
        }
    }
}
