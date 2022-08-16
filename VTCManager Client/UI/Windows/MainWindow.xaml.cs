using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VTCManager_Client.Controllers;
using System.Drawing;
using Application = System.Windows.Forms.Application;
using System.ComponentModel;
using SCSSdkClient;

namespace VTCManager_Client.Windows
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer TopBarMouseClickTimer;
        private double NormalWindowHeight = 0;
        private double NormalWindowWidth = 0;
        public Views.Layouts.MainLayout dashPage = null;
        private Views.Login loginPage = null;
        private System.Windows.Media.Brush WindowCloseButtonHoverBrush = null;
        private System.Windows.Media.Brush ChangeWindowSizeBtnHoverBrush = null;

        private NotifyIcon TrayIcon;

        private App app;

        public MainWindow(App _app, List<Models.ControllerStatus> app_init_result)
        {
            app = _app;
            InitializeComponent();

            if(!AppInfo.SilentAutoStartMode)
                Controllers.DiscordRPCController.Init();

            //init tray icon
            TrayIcon = new NotifyIcon()
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().Location.Replace("VTCManager Client.exe", "vtcmanager_logo.ico")),
                ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] {
                    new System.Windows.Forms.MenuItem("Open", Open),
                    new System.Windows.Forms.MenuItem("Exit", ExitApplication)
                }),
                Visible = true
            };
            TrayIcon.Click += Open;

            //convert Brushes
            BrushConverter brushConverter = new BrushConverter();
            WindowCloseButtonHoverBrush = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FFD12121");
            ChangeWindowSizeBtnHoverBrush = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#095db8");

            AppInfo.CurrentWindow = Models.Enums.AppWindow.MainWindow;

            TopBarMouseClickTimer = new System.Timers.Timer(500);
            TopBarMouseClickTimer.Elapsed += new ElapsedEventHandler(TopBarMouseClickTimerEvent);

            NormalWindowHeight = this.ActualHeight;
            NormalWindowWidth = this.ActualWidth;

            bool show_login = false;
            foreach (Models.ControllerStatus info in app_init_result)
            {
                switch (info)
                {
                    case Models.ControllerStatus.VTCMShowLoginTokenExpired:
                        show_login = true;
                        break;
                    case Models.ControllerStatus.VTCMShowLogin:
                        show_login = true;
                        break;
                    default:
                        break;
                }
            }

            if (show_login)
            {
                loginPage = new Views.Login();
                MainFrame.Content = loginPage;
            }
            else
            {
                ShowDashboard();
            }

            Controllers.ControllerManager.MainWindowInit();

            //show changelog
            if(StorageController.Config.last_version_used != AppInfo.Version)
            {
                Task.Run(() =>
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        Controllers.ModalController.AddModalToQueue(new UI.Views.Changelog());
                    })
                );
                });
            }
        }

        private void Open(object sender, EventArgs e)
        {
            this.Show();
            Controllers.DiscordRPCController.Init();
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            TrayIcon.Visible = false; // hide it because it doesn't auto hide after shutdown of the app
            Controllers.LogController.Write("Shutting down (user closed main window)...");
            Controllers.ControllerManager.ShutDown();
            Environment.Exit(0);
        }

        public void ShowDashboard()
        {
            dashPage = new Views.Layouts.MainLayout(this);
            MainFrame.Content = dashPage;
            loginPage = null;
        }

        private void Close_Btn_Clicked(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //dragging the window
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
            if (TopBarMouseClickTimer.Enabled)
            {
                if (this.Height != SystemParameters.FullPrimaryScreenHeight || this.Width != SystemParameters.FullPrimaryScreenWidth)
                {
                    this.Left = 0;
                    this.Top = 0;
                    this.Height = SystemParameters.FullPrimaryScreenHeight;
                    this.Width = SystemParameters.FullPrimaryScreenWidth;
                }
                else
                {
                    this.Height = NormalWindowHeight;
                    this.Width = NormalWindowWidth;
                }
                TopBarMouseClickTimer.Stop();
            }
            else
                TopBarMouseClickTimer.Start();

        }

        private void TopBarMouseClickTimerEvent(object sender, ElapsedEventArgs e)
        {
            TopBarMouseClickTimer.Stop();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    ResizeContent();
                    ResizeTopBar();
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (Window window in System.Windows.Application.Current.Windows)
                        {
                            if (window.GetType() == typeof(Windows.LoadingWindow))
                            {
                                LoadingWindow loadwin = (window as Windows.LoadingWindow);
                                loadwin.Close();
                            }
                        }
                    });
                })
            );
        }

        private void ResizeContent()
        {
            ContentWrapper.Width = this.ActualWidth;
            ContentWrapper.Height = this.ActualHeight - WindowCloseButton.ActualHeight; //minus top bar
            Modal.MaxWidth = this.ActualWidth - 40;
            Modal.MaxHeight = this.ActualHeight - WindowCloseButton.ActualHeight - 40;
            ModalFrame.MaxWidth = this.ActualWidth - 40;
            ModalFrame.MaxHeight = this.ActualHeight - WindowCloseButton.ActualHeight - 40;
            MainFrame.Width = this.ActualWidth;
            MainFrame.Height = this.ActualHeight - WindowCloseButton.ActualHeight;
            MainFrameOverlay.Width = this.ActualWidth;
            MainFrameOverlay.Height = this.ActualHeight - WindowCloseButton.ActualHeight;
        }

        private void ResizeTopBar()
        {
            TopBar.Width = this.ActualWidth - WindowTopBarIconWrapper.ActualWidth; //can't done directly in xaml. Need this for dragging the window
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //don't exit the app by closing. just hide the window
            e.Cancel = true;
            this.Hide();
            if(TelemetryController.TelemetryData.Game == SCSGame.Unknown)
                Controllers.DiscordRPCController.ShutDown();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeContent();
            ResizeTopBar();
            if (this.ActualHeight != SystemParameters.FullPrimaryScreenHeight && this.ActualWidth != SystemParameters.FullPrimaryScreenWidth)
            {
                NormalWindowHeight = this.ActualHeight;
                NormalWindowWidth = this.ActualWidth;
            }
        }

        public void ShowModal(Page page)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    MainFrameOverlay.Visibility = Visibility.Visible;
                    Modal.Visibility = Visibility.Visible;
                    ModalFrame.Navigate(page);
                    Storyboard sb = this.FindResource("ShowModalAnimation") as Storyboard;
                    sb.Begin();
                })
            );
        }

        public void HideModal()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    Storyboard sb = this.FindResource("HideModalAnimation") as Storyboard;
                    sb.Completed += HideModalAnimationSb_Completed;
                    sb.Begin();
                })
            );
        }

        private void HideModalAnimationSb_Completed(object sender, EventArgs e)
        {
            MainFrameOverlay.Visibility = Visibility.Hidden;
            Modal.Visibility = Visibility.Hidden;
        }

        private void MainFrameOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Controllers.ModalController.CloseCurrentModal();
        }

        private void ChangeWindowSizeBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    WindowMaximizeIcon.Visibility = Visibility.Hidden;
                    WindowRestoreIcon.Visibility = Visibility.Visible;
                    return;
                case WindowState.Normal:
                    WindowMaximizeIcon.Visibility = Visibility.Visible;
                    WindowRestoreIcon.Visibility = Visibility.Hidden;
                    return;
            }
        }

        private void WindowCloseButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            WindowCloseButton.Background = WindowCloseButtonHoverBrush;
        }

        private void WindowCloseButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            WindowCloseButton.Background = System.Windows.Media.Brushes.Black;
        }

        private void ChangeWindowSizeBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeWindowSizeBtn.Background = ChangeWindowSizeBtnHoverBrush;
        }

        private void ChangeWindowSizeBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeWindowSizeBtn.Background = System.Windows.Media.Brushes.Black;
        }
    }
}
