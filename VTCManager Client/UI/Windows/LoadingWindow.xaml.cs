using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace VTCManager_Client.Windows
{
    public partial class LoadingWindow : Window
    {
        private string OriginalStatusLabelText;
        private Timer UpdateStatusLabelTimer;

        public bool IgnoreCloseEvent = false;
        private App app;

        private bool isAppInitializing = false;

        public LoadingWindow(App _app)
        {
            app = _app;
            Console.Write("Booting...");
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (MessageBox.Show("We have detected that you are using an unsupported operating system. We can't guarantee that this application will be working on this OS. Use it at your own risk.", "VTCManager: Unsupported Operating System Detected", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    Environment.Exit(0);
            }

            if (VTCManager.SilentAutoStartMode)
            {
                Controllers.LogController.Write("Starting in silent mode");
                this.Hide();
                VCCLogoIntroPlayer_MediaEnded(null, null);
                return;
            }

            //UI stuff
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while building the main window.\nError message: " + ex.Message + "\nSource:"+ ex.Source + "\nInnerException:" + ex.InnerException + "\nStackTrace: " + ex.StackTrace, "VTCManager Client: Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            OriginalStatusLabelText = StatusLabel.Content.ToString();
            VersionLabel.Content = VTCManager.Version;
            VCCLogoIntroPlayer.Visibility = Visibility.Visible;
            LoadingInformationScreen.Opacity = 0;
            UpdateProgressBar.Visibility = Visibility.Collapsed;
            UpdateProgressBar.Value = 0;

            UpdateStatusLabelTimer = new Timer(600);
            UpdateStatusLabelTimer.Elapsed += new ElapsedEventHandler(UpdateStatusLabelEvent);
            UpdateStatusLabelTimer.Start();
            Console.WriteLine();
        }

        // Status Label 3 dots animation
        private void UpdateStatusLabelEvent(object sender, ElapsedEventArgs e)
        {
            this.StatusLabel.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    if (StatusLabel.Content.ToString().Length <= OriginalStatusLabelText.Length)
                    {
                        //set to original
                        StatusLabel.Content = OriginalStatusLabelText + "...";
                    }
                    else
                    {
                        StatusLabel.Content = StatusLabel.Content.ToString().Remove(StatusLabel.Content.ToString().Length - 1);
                    }
                }));
        }

        //init the controllers and boot the app
        private void VCCLogoIntroPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isAppInitializing)
                return;

            isAppInitializing = true;

            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    if (!VTCManager.SilentAutoStartMode)
                    {
                        VCCLogoIntroPlayer.Stop();
                        Storyboard sb = this.FindResource("IntroFadeOut") as Storyboard;
                        sb.Begin();
                    }

                    // make this async so the window doesn't freeze
                    Task.Run(() =>
                    {
                        List<Models.ControllerStatus> app_init_result = Controllers.ControllerManager.BootInit();
                        // we need the dispatcher because the window requires an STATHREAD
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() =>
                            {
                                IgnoreCloseEvent = true;
                                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                                app.LaunchMainWindow(app_init_result);
                            }));
                    });
                }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (IgnoreCloseEvent)
                return;
            Controllers.LogController.Write("Shutting down (user closed loading window)...");
            Controllers.ControllerManager.ShutDown();
            Environment.Exit(0);
        }

        public void ChangeStatusText(string new_status)
        {
            if (VTCManager.SilentAutoStartMode)
                return;
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    this.StatusLabel.Content = new_status;
                    this.OriginalStatusLabelText = new_status;
                }));
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //dragging the window
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Console.WriteLine("Showing VTCM logo intro");
            Console.WriteLine("Has video: " + VCCLogoIntroPlayer.HasVideo);
            Console.WriteLine("current path: " + System.Reflection.Assembly.GetEntryAssembly().Location);
            Console.WriteLine(File.Exists("Resources/Videos/VCC-logo-animated.mp4"));
            VCCLogoIntroPlayer.Play();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                VCCLogoIntroPlayer.Pause(); // prevent that the MediaEnded event will be executed twice
                VCCLogoIntroPlayer_MediaEnded(null, null); //skip VCC logo animation
            }
        }
    }
}
