using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VTCManager.Logging;
using VTCManager_Client.Controllers;

namespace VTCManager_Client.Views
{
    /// <summary>
    /// Interaktionslogik für Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private readonly String LogPrefix = "[LoginUI] ";
        private String VTCMServerHost = "https://api.vtcmanager.eu/";
        
        public Login()
        {
            if (AppInfo.UseLocalServer)
                VTCMServerHost = "http://localhost:8000/";

            InitializeComponent();
            LoginWebBrowser.IsBrowserInitializedChanged += LoginWebBrowser_IsBrowserInitializedChanged;
        }

        private void LoginWebBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!LoginWebBrowser.IsBrowserInitialized)
                return;
            LoginWebBrowser.FrameLoadEnd += LoginWebBrowser_FrameLoadEnd;
            LoginWebBrowser.AddressChanged += LoginWebBrowser_AddressChanged;
            LoginWebBrowser.Address = VTCMServerHost + "auth/vcc/desktop-client/redirect";
        }

        private void LoginWebBrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LogController.Write("Changed address to " + e.NewValue + " from " + e.OldValue);

                if (e.NewValue.ToString().StartsWith(VTCMServerHost + "auth/vcc/desktop-client/callback"))
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    LoginWebBrowser.Visibility = Visibility.Hidden;
                }));
            }else if (e.NewValue.ToString().StartsWith("https://vcc-online.eu/register"))
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    LoginWebBrowser.GetBrowser().StopLoad();
                    LoginWebBrowser.GetBrowser().MainFrame.LoadUrl(VTCMServerHost + "auth/vcc/desktop-client/redirect");
                }));
                MessageBox.Show("Please create a new VCC account on the official VCC website in your webbrowser: https://vcc-online.eu/register ", "Warning: Can't register a new account in the client", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (e.NewValue.ToString().StartsWith("https://vcc-online.eu/forgot-password"))
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    LoginWebBrowser.GetBrowser().StopLoad();
                    LoginWebBrowser.GetBrowser().MainFrame.LoadUrl(VTCMServerHost + "auth/vcc/desktop-client/redirect");
                }));
                MessageBox.Show("Please reset your password on the official VCC website in your webbrowser: https://vcc-online.eu/forgot-password ", "Warning: Can't reset password in the client", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (e.NewValue.ToString().StartsWith("https://vcc-online.eu/login") || e.NewValue.ToString().StartsWith("https://vcc-online.eu/two-factor-challenge"))
            {

            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    LoginWebBrowser.GetBrowser().StopLoad();
                    LoginWebBrowser.GetBrowser().MainFrame.LoadUrl(VTCMServerHost + "auth/vcc/desktop-client/redirect");
                }));
            }
        }

        private void LoginWebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            e.Frame.GetTextAsync().ContinueWith(taskHtml =>
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
            new Action(() =>
            {
                LogController.Write(e.Url + "Res: " + taskHtml.Result);
            }));
            });

            if (e.Url.StartsWith(VTCMServerHost + "auth/vcc/desktop-client/callback") && e.Frame.IsMain)
            {
                e.Frame.GetTextAsync().ContinueWith(taskHtml =>
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    try
                    {
                        Models.API.VTCManager.AuthTokenResponse authTokenResponse = JsonConvert.DeserializeObject<Models.API.VTCManager.AuthTokenResponse>(taskHtml.Result);
                        if (authTokenResponse.message == "OK")
                        {
                            CheckToken(authTokenResponse.token);
                        }
                        else
                        {
                            LogController.Write(LogPrefix + "Getting the auth token failed. Code: 1");
                            LoginWebBrowser.Visibility = Visibility.Visible;
                            MessageBox.Show("Oh no. An error occurred while signing in.", "Error: Can't get auth token", MessageBoxButton.OK, MessageBoxImage.Warning);
                            LoginWebBrowser.GetBrowser().MainFrame.LoadUrl(VTCMServerHost + "auth/vcc/desktop-client/redirect");
                            return;
                        }
                    }
                    catch(Exception ex)
                    {
                        LogController.Write(LogPrefix + "Getting the auth token failed. Code: 2 Exception: " + ex.Message);
                        LoginWebBrowser.Visibility = Visibility.Visible;
                        MessageBox.Show("Oh no. An error occurred while signing in.", "Error: Can't get auth token", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoginWebBrowser.GetBrowser().MainFrame.LoadUrl(VTCMServerHost + "auth/vcc/desktop-client/redirect");
                        return;
                    }
                }));
                });
            }
        }

        private void CheckToken(string auth_key)
        {
            Controllers.AuthDataController.SetAPIToken(auth_key);
            Thread.Sleep(500);
            if (!Controllers.API.VTCM_APIController.IsAuthTokenValid())
            {
                LogController.Write(LogPrefix + "Checking the auth token failed. Couldn't get user data.");
                LoginWebBrowser.Visibility = Visibility.Visible;
                MessageBox.Show("Oh no. An error occurred while signing in.", "Error: AuthToken Invalid", MessageBoxButton.OK,MessageBoxImage.Warning);
                LoginWebBrowser.GetBrowser().MainFrame.LoadUrl(VTCMServerHost + "auth/vcc/desktop-client/redirect");
                return;
            }

            Controllers.API.VTCM_APIController.ActivateDataSync();

            Windows.MainWindow mainWindow = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(Windows.MainWindow))
                    {
                        mainWindow = (window as Windows.MainWindow);
                        mainWindow.ShowDashboard();
                    }
                }
            });
        }
    }

}
