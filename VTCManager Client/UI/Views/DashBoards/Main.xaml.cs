using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VTCManager_Client.Controllers;

namespace VTCManager_Client.Views.DashBoards
{
    /// <summary>
    /// Interaktionslogik für Main.xaml
    /// </summary>
    public partial class Main : Page
    {

        private Timer UpdateDashboardTimer;
        private UI.Views.Models.MainDashboardViewModel viewModel = new UI.Views.Models.MainDashboardViewModel();
        public ImageSourceConverter sourceConverter = new ImageSourceConverter();
        private DurationConverter DurationConverter = new DurationConverter();
        private bool DashboardModeGameRunning = false;
        private ImageSource TruckIconWithFreight;
        private ImageSource TruckIconNoFreight;

        public Main()
        {
            viewModel.BackgroundImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(AppThemeController.GetDashboardBackground());
            viewModel.NoGameRunningImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(AppThemeController.GetNoGameRunningImage());

            DataContext = viewModel;
            InitializeComponent();

            viewModel.TruckVisibility = Visibility.Hidden;
            viewModel.CargoVisibility = Visibility.Hidden;
            viewModel.DestinationVisibility = Visibility.Hidden;

            NoGameRunningInfoPanel.Visibility = Visibility.Visible;
            GameRunningInfoPanel.Visibility = Visibility.Collapsed;

            Controllers.TelemetryController.Init();

            UpdateDashboardTimer = new Timer(300);
            UpdateDashboardTimer.Elapsed += new ElapsedEventHandler(UpdateDashboard);
            UpdateDashboardTimer.Start();

            BrushConverter brushConverter = new BrushConverter();
            viewModel.CurrentConnectionStateColor = (Brush)brushConverter.ConvertFromString("#e8e8e8"); //white
            viewModel.CurrentConnectionStateString = "not yet connected";

            if(Controllers.API.VTCM_WSController.client.State == PusherClient.ConnectionState.Connected)
            {
                UpdateConnectionStatus(Models.ConnectionState.Connected);
            }
            else
            {
                UpdateConnectionStatus(Models.ConnectionState.Disconnected);
            }

            TruckIconWithFreight = (ImageSource)sourceConverter.ConvertFromString("pack://application:,,,/Resources/Images/UI/Icons/Truck-Icon-With-Freight.png");
            TruckIconNoFreight = (ImageSource)sourceConverter.ConvertFromString("pack://application:,,,/Resources/Images/UI/Icons/Truck-Icon-No-Freight.png");

            var clockControlType = new VTCManager.Plugins.Clock.PluginInfo().WidgetControlType;
            var clockControlInstance = (UIElement)Activator.CreateInstance(clockControlType);
            Grid.SetColumn(clockControlInstance, 2);
            FirstRow.Children.Add(clockControlInstance);
        }

        private void UpdateDashboard(object sender, ElapsedEventArgs e)
        {
            // init not done
            if (!Controllers.TelemetryController.InitDone)
                return;
            // game not running
            if (Controllers.TelemetryController.TelemetryData.Game == SCSSdkClient.SCSGame.Unknown)
            {
                ShowGameNotRunningInfoPanel();
                if(!Controllers.ControllerManager.MainWindow.IsVisible)
                    Controllers.DiscordRPCController.ShutDown();
                if (Controllers.DiscordRPCController.CurrentRPCStatus != Controllers.DiscordRPCController.RPCStatus.IDLE)
                    Controllers.DiscordRPCController.SetPresence(Controllers.DiscordRPCController.RPCStatus.IDLE);
                return;
            }
            Controllers.DiscordRPCController.Init();
            ShowGameRunningInfoPanel();
            viewModel.CurrentSpeedString = Math.Abs((int)Controllers.TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph) + " km/h";
            if (!String.IsNullOrWhiteSpace(Controllers.TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + Controllers.TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name))
            {
                viewModel.CurrentTruckModelString = Controllers.TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + Controllers.TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name;
                viewModel.TruckVisibility = Visibility.Visible;
                if (!Controllers.TelemetryController.JobRunning && Controllers.DiscordRPCController.CurrentRPCStatus != Controllers.DiscordRPCController.RPCStatus.FreeRoam)
                    Controllers.DiscordRPCController.SetPresence(Controllers.DiscordRPCController.RPCStatus.FreeRoam);
            }
            else
                viewModel.TruckVisibility = Visibility.Hidden;

            if (Controllers.TelemetryController.TelemetryData.NavigationValues.SpeedLimit.Kph > 0)
            {
                viewModel.SpeedLimitString = ((int)Controllers.TelemetryController.TelemetryData.NavigationValues.SpeedLimit.Kph).ToString();
                viewModel.SpeedLimitSignVisibility = Visibility.Visible;
            }
            else
                viewModel.SpeedLimitSignVisibility = Visibility.Hidden;

            if (Controllers.TelemetryController.TelemetryData.NavigationValues.NavigationDistance > 0)
            {
                DateTime ArrivalTime = (Controllers.TelemetryController.TelemetryData.CommonValues.GameTime.Date).Add(TimeSpan.FromSeconds(Controllers.TelemetryController.TelemetryData.NavigationValues.NavigationTime));
                viewModel.ArrivalTimeString = ArrivalTime.ToString("HH:mm");
                TimeSpan ArrivalTimeLeft = TimeSpan.FromSeconds(Controllers.TelemetryController.TelemetryData.NavigationValues.NavigationTime);
                viewModel.ArrivalTimeLeftString = ((int)ArrivalTimeLeft.TotalHours).ToString("00") + ":" + ArrivalTimeLeft.Minutes.ToString("00");
                viewModel.ArrivalDistanceLeftString = (((int)Controllers.TelemetryController.TelemetryData.NavigationValues.NavigationDistance) / 1000).ToString();
                viewModel.NavigationInfoPanelVisibility = Visibility.Visible;
            }
            else
                viewModel.NavigationInfoPanelVisibility = Visibility.Hidden;

            if (Controllers.TelemetryController.JobRunning)
            {
                viewModel.CargoVisibility = Visibility.Visible;
                viewModel.DestinationVisibility = Visibility.Visible;
                viewModel.CurrentCargoString = Controllers.TelemetryController.TelemetryData.JobValues.CargoValues.Name + " (" + ((int)Controllers.TelemetryController.TelemetryData.JobValues.CargoValues.Mass) / 1000 + "t)";
                viewModel.CurrentDestinationString = Controllers.TelemetryController.TelemetryData.JobValues.CityDestination + ", " + Controllers.TelemetryController.TelemetryData.JobValues.CompanyDestination;
                viewModel.TruckImageSource = TruckIconWithFreight;
            }
            else
            {
                viewModel.CargoVisibility = Visibility.Hidden;
                viewModel.DestinationVisibility = Visibility.Hidden;
                viewModel.TruckImageSource = TruckIconNoFreight;
            }
        }

