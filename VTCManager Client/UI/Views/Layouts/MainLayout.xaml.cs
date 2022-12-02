using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VTCManager.Plugins.ModSyncPro;

namespace VTCManager_Client.Views.Layouts
{
    /// <summary>
    /// Interaktionslogik für MainLayout.xaml
    /// </summary>
    public partial class MainLayout : Page
    {
        public DashBoards.Main maindash = null;
        private UI.Views.SettingsPage settingsPage = null;
        private ModSyncProPage modSyncProPage = null;
        private string CurrentPage = "Dashboard";
        private bool isSideBarAnimationRunning = false;
        private bool wasNavItemClicked = false;
        private bool isSidebarOpen = false;
        public MainLayout(Windows.MainWindow mainWindow)
        {
            InitializeComponent();

            SideBar.Visibility = Visibility.Collapsed;

            maindash = new DashBoards.Main();
            PageContent.Navigate(maindash);
            SBDashboardItem.Opacity = 1;

            mainWindow.KeyUp += Window_KeyUp;

            Controllers.NotificationController.Init();
        }
        private void CloseSideBarCompleted(object sender, EventArgs e)
        {
            ContentOverlay.Visibility = Visibility.Collapsed;
            SideBar.Visibility = Visibility.Collapsed;
            isSideBarAnimationRunning = false;
            wasNavItemClicked = false;
        }

        private void OpenSideBarCompleted(object sender, EventArgs e)
        {
            isSideBarAnimationRunning = false;
        }

        private void OpenSideBarButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenSideBar();
        }

        private void SideBarContentOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSideBarAnimationRunning)
                return;
            CloseSideBar();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SideBar.Height = this.ActualHeight;
            ContentOverlay.Height = this.ActualHeight;
            ContentOverlay.Width = this.ActualWidth;
            PageContent.Height = this.ActualHeight - TopBar.ActualHeight;
            PageContent.Width = this.ActualWidth;
            PageContentWrapper.Height = this.ActualHeight;
            PageContentWrapper.Width = this.ActualWidth;
            NotificationWrapper.Width = this.ActualWidth;
        }

        public void ShowNotifcation(string Text)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    NotificationTextLabel.Content = Text;
                    NotificationWrapper.Visibility = Visibility.Visible;
                    Storyboard sb = this.FindResource("ShowNotificationStoryboard") as Storyboard;
                    sb.Begin();
                }));
        }

        public void CloseNotifcation()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    Storyboard sb = this.FindResource("CloseNotificationStoryboard") as Storyboard;
                    sb.Completed += CloseNotificationStoryboard_Completed;
                    sb.Begin();
                }));
        }

        private void CloseNotificationStoryboard_Completed(object sender, EventArgs e)
        {
            NotificationWrapper.Visibility = Visibility.Collapsed;
        }

        private void SideBarItems_MouseEnter(object _sender, MouseEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    Border sender = (Border)_sender;
                    if (sender.Name != "SB" + CurrentPage + "Item")
                    {
                        Storyboard sb = new Storyboard
                        {
                            Duration = TimeSpan.FromMilliseconds(100)
                        };
                        QuadraticEase qe = new QuadraticEase
                        {
                            EasingMode = EasingMode.EaseInOut
                        };
                        DoubleAnimation da = new DoubleAnimation
                        {
                            From = 0.7,
                            To = 1,
                            Duration = TimeSpan.FromMilliseconds(100),
                            EasingFunction = qe,
                        };
                        Storyboard.SetTarget(da, sender);
                        Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));
                        sb.Children.Add(da);
                        sb.Begin();
                    }
                }));
        }

        private void SideBarItems_MouseLeave(object _sender, MouseEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    Border sender = (Border)_sender;
                    if (sender.Name != "SB" + CurrentPage + "Item")
                    {
                        Storyboard sb = new Storyboard
                        {
                            Duration = TimeSpan.FromMilliseconds(100)
                        };
                        QuadraticEase qe = new QuadraticEase
                        {
                            EasingMode = EasingMode.EaseInOut
                        };
                        DoubleAnimation da = new DoubleAnimation
                        {
                            From = 1,
                            To = 0.7,
                            Duration = TimeSpan.FromMilliseconds(100),
                            EasingFunction = qe,
                        };
                        Storyboard.SetTarget(da, sender);
                        Storyboard.SetTargetProperty(da, new PropertyPath(Border.OpacityProperty));
                        sb.Children.Add(da);
                        sb.Begin();
                    }
                }));
        }

        private void SBSettingsItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (wasNavItemClicked)
                return;
            wasNavItemClicked = true;
            CurrentPage = "Settings";
            if (CurrentPage == "Dashboard")
            {
                SideBarItems_MouseLeave(SBDashboardItem, null); //Fixes item still visually active
            }
            else
            {
                SideBarItems_MouseLeave(SBModSyncItem, null); //Fixes item still visually active
            }
            SBSettingsItem.Opacity = 1;
            if (settingsPage == null)
                settingsPage = new UI.Views.SettingsPage();
            PageContent.Navigate(settingsPage);
            CloseSideBar();
        }

        private void SBDashboardItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (wasNavItemClicked)
                return;
            wasNavItemClicked = true;
            CurrentPage = "Dashboard";
            if (CurrentPage == "Settings")
            {
                SideBarItems_MouseLeave(SBSettingsItem, null); //Fixes item still visually active
            }
            else
            {
                SideBarItems_MouseLeave(SBModSyncItem, null); //Fixes item still visually active
            }
            SBDashboardItem.Opacity = 1;
            PageContent.Navigate(maindash);
            CloseSideBar();
        }

        private void CloseSideBarButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSideBarAnimationRunning)
                return;
            CloseSideBar();
        }

        private void CloseSideBar()
        {
            if (!isSidebarOpen)
                return;
            isSideBarAnimationRunning = true;
            isSidebarOpen = false;
            (this.FindResource("CloseSideBarStoryboard") as Storyboard).Begin();
        }

        private void OpenSideBar()
        {
            if (isSidebarOpen)
                return;
            ContentOverlay.Visibility = Visibility.Visible;
            SideBar.Visibility = Visibility.Visible;
            isSideBarAnimationRunning = true;
            isSidebarOpen = true;
            (this.FindResource("OpenSideBarStoryboard") as Storyboard).Begin();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    if (isSidebarOpen)
                    {
                        CloseSideBar();
                    }
                    else
                    {
                        OpenSideBar();
                    }
                }));
            }
        }

        private void SBModSyncItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (wasNavItemClicked)
                return;
            wasNavItemClicked = true;
            if (CurrentPage == "Settings")
            {
                SideBarItems_MouseLeave(SBSettingsItem, null); //Fixes item still visually active
            } else
            {
                SideBarItems_MouseLeave(SBDashboardItem, null); //Fixes item still visually active
            }
            CurrentPage = "ModSyncPro";
            SBModSyncItem.Opacity = 1;
            if (modSyncProPage == null)
                modSyncProPage = new ModSyncProPage();
            PageContent.Navigate(modSyncProPage);
            CloseSideBar();
        }
    }
}