        private void ShowGameNotRunningInfoPanel()
        {
            if (!DashboardModeGameRunning)
                return;
            DashboardModeGameRunning = false;
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    NoGameRunningInfoPanel.Visibility = Visibility.Visible;
                    Storyboard sb = this.FindResource("SwitchToGameNotRunningInfoPanel") as Storyboard;
                    DoubleAnimation HeightAnimation = new DoubleAnimation(248, 143, (Duration)DurationConverter.ConvertFromString("00:00:01"), FillBehavior.Stop);
                    QuadraticEase quadraticEase = new QuadraticEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                    HeightAnimation.EasingFunction = quadraticEase;
                    Storyboard.SetTargetName(HeightAnimation, "GameRunningInfoPanel");
                    Storyboard.SetTargetProperty(HeightAnimation, new PropertyPath("Height"));
                    sb.Children.Add(HeightAnimation);

                    DoubleAnimation HeightAnimation2 = new DoubleAnimation(248, 143, (Duration)DurationConverter.ConvertFromString("00:00:01"), FillBehavior.Stop);
                    QuadraticEase quadraticEase2 = new QuadraticEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                    HeightAnimation2.EasingFunction = quadraticEase2;
                    Storyboard.SetTargetName(HeightAnimation2, "NoGameRunningInfoPanel");
                    Storyboard.SetTargetProperty(HeightAnimation2, new PropertyPath("Height"));
                    sb.Children.Add(HeightAnimation2);
                    sb.Begin();
                }));
        }

        private void ShowGameRunningInfoPanel()
        {
            if (DashboardModeGameRunning)
                return;
            DashboardModeGameRunning = true;
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    GameRunningInfoPanel.Visibility = Visibility.Visible;
                    Storyboard sb = this.FindResource("SwitchToGameRunningInfoPanel") as Storyboard;
                    DoubleAnimation HeightAnimation = new DoubleAnimation(143, 248, (Duration)DurationConverter.ConvertFromString("00:00:01"), FillBehavior.Stop);
                    QuadraticEase quadraticEase = new QuadraticEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                    HeightAnimation.EasingFunction = quadraticEase;
                    Storyboard.SetTargetName(HeightAnimation, "GameRunningInfoPanel");
                    Storyboard.SetTargetProperty(HeightAnimation, new PropertyPath("Height"));
                    sb.Children.Add(HeightAnimation);
                    DoubleAnimation HeightAnimation2 = new DoubleAnimation(143, 248, (Duration)DurationConverter.ConvertFromString("00:00:01"), FillBehavior.Stop);
                    QuadraticEase quadraticEase2 = new QuadraticEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                    HeightAnimation2.EasingFunction = quadraticEase2;
                    Storyboard.SetTargetName(HeightAnimation2, "NoGameRunningInfoPanel");
                    Storyboard.SetTargetProperty(HeightAnimation2, new PropertyPath("Height"));
                    sb.Children.Add(HeightAnimation2);

                    sb.Begin();
                }));
        }

        private void SwitchToGameRunningInfoPanelStoryboard_Completed(object sender, EventArgs e)
        {
            NoGameRunningInfoPanel.Visibility = Visibility.Collapsed;
            GameRunningInfoPanel.Height = 248;
            DashboardModeGameRunning = true;
        }

        private void SwitchToGameNotRunningInfoPanelStoryboard_Completed(object sender, EventArgs e)
        {
            GameRunningInfoPanel.Visibility = Visibility.Collapsed;
            GameRunningInfoPanel.Height = 143;
            DashboardModeGameRunning = false;
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Controllers.NotificationController.Notificate(Controllers.NotificationController.NotificationType.JobStarted, "Job Started");
        }

        public void UpdateConnectionStatus(Models.ConnectionState connectionState)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    BrushConverter brushConverter = new BrushConverter();
                    switch (connectionState)
                    {
                        case Models.ConnectionState.Connected:
                            viewModel.CurrentConnectionStateColor = (Brush)brushConverter.ConvertFromString("#13bf00"); //green
                            viewModel.CurrentConnectionStateString = "Connected";
                            break;
                        case Models.ConnectionState.Disconnected:
                            viewModel.CurrentConnectionStateColor = (Brush)brushConverter.ConvertFromString("#c22d2d"); //red
                            viewModel.CurrentConnectionStateString = "Disconnected";
                            break;
                    }
                }));
        }
    }
}